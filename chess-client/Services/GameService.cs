
using Shared;
using Shared.WebSocketMessages;

namespace chess_client.Services;

public interface IGameService
{
    Task CreateGame(Guid opponentId);
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
        var createGameRequest = new CreateGamePayload
        {
            OpponentId = opponentId,
        };
        
        var websocketMessage = new WebSocketMessage
        {
            Type = "CreateGame",
            Payload = _jsonParser.SerializeToJsonElement(createGameRequest)
        };
        
        return _webSocketService.SendAsync(websocketMessage);
    }
}