using chess_client.Logic;
using chess_client.Services;
using chess_client.States;
using Shared.Logger;
using chess_client.UserInterface;

namespace chess_client.Menus;

/// <summary>
/// Displays past games and lets the user open a replay.
/// </summary>
public class ReplayMenu
{
    private readonly UserContainer _userContainer;
    private readonly WebSocketService _webSocketService;
    private readonly ReplayService _replayService;
    private readonly ReplayMenuUi _ui = new();

    /// <summary>
    /// Initializes a replay menu for the currently authenticated user.
    /// </summary>
    /// <param name="userContainer">Shared user state containing the active user id.</param>
    /// <param name="webSocketService">WebSocket connection used to transition into replay menu state.</param>
    public ReplayMenu(UserContainer userContainer, WebSocketService webSocketService)
    {
        _userContainer = userContainer;
        _webSocketService = webSocketService;
        _replayService = new ReplayService();
    }

    /// <summary>
    /// Displays recent games and handles selection input until the user exits the replay menu.
    /// </summary>
    public async Task DisplayMenu()
    {
        _webSocketService.TransitionTo(new ReplayMenuState());
        string? currentErrorMessage = null;

        while (true)
        {
            GameLogger.Info("Displaying replay menu.");

            var games = await _replayService.GetGames(_userContainer.Id);

            // show max 5 games
            var displayLimit = Math.Min(5, games.Count);

            var gameStrings = games.Take(displayLimit)
                .Select(g => $"{g.WhitePlayerUsername} vs {g.BlackPlayerUsername}")
                .ToList();

            ReplayMenuUi.DrawMenu(gameStrings, currentErrorMessage);
            currentErrorMessage = null;

            var input = ReplayMenuUi.ReadKey();

            if (input.Key == ConsoleKey.Q)
            {
                GameLogger.Info("User selected 'Quit' in replay menu.");
                return;
            }

            var selectedIndex = -1;
            switch (input.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1: selectedIndex = 0; break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2: selectedIndex = 1; break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3: selectedIndex = 2; break;
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4: selectedIndex = 3; break;
                case ConsoleKey.D5:
                case ConsoleKey.NumPad5: selectedIndex = 4; break;
            }

            if (selectedIndex >= 0)
            {
                if (selectedIndex < games.Count)
                {
                    GameLogger.Info($"User selected game {selectedIndex + 1} for replay.");
                    var replayLogic = new ReplayLogic(games[selectedIndex]);
                    replayLogic.StartReplay();
                }
                else
                {
                    GameLogger.Warning($"User selected game {selectedIndex + 1} for replay, but it is not available.");
                    currentErrorMessage = "Invalid selection. That game is not available.";
                }
            }
            else
            {
                GameLogger.Warning($"Invalid menu input: '{input.Key}'");
                currentErrorMessage = "Invalid input. Please use numbers 1-5 or Q.";
            }
        }
    }
}