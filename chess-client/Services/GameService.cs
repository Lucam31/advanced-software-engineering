
using System.Text.Json;
using Shared;
using Shared.Logger;
using Shared.WebSocketMessages;

namespace chess_client.Services;

public interface IGameService
{
    Task CreateGame(Guid opponentId);
    Task AcceptGameInvitation(Guid gameId);
    Task SearchGame();
}

public class GameService : IGameService
{
    private readonly UserContainer _userContainer;
    private readonly WebSocketService _webSocketService;
    private readonly JsonParser _jsonParser = new();

    /// <summary>
    /// Initializes a new instance of the GameService class.
    /// </summary>
    /// <param name="userContainer">The user container </param>
    /// <param name="webSocketService"></param>
    public GameService(UserContainer userContainer, WebSocketService webSocketService)
    {
        _userContainer = userContainer;
        _webSocketService = webSocketService;
    }

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