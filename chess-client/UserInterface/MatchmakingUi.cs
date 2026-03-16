namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation of the matchmaking queue.
/// </summary>
public class MatchmakingUi
{
    /// <summary>
    /// Clears the screen and draws the matchmaking queue view.
    /// </summary>
    /// <param name="errorMessage">Optional error message shown above the queue input prompt.</param>
    public static void DrawQueueScreen(string? errorMessage = null)
    {
        CliOutput.ClearTerminal();
        Console.WriteLine();
        CliOutput.PrintConsoleNewline("   === MATCHMAKING ===");
        Console.WriteLine();
        CliOutput.PrintConsoleNewline("   Searching for an opponent...");
        CliOutput.PrintConsoleNewline("   Please wait.");
        Console.WriteLine();
        
        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CliOutput.PrintConsoleNewline($"   {errorMessage}");
            Console.ResetColor();
            Console.WriteLine();
        }

        CliOutput.PrintConsoleNewline("   [Q] Quit Queue");
        Console.WriteLine();
        CliOutput.PrintConsoleNewline("   Your input: ");
    }

    /// <summary>
    /// Displays a temporary message before transitioning away.
    /// </summary>
    /// <param name="message">Message text to display.</param>
    public static void ShowMessage(string message)
    {
        Console.WriteLine();
        CliOutput.PrintConsoleNewline($"   {message}");
    }

    /// <summary>
    /// Reads queue input asynchronously and supports cancellation.
    /// </summary>
    /// <param name="token">Cancellation token used to interrupt waiting for input.</param>
    /// <returns>The entered input, or <c>null</c> when no input is available.</returns>
    public static async Task<string?> ReadInputAsync(CancellationToken token)
    {
        return await ConsoleHelper.ReadLineAsync(token);
    }
}