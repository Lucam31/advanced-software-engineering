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
    private const int DisplayLimit = 5;
    private const string InvalidSelectionMessage = "Invalid selection. That game is not available.";
    private const string InvalidInputMessage = "Invalid input. Please use numbers 1-5 or Q.";

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

            var gameStrings = BuildGameDisplayStrings(games);

            _ui.DrawMenu(gameStrings, currentErrorMessage);
            currentErrorMessage = null;

            var input = BaseMenuUi.ReadKey();

            if (input.Key == ConsoleKey.Q)
            {
                GameLogger.Info("User selected 'Quit' in replay menu.");
                return;
            }

            if (TryGetSelectedIndex(input.Key, out var selectedIndex))
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
                    currentErrorMessage = InvalidSelectionMessage;
                }
            }
            else
            {
                GameLogger.Warning($"Invalid menu input: '{input.Key}'");
                currentErrorMessage = InvalidInputMessage;
            }
        }
    }

    /// <summary>
    /// Creates display labels for replayable games and limits output to the configured maximum.
    /// </summary>
    /// <param name="games">The full list of recent games.</param>
    /// <returns>Formatted labels shown in the replay menu.</returns>
    private List<string> BuildGameDisplayStrings(IReadOnlyList<Shared.Dtos.PlayedGame> games)
    {
        var visibleCount = Math.Min(DisplayLimit, games.Count);
        return games.Take(visibleCount)
            .Select(g => $"{g.WhitePlayerUsername} vs {g.BlackPlayerUsername}")
            .ToList();
    }

    /// <summary>
    /// Maps numeric key input to a zero-based replay selection index.
    /// </summary>
    /// <param name="key">The pressed menu key.</param>
    /// <param name="selectedIndex">The resolved zero-based index when parsing succeeds.</param>
    /// <returns><c>true</c> when a supported numeric selection key was pressed.</returns>
    private bool TryGetSelectedIndex(ConsoleKey key, out int selectedIndex)
    {
        selectedIndex = key switch
        {
            ConsoleKey.D1 or ConsoleKey.NumPad1 => 0,
            ConsoleKey.D2 or ConsoleKey.NumPad2 => 1,
            ConsoleKey.D3 or ConsoleKey.NumPad3 => 2,
            ConsoleKey.D4 or ConsoleKey.NumPad4 => 3,
            ConsoleKey.D5 or ConsoleKey.NumPad5 => 4,
            _ => -1
        };

        return selectedIndex >= 0;
    }
}