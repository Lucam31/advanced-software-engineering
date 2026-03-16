namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation and console interactions for the main menu.
/// </summary>
public class GameMenuUi
{
    /// <summary>
    /// Clears the screen and draws the main menu, optionally displaying an error message.
    /// </summary>
    /// <param name="errorMessage">An optional error message to display in red.</param>
    public void DrawMainMenu(string? errorMessage = null)
    {
        CliOutput.ClearTerminal();
        CliOutput.PrintConsoleNewline(ConsoleHelper.GameMenu);
        
        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CliOutput.PrintConsoleNewline(errorMessage);
            Console.ResetColor();
            Console.WriteLine();
        }
        
        CliOutput.PrintConsoleNewline("Please enter your choice or press Enter to refresh: ");
    }

    /// <summary>
    /// Displays the state when the user enters the matchmaking queue.
    /// </summary>
    public void ShowMatchmakingState()
    {
        CliOutput.PrintConsoleNewline("Entering matchmaking queue...");
        CliOutput.PrintConsoleNewline("Press Q to quit matchmaking queue.");
    }

    /// <summary>
    /// Displays a generic status message to the user.
    /// </summary>
    public void ShowMessage(string message)
    {
        CliOutput.PrintConsoleNewline(message);
    }

    /// <summary>
    /// Reads user input asynchronously, supporting cancellation.
    /// </summary>
    public async Task<string?> ReadInputAsync(CancellationToken token)
    {
        return await ConsoleHelper.ReadLineAsync(token);
    }
}