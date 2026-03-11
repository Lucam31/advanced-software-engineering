using chess_client.Services;
using chess_client.States;
using Shared;
using Shared.Logger;
using Shared.WebSocketMessages;

namespace chess_client.Menus;

/// <summary>
/// Manages the main menu of the game
/// </summary>
public class GameMenu
{
    private readonly UserContainer _userContainer;
    private readonly FriendshipMenu _friendshipMenu;
    private readonly WebSocketService _webSocketService;
    private readonly IGameService _gameService;
    private volatile bool _refreshRequested = false;
    
    /// <summary>
    /// Initializes a new instance of the GameMenu class
    /// </summary>
    /// <param name="userContainer">The user container</param>
    /// <param name="friendshipMenu">The friendship menu</param>
    /// <param name="gameService">The game service</param>
    /// <param name="webSocketService">The WebSocket service</param>
    public GameMenu(UserContainer userContainer, FriendshipMenu friendshipMenu, IGameService gameService, WebSocketService webSocketService)
    {
        _userContainer = userContainer;
        _friendshipMenu = friendshipMenu;
        _gameService = gameService;
        _webSocketService = webSocketService;
    }
    
    /// <summary>
    /// Displays the main menu and handles user input
    /// </summary>
    public async Task DisplayMainMenu()
    {
        while (true)
        {
            var state = new GameMenuState();
            using var cts = new CancellationTokenSource();

            GameInvitationPayload? pendingInvitation = null;
            state.OnGameInvitation += payload =>
            {
                GameLogger.Info("Invitation-Refresh with opponentId " + payload.InviterId);
                pendingInvitation = payload;
                cts.Cancel();
            };
            _webSocketService.TransitionTo(state);
            
            StartGamePayload? pendingStartGame = null;
            state.OnStartGame += payload =>
            {
                GameLogger.Info("Game Start as color " + payload.Color);
                pendingStartGame = payload;
                cts.Cancel();
            };
            
            GameLogger.Info("Displaying main menu.");

            CliOutput.ClearTerminal();
            CliOutput.PrintConsoleNewline(ConsoleHelper.GameMenu);
            CliOutput.PrintConsoleNewline("Please enter your choice or press Enter to refresh: ");
            
            string? input = null;
            try
            {
                input = (await ConsoleHelper.ReadLineAsync(cts.Token))?.ToUpper();
            }
            catch (OperationCanceledException)
            {
                GameLogger.Info("Received game invitation.");
                await _gameService.AcceptGameInvitation(pendingInvitation.GameId);
                
                // wait for start game message
                while (true)
                {
                    if (pendingStartGame != null)
                    {
                        GameLogger.Info("Starting game with ID " + pendingStartGame.GameId);
                        var game = new GameLogic();
                        await game.StartGame(_webSocketService, pendingStartGame);
                        break;
                    }
                }
                continue;
            }
            GameLogger.Debug($"User entered menu input: '{input}'");
            
            switch (input)
            {
                case "P":
                case "PLAY":
                    CliOutput.PrintConsoleNewline("Entering matchmaking queue...");
                    await _gameService.SearchGame();
                    
                    while (true)
                    {
                        CliOutput.PrintConsoleNewline("Press Q to quit matchmaking queue.");
                        input = null;
                        try
                        {
                            input = (await ConsoleHelper.ReadLineAsync(cts.Token))?.Trim().ToUpper();
                            if (input == "Q")
                            {
                                var cancelMessage = new WebSocketMessage
                                {
                                    Type = MessageType.CancelSearch,
                                    Payload = null
                                };
                                await _webSocketService.SendAsync(cancelMessage);
                                break;
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            if (pendingStartGame != null)
                            {
                                GameLogger.Info("Starting game with ID " + pendingStartGame.GameId);
                                var game = new GameLogic();
                                await game.StartGame(_webSocketService, pendingStartGame);
                                break;
                            }
                        }
                    }
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