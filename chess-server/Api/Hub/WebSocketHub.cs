using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;
using System.Threading.Channels;
using chess_server.Services;
using Shared;
using Shared.Dtos;
using Shared.Exceptions;
using Shared.Logger;
using Shared.WebSocketMessages;

namespace chess_server.Api.Hub;

/// <summary>
/// Defines the interface for a websocket hub that manages connected clients.
/// </summary>
public interface IWebSocketHub
{
    /// <summary>
    /// Accepts and handles a new WebSocket connection request from an HTTP listener context.
    /// </summary>
    /// <param name="context">The HTTP listener context containing the WebSocket upgrade request and querystring.</param>
    /// <returns>A task that completes when the connection has been handled.</returns>
    Task HandleConnectionRequest(HttpListenerContext context);
}

/// <summary>
/// Concrete implementation of <see cref="IWebSocketHub"/> that manages connected clients and a central input channel.
/// </summary>
public class WebSocketHub : IWebSocketHub
{
    /// <summary>
    /// The game service used to manage game logic.
    /// </summary>
    private readonly IGameService _gameService;
    /// <summary>
    /// The dictionary of connected clients, keyed by their unique identifiers.
    /// </summary>
    private readonly ConcurrentDictionary<Guid, WebSocketClient> _clients = new();
    /// <summary>
    /// The dictionary of active games, keyed by their unique game identifiers.
    /// </summary>
    private readonly ConcurrentDictionary<Guid, ActiveGame> _games = new();
    /// <summary>
    /// The list of clients that are currently waiting for a game to play, used for matchmaking purposes.
    /// </summary>
    private readonly List<Guid> _waitingClients = new();
    /// <summary>
    /// The JSON parser used for serializing and deserializing messages.
    /// </summary>
    private readonly JsonParser _jsonParser = new();
    /// <summary>
    /// The channel for incoming notifications to be processed and sent to clients.
    /// </summary>
    private readonly Channel<Notification> _notificationChannel = Channel.CreateUnbounded<Notification>();
    /// <summary>
    /// Provides the writer for the notification channel.
    /// </summary>
    public ChannelWriter<Notification> NotificationWriter => _notificationChannel.Writer;

    public WebSocketHub(IGameService gameService)
    {
        _gameService = gameService;
        Task.Run(ProcessNotificationsAsync);
    }
    
    /// <summary>
    /// Unregisters a client from the hub by removing it from the clients' dictionary.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to unregister.</param>
    /// <returns>A task that represents the asynchronous unregister operation.</returns>
    private Task UnregisterClient(Guid userId)
    {
        _clients.TryRemove(userId, out _);
        GameLogger.Info($"Unregistered client {userId}");
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Registers a new client in the hub by adding it to the clients' dictionary.
    /// </summary>
    /// <param name="client">The <see cref="WebSocketClient"/> instance to register.</param>
    /// <returns>A task that represents the asynchronous register operation.</returns>
    private Task RegisterClient(WebSocketClient client)
    {
        _clients.TryAdd(client.Id, client);
        GameLogger.Info($"Registered client {client.Id} in websocket");
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Handles an incoming WebSocket connection request. Validates the query parameters, accepts the WebSocket
    /// upgrade and registers a new <see cref="WebSocketClient"/> for the given user id.
    /// </summary>
    /// <param name="context">The <see cref="HttpListenerContext"/> representing the incoming request.</param>
    /// <returns>A task that completes after the connection has been accepted and the client registered.</returns>
    /// <exception cref="Shared.Exceptions.BadParameters">Thrown when the required query parameter is missing or invalid.</exception>
    public async Task HandleConnectionRequest(HttpListenerContext context)
    {
        GameLogger.Info("Parsing the userId.");
        
        var userIdStr = context.Request.QueryString["userId"];
        if (string.IsNullOrWhiteSpace(userIdStr))
            throw new BadParameters("NO_USERID");

        if (!Guid.TryParse(userIdStr, out var userId))
            throw new BadParameters("INVALID_USERID");
        
        var wsContext = await context.AcceptWebSocketAsync(subProtocol: null);
        var socket = wsContext.WebSocket;
        
        var client = new WebSocketClient(userId, socket);
        client.MessageReceived += DispatchToService;
        client.ClientDisconnected += UnregisterClient;

        await RegisterClient(client);
        
        GameLogger.Info($"New client connected with {userIdStr}");
    }
    
    /// <summary>
    /// Task that continuously processes notifications from the notification channel
    /// </summary>
    private async Task ProcessNotificationsAsync()
    {
        await foreach (var notification in _notificationChannel.Reader.ReadAllAsync())
        {
            if (_clients.TryGetValue(notification.UserId, out var client))
            {
                await client.SendAsync(notification.Message);
                GameLogger.Info($"Sent notification to user {notification.UserId}");
            }
            else
            {
                GameLogger.Warning($"Client {notification.UserId} not found for notification");
            }
        }
    }

    /// <summary>
    /// Dispatches the received message to the appropriate service based on the message type.
    /// </summary>
    /// <param name="messageType">The type of the message to dispatch.</param>
    /// <param name="payload">The payload of the message as a JSON element.</param>
    /// <param name="clientId">The id of the client who send the message</param>
    /// <returns>A task that represents the asynchronous dispatch operation.</returns>
    private async Task DispatchToService(string messageType, JsonElement? payload, Guid clientId)
    {
        GameLogger.Debug($"Dispatching {messageType} message to service for client {clientId}");
        switch (messageType)
        {
            case MessageType.CreateGame:
                await HandleCreateGame(payload!.Value, clientId);
                break;
            case MessageType.JoinGame:
                await HandleJoinGame(payload!.Value, clientId);
                break;
            case MessageType.GameTurn:
                await HandleMakeMove(payload!.Value, clientId);
                break;
            case MessageType.GameOver:
                await HandleGameOver(payload!.Value, clientId);
                break;
            case MessageType.SearchGame:
                await HandleSearchGame(clientId);
                break;
            case MessageType.CancelSearch:
                _waitingClients.Remove(clientId);
                GameLogger.Debug($"Client {clientId} cancelled search");
                break;
            default:
                GameLogger.Error($"Unknown message type {messageType}");
                break;
        }
        
        await Task.CompletedTask;
    }
    
    private async Task HandleSearchGame(Guid clientId)
    {
        GameLogger.Debug($"Client {clientId} is searching for a game");

        Guid opponentId = Guid.Empty;
        bool matchFound = false;

        lock (_waitingClients)
        {
            if (_waitingClients.Count > 0)
            {
                opponentId = _waitingClients[0];
                _waitingClients.RemoveAt(0);
                matchFound = true;
                GameLogger.Debug($"Found opponent {opponentId} for client {clientId}");
            }
            else
            {
                _waitingClients.Add(clientId);
                GameLogger.Debug($"No opponent found for client {clientId}, added to waiting list");
            }
        }

        if (!matchFound) return;
        
        await StartGameBetween(clientId, opponentId);
    }

    /// <summary>
    /// Creates a game directly between two connected clients and notifies both with a StartGame message.
    /// Used for matchmaking where no invitation flow is needed.
    /// </summary>
    /// <param name="whiteClientId">The client that will play as white.</param>
    /// <param name="blackClientId">The client that will play as black.</param>
    private async Task StartGameBetween(Guid whiteClientId, Guid blackClientId)
    {
        if (!_clients.TryGetValue(whiteClientId, out var whiteClient))
        {
            GameLogger.Error($"White client {whiteClientId} not found when starting game");
            return;
        }

        if (!_clients.TryGetValue(blackClientId, out var blackClient))
        {
            GameLogger.Error($"Black client {blackClientId} not found when starting game");
            return;
        }

        var game = await _gameService.CreateGame(whiteClientId);

        lock (game.SyncRoot)
        {
            _gameService.JoinGame(game, blackClientId);
        }

        _games.TryAdd(game.Id, game);
        GameLogger.Info($"Matchmaking: created game {game.Id} between {whiteClientId} (white) and {blackClientId} (black)");

        var whiteStartMessage = new WebSocketMessage
        {
            Type = MessageType.StartGame,
            Payload = _jsonParser.SerializeToJsonElement(new StartGamePayload
            {
                GameId = game.Id,
                Color = "white"
            })
        };

        var blackStartMessage = new WebSocketMessage
        {
            Type = MessageType.StartGame,
            Payload = _jsonParser.SerializeToJsonElement(new StartGamePayload
            {
                GameId = game.Id,
                Color = "black"
            })
        };

        await whiteClient.SendAsync(whiteStartMessage);
        await blackClient.SendAsync(blackStartMessage);
    }

    /// <summary>
    /// Handles the creation of a new game when a client sends a CreateGame message.
    /// </summary>
    /// <param name="payload">The payload of the message</param>
    /// <param name="clientId">The id of the sender</param>
    private async Task HandleCreateGame(JsonElement payload, Guid clientId)
    {
        GameLogger.Debug($"Creating game {clientId}");
        var createGamePayload = _jsonParser.DeserializeJsonElement<CreateGamePayload>(payload);
        if (createGamePayload == null)
        {
            GameLogger.Error("Failed to deserialize CreateGamePayload");
            return;
        }
        
        var game = await _gameService.CreateGame(clientId);
        _games.TryAdd(game.Id, game);
        GameLogger.Debug($"Created new game {game.Id} by client {clientId}");

        if (!_clients.TryGetValue(createGamePayload.OpponentId, out var opp))
        {
            GameLogger.Error($"Opponent client with id {clientId} not found in clients dictionary");
            return;
        }
        
        var inviteMessage= new WebSocketMessage
        {
            Type = MessageType.GameInvitation,
            Payload = _jsonParser.SerializeToJsonElement(new GameInvitationPayload
            {
                GameId = game.Id,
                InviterId = clientId
            })
        };
        
        
        await opp.SendAsync(inviteMessage);
        GameLogger.Info($"Client {clientId} joined game {game.Id} and invited opponent {createGamePayload.OpponentId}");
    }
    
    /// <summary>
    /// Handles a client joining an existing game.
    /// </summary>
    /// <param name="payload">The payload of the message</param>
    /// <param name="clientId">The id of the sender</param>
    private async Task HandleJoinGame(JsonElement payload, Guid clientId)
    {
        GameLogger.Debug($"Joining game {clientId}");
        var joinGamePayload = _jsonParser.DeserializeJsonElement<JoinGamePayload>(payload);
        if (joinGamePayload == null)
        {
            GameLogger.Error("Failed to deserialize CreateGamePayload");
            return;
        }
        
        if (!_games.TryGetValue(joinGamePayload.GameId, out var game))
        {
            GameLogger.Error("Game not found");
            return;
        }
        
        lock (game.SyncRoot) 
        {
            _gameService.JoinGame(game, clientId);
        }
        
        GameLogger.Info($"Client {clientId} joined game {joinGamePayload.GameId}");
        
        _clients.TryGetValue(game.GetWhitePlayerId(), out var whiteClient);
        _clients.TryGetValue(game.GetBlackPlayerId(), out var blackClient);
        
        if (whiteClient == null || blackClient == null)
        {
            GameLogger.Error("One of the clients is null when sending StartGame message");
            return;
        }
        
        // inform players about game start
        var whiteStartMessage = new WebSocketMessage
        {
            Type = MessageType.StartGame,
            Payload = _jsonParser.SerializeToJsonElement(new StartGamePayload
            {
                GameId = joinGamePayload.GameId,
                Color = "white"
            })
        };

        var blackStartMessage = new WebSocketMessage
        {
            Type = MessageType.StartGame,
            Payload = _jsonParser.SerializeToJsonElement(new StartGamePayload
            {
                GameId = joinGamePayload.GameId,
                Color = "black"
            })
        };

        await whiteClient.SendAsync(whiteStartMessage);
        await blackClient.SendAsync(blackStartMessage);
        
        await Task.CompletedTask;
    }

    /// <summary>
    /// Handles a client making a move in an existing game.
    /// </summary>
    /// <param name="payload">The payload of the message</param>
    /// <param name="clientId">The id of the sender</param>
    private async Task HandleMakeMove(JsonElement payload, Guid clientId)
    {
        var gameTurnPayload = _jsonParser.DeserializeJsonElement<GameTurnPayload>(payload);
        if (gameTurnPayload == null) return;
    
        if (!_games.TryGetValue(gameTurnPayload.GameId, out var game)) return;
        
        game.AppendMove(gameTurnPayload.LastMove);

        Guid opponentId;
        
        // if clientId is not WhitePlayerId, then it was the black players turn so, to inform white we need his id
        if (game.GetWhitePlayerId() != clientId)
        {
            opponentId = game.GetWhitePlayerId();
        }
        else if (game.GetBlackPlayerId() != clientId)
        {
            opponentId = game.GetBlackPlayerId();
        }
        else
        {
            GameLogger.Error("Client is not part of the game when making a move");
            return;
        }
        
        if (!_clients.TryGetValue(opponentId, out var opponent)) return;
    
        // send ack to sender
        var ackMessage = new WebSocketMessage
        {
            Type = MessageType.GameTurnAck,
            Payload = _jsonParser.SerializeToJsonElement(new GameTurnAckPayload()
            {
                GameId = gameTurnPayload.GameId
            })
        };
        
        // send ack to sender
        await _clients[clientId].SendAsync(ackMessage);
        
        // forward new game state to opponent
        var forwardMessage = new WebSocketMessage
        {
            Type = MessageType.GameTurn,
            Payload = payload 
        };
    
        await opponent.SendAsync(forwardMessage);
    }
    
    /// <summary>
    /// Handles the end of a game when a client sends a GameOver message. Saves the game result to the database,
    /// </summary>
    /// <param name="payload">The payload of the message</param>
    /// <param name="clientId">The id of the sender</param>
    private async Task HandleGameOver(JsonElement payload, Guid clientId)
    {
        var gameOverPayload = _jsonParser.DeserializeJsonElement<GameOverPayload>(payload);
        if (gameOverPayload == null) return;
        
        if (!_games.TryGetValue(gameOverPayload.GameId, out var game)) return;

        var gameDto = new GameDto
        {
            Id = game.Id,
            WhitePlayerId = game.GetWhitePlayerId(),
            BlackPlayerId = game.GetBlackPlayerId(),
            Moves = game.GetMoveHistory()
        };
        
        await _gameService.InsertGameAsync(gameDto);
        
        var message = new WebSocketMessage
        {
            Type = MessageType.GameOver,
            Payload = payload
        };
    
        if (_clients.TryGetValue(game.GetWhitePlayerId(), out var white))
            await white.SendAsync(message);
    
        if (_clients.TryGetValue(game.GetBlackPlayerId(), out var black))
            await black.SendAsync(message);
    
        // remove game from active games
        _games.TryRemove(gameOverPayload.GameId, out _);
    }
    
}