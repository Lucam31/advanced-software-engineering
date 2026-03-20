using Shared;
using Shared.Logger;
using Shared.WebSocketMessages;

namespace chess_client.Services;

/// <summary>
/// Defines game-related actions sent by the client over WebSocket.
/// </summary>
public interface IGameService
{
    /// <summary>
    /// Sends a request to create a game against a specific opponent.
    /// </summary>
    /// <param name="opponentId">The ID of the opponent to challenge.</param>
    /// <returns>A task that completes when the request is sent.</returns>
    Task CreateGame(Guid opponentId);

    /// <summary>
    /// Accepts an incoming game invitation.
    /// </summary>
    /// <param name="gameId">The ID of the game invitation to accept.</param>
    /// <returns>A task that completes when the request is sent.</returns>
    Task AcceptGameInvitation(Guid gameId);

    /// <summary>
    /// Requests matchmaking for a new game.
    /// </summary>
    /// <returns>A task that completes when the matchmaking request is sent.</returns>
    Task SearchGame();
}

/// <summary>
/// Implements game actions by sending WebSocket messages to the server.
/// </summary>
public class GameService : IGameService
{
    private readonly UserContainer _userContainer;
    private readonly WebSocketService _webSocketService;
    private readonly JsonParser _jsonParser = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="GameService"/> class.
    /// </summary>
    /// <param name="userContainer">Holds user-specific client state.</param>
    /// <param name="webSocketService">Sends serialized WebSocket messages to the server.</param>
    public GameService(UserContainer userContainer, WebSocketService webSocketService)
    {
        _userContainer = userContainer;
        _webSocketService = webSocketService;
    }

    /// <inheritdoc/>
    public Task CreateGame(Guid opponentId)
    {
        GameLogger.Debug("Creating game...");
        var createGameRequest = new CreateGamePayload
        {
            OpponentId = opponentId,
        };

        var websocketMessage = new WebSocketMessage
        {
            Type = MessageType.CreateGame,
            Payload = _jsonParser.SerializeToJsonElement(createGameRequest)
        };

        return _webSocketService.SendAsync(websocketMessage);
    }

    /// <inheritdoc/>
    public Task AcceptGameInvitation(Guid gameId)
    {
        GameLogger.Debug("Accepting game invitation...");
        var acceptGameRequest = new JoinGamePayload()
        {
            GameId = gameId,
        };

        var websocketMessage = new WebSocketMessage
        {
            Type = MessageType.JoinGame,
            Payload = _jsonParser.SerializeToJsonElement(acceptGameRequest)
        };

        return _webSocketService.SendAsync(websocketMessage);
    }

    /// <inheritdoc/>
    public Task SearchGame()
    {
        GameLogger.Debug("Searching for games...");
        var websocketMessage = new WebSocketMessage
        {
            Type = MessageType.SearchGame,
            Payload = null
        };

        return _webSocketService.SendAsync(websocketMessage);
    }
}