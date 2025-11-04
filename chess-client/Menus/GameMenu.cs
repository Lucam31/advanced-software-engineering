using chess_client.Services;
using Shared.Logger;

namespace chess_client.Menus;

/// <summary>
/// Manages the main menu of the game.
/// </summary>
public class GameMenu
{
    private readonly UserContainer _userContainer;
    
    /// <summary>
    /// Initializes a new instance of the GameMenu class.
    /// </summary>
    /// <param name="userContainer">The user container.</param>
    public GameMenu(UserContainer userContainer)
    {
        _userContainer = userContainer;
    }
    
    /// <summary>
    /// Displays the main menu and handles user input.
    /// </summary>
    public void DisplayMainMenu()
    {
        while (true)
        {
            GameLogger.Info("Displaying main menu.");

            CliOutput.PrintConsoleNewline(ConsoleHelper.GameMenu);
            CliOutput.PrintConsoleNewline("Please enter your choice: ");
            var input = Console.ReadLine()?.Trim().ToUpper();

            GameLogger.Debug($"User entered menu input: '{input}'");

            switch (input)
            {
                case "S":
                case "SEARCH":
                    GameLogger.Info("User selected 'Search'.");
                    SearchView();
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

    /// <summary>
    /// Handles the search view.
    /// </summary>
    private void SearchView()
    {
        CliOutput.PrintConsoleNewline(_userContainer.Id.ToString());
    }
}