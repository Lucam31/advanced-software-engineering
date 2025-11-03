using Shared.Logger;

namespace chess_client;

/// <summary>
/// Manages the main menu of the game.
/// </summary>
public class GameMenu
{
    /// <summary>
    /// Displays the main menu and handles user input for starting or quitting the game.
    /// </summary>
    /// <returns>True if the user chooses to play, false if they choose to quit.</returns>
    public static bool DisplayMainMenu()
    {
        while (true)
        {
            GameLogger.Info("Displaying main menu.");

            CliOutput.PrintConsoleNewline(ConsoleHelper.Menu);
            CliOutput.PrintConsoleNewline("Please enter your choice: ");
            var input = Console.ReadLine()?.Trim().ToUpper();

            GameLogger.Debug($"User entered menu input: '{input}'");

            switch (input)
            {
                case "P":
                case "PLAY":
                    GameLogger.Info("User selected 'Play'.");
                    return true;
                case "Q":
                case "QUIT":
                    GameLogger.Info("User selected 'Quit'.");
                    return false;
                default:
                    GameLogger.Warning($"Invalid menu input: '{input}'");
                    CliOutput.PrintConsoleNewline("Invalid input. Please try again.");
                    continue;
            }
        }
    }
}