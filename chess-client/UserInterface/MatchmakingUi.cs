using Shared;

namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation of the matchmaking queue.
/// </summary>
public class MatchmakingUi
{
    /// <summary>
    /// Clears the screen and draws the matchmaking queue screen.
    /// </summary>
    public void DrawQueueScreen(string? errorMessage = null)
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
    public void ShowMessage(string message)
    {
        Console.WriteLine();
        CliOutput.PrintConsoleNewline($"   {message}");
    }

    /// <summary>
    /// Reads user input asynchronously.
    /// </summary>
    public async Task<string?> ReadInputAsync(CancellationToken token)
    {
        return await ConsoleHelper.ReadLineAsync(token);
    }
}