using chess_client.Services;
using chess_client.States;
using Shared.Logger;

namespace chess_client.Menus;

/// <summary>
/// Manages the main menu of the game
/// </summary>
public class GameMenu
{
    private readonly UserContainer _userContainer;
    private readonly FriendshipMenu _friendshipMenu;
    private readonly WebSocketService _webSocketService;
    
    /// <summary>
    /// Initializes a new instance of the GameMenu class
    /// </summary>
    /// <param name="userContainer">The user container</param>
    /// <param name="friendshipMenu">The friendship menu</param>
    /// <param name="webSocketService">The WebSocket service</param>
    public GameMenu(UserContainer userContainer, FriendshipMenu friendshipMenu, WebSocketService webSocketService)
    {
        _userContainer = userContainer;
        _friendshipMenu = friendshipMenu;
        _webSocketService = webSocketService;
    }
    
    /// <summary>
    /// Displays the main menu and handles user input
    /// </summary>
    public async Task DisplayMainMenu()
    {
        // Beim Eintritt ins Hauptmenü den passenden State aktivieren
        _webSocketService.TransitionTo(new MainMenuState());

        while (true)
        {
            GameLogger.Info("Displaying main menu.");

            CliOutput.ClearTerminal();
            CliOutput.PrintConsoleNewline(ConsoleHelper.GameMenu);
            CliOutput.PrintConsoleNewline("Please enter your choice: ");
            var input = Console.ReadLine()?.Trim().ToUpper();

            GameLogger.Debug($"User entered menu input: '{input}'");

            switch (input)
            {
                case "P":
                case "PLAY":
                    // TODO: Implement real gameplay logic and transition to gameplay state
                    GameLogger.Info("User selected 'Play'.");
                    var gameLogic = new GameLogic();
                    await gameLogic.StartGame(_webSocketService);
                    _webSocketService.TransitionTo(new MainMenuState());
                    break;
                case "F":
                case "FRIENDS":
                    GameLogger.Info("User selected 'Friends'.");
                    await _friendshipMenu.DisplayMenu();
                    continue;
                case "Q":
                case "QUIT":
                    GameLogger.Info("User selected 'Quit'.");
                    return;
                default:
                    GameLogger.Warning($"Invalid menu input: '{input}'");
                    CliOutput.PrintConsoleNewline("Invalid input. Please try again.");
                    continue;
            }
        }
    }
}