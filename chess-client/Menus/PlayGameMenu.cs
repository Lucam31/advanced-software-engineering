using chess_client.Services;

namespace chess_client.Menus;

public class PlayGameMenu
{
    private readonly UserContainer _userContainer;
    private readonly FriendshipMenu _friendshipMenu;
    private readonly WebSocketService _webSocketService;
    
    /// <summary>
    /// Initializes a new instance of the PlayGameMenu class
    /// </summary>
    /// <param name="userContainer">The user container</param>
    /// <param name="friendshipMenu">The friendship menu</param>
    /// <param name="webSocketService">The WebSocket service</param>
    public PlayGameMenu(UserContainer userContainer, FriendshipMenu friendshipMenu, WebSocketService webSocketService)
    {
        _userContainer = userContainer;
        _friendshipMenu = friendshipMenu;
        _webSocketService = webSocketService;
    }
}