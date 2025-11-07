using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;
using System.Threading.Channels;
using chess_server.Services;
using Shared;
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
    private readonly IGameService _gameService;
    private readonly ConcurrentDictionary<Guid, WebSocketClient> _clients = new();
    private readonly ConcurrentDictionary<Guid, ActiveGame> _games = new();
    private readonly JsonParser _jsonParser = new();
    private readonly Channel<Notification> _notificationChannel = Channel.CreateUnbounded<Notification>();

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
        GameLogger.Info($"Registered client {client.Id}");
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
    /// Dispatches the received message to the appropriate service based on the message type.
    /// </summary>
    /// <param name="messageType">The type of the message to dispatch.</param>
    /// <param name="payload">The payload of the message as a JSON element.</param>
    /// <param name="clientId">The id of the client who send the message</param>
    /// <returns>A task that represents the asynchronous dispatch operation.</returns>
    private async Task DispatchToService(string messageType, JsonElement payload, Guid clientId)
    {
        switch (messageType)
        {
            case MessageType.CreateGame:
                await HandleCreateGame(payload, clientId);
                break;
            case MessageType.JoinGame:
                await HandleJoinGame(payload, clientId);
                break;
            
        }
        
        await Task.CompletedTask;
    }

    private async Task HandleCreateGame(JsonElement payload, Guid clientId)
    {
        var createGamePayload = _jsonParser.DeserializeJsonElement<CreateGamePayload>(payload);
        if (createGamePayload == null)
        {
            GameLogger.Error("Failed to deserialize CreateGamePayload");
            return;
        }
        
        var game = _gameService.CreateGame(clientId);
        _games.TryAdd(game.Id, game);
        GameLogger.Info($"Created new game {game.Id} by client {clientId}");
        
        _clients.TryGetValue(clientId, out var client);
        _clients.TryGetValue(createGamePayload.OpponentId, out var opp);
        
        var gameCreatedMessage = new WebSocketMessage
        {
            Type = MessageType.GameCreated
        };

        var inviteMessage= new WebSocketMessage
        {
            Type = MessageType.GameInvitation,
            Payload = _jsonParser.SerializeToJsonElement(new GameInvitationPayload
            {
                GameId = game.Id,
                InviterId = clientId
            })
        };
        
        if (client == null || opp == null)
        {
            GameLogger.Error("One of the clients is null when sending GameCreated message");
            return;
        }
        
        await client.SendAsync(gameCreatedMessage);
        await opp.SendAsync(inviteMessage);
    }
    
    private async Task HandleJoinGame(JsonElement payload, Guid clientId)
    {
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
        
        lock (game) 
        {
            _gameService.JoinGame(game, clientId);
        }
        
        GameLogger.Info($"Client {clientId} joined game {joinGamePayload.GameId}");
        
        // inform the clients about game start, maybe make some public game method so other can easily spectate
        
        await Task.CompletedTask;
    }

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
}