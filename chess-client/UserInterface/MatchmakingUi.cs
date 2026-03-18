namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation of the matchmaking queue.
/// </summary>
public class MatchmakingUi : BaseMenuUi
{
    /// <summary>
    /// Clears the screen and draws the matchmaking queue view.
    /// </summary>
    /// <param name="errorMessage">Optional error message shown above the queue input prompt.</param>
    public static void DrawQueueScreen(string? errorMessage = null)
    {
        ConsoleHelper.ClearTerminal();
        Console.WriteLine();
        ConsoleHelper.PrintConsoleNewline("   === MATCHMAKING ===");
        Console.WriteLine();
        ConsoleHelper.PrintConsoleNewline("   Searching for an opponent...");
        ConsoleHelper.PrintConsoleNewline("   Please wait.");
        Console.WriteLine();

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleHelper.PrintConsoleNewline($"   ⚠ {errorMessage}");
            Console.ResetColor();
            Console.WriteLine();
        }

        ConsoleHelper.PrintConsoleNewline("   Press [Q] to quit queue.");
        Console.WriteLine();
    }
}