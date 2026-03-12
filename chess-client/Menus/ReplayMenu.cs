using chess_client.Services;
using chess_client.States;
using Shared.Logger;

namespace chess_client.Menus;

/// <summary>
/// Manages the menu of past games and the replay function
/// </summary>
public class ReplayMenu
{
    private readonly UserContainer _userContainer;
    private readonly WebSocketService _webSocketService;
    private readonly ReplayService _replayService;
    
    /// <summary>
    /// Initializes a new instance of the GameMenu class
    /// </summary>
    /// <param name="userContainer">The user container</param>
    /// <param name="webSocketService">The WebSocket service</param>
    public ReplayMenu(UserContainer userContainer, WebSocketService webSocketService)
    {
        _userContainer = userContainer;
        _webSocketService = webSocketService;
        _replayService = new ReplayService();
    }
    
    /// <summary>
    /// Displays the replay menu and handles user input
    /// </summary>
    public async Task DisplayMenu()
    {
        Console.Clear();
        _webSocketService.TransitionTo(new ReplayMenuState());

        while (true)
        {
            GameLogger.Info("Displaying replay menu.");

            var games = await _replayService.GetGames(_userContainer.Id);
            if (games.Count == 0)
            {
                CliOutput.PrintConsoleNewline("No games found.");
                CliOutput.PrintConsoleNewline("Press Q to return to the main menu: ");
            }
            else
            {
                CliOutput.PrintConsoleNewline("Press the number of the game you want to replay.");
                for (int i = 0; i < (5 <= games.Count ? 5 : games.Count); i++)
                {
                    CliOutput.PrintConsoleNewline(
                        $"({i + 1}) {games[i].WhitePlayerUsername} vs {games[i].BlackPlayerUsername}"); // - Result: {games[i].Result}");
                }

                CliOutput.PrintConsoleNewline("(Q) return to the main menu.");
                CliOutput.PrintConsoleNewline("Enter your choice: ");
            }

            var input = Console.ReadKey();
            switch (input.Key)
            {
                case ConsoleKey.Q:
                    GameLogger.Info("User selected 'Quit' in replay menu.");
                    Console.Clear();
                    return;
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    if (games.Count >= 1)
                    {
                        GameLogger.Info("User selected game 1 for replay.");
                        var replayLogic = new ReplayLogic(games[0]);
                        replayLogic.StartReplay();
                    }
                    else
                    {
                        GameLogger.Warning("User selected game 1 for replay, but no games are available.");
                    }

                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    if (games.Count >= 2)
                    {
                        GameLogger.Info("User selected game 2 for replay.");
                        var replayLogic = new ReplayLogic(games[1]);
                        replayLogic.StartReplay();
                    }
                    else
                    {
                        GameLogger.Warning("User selected game 2 for replay, but no games are available.");
                    }

                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    if (games.Count >= 3)
                    {
                        GameLogger.Info("User selected game 3 for replay.");
                        var replayLogic = new ReplayLogic(games[2]);
                        replayLogic.StartReplay();
                    }
                    else
                    {
                        GameLogger.Warning("User selected game 3 for replay, but no games are available.");
                    }

                    break;
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    if (games.Count >= 4)
                    {
                        GameLogger.Info("User selected game 4 for replay.");
                        var replayLogic = new ReplayLogic(games[3]);
                        replayLogic.StartReplay();
                    }
                    else
                    {
                        GameLogger.Warning("User selected game 4 for replay, but no games are available.");
                    }

                    break;
                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    if (games.Count >= 5)
                    {
                        GameLogger.Info("User selected game 5 for replay.");
                        var replayLogic = new ReplayLogic(games[4]);
                        replayLogic.StartReplay();
                    }
                    else
                    {
                        GameLogger.Warning("User selected game 5 for replay, but no games are available.");
                    }

                    break;
            }
        }
    }
}