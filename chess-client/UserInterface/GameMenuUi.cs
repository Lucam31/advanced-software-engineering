namespace chess_client.UserInterface;

/// <summary>
/// Renders the dashboard menu and handles related console input/output interactions.
/// </summary>
public class GameMenuUi
{
    /// <summary>
    /// Clears the screen and draws the dashboard menu.
    /// </summary>
    /// <param name="errorMessage">Optional error message shown above the input prompt.</param>
    public static void DrawMainMenu(string? errorMessage = null)
    {
        ConsoleHelper.ClearTerminal();
        Console.WriteLine();
        ConsoleHelper.PrintConsoleNewline("       === CHESS DASHBOARD ===");
        Console.WriteLine();

        ConsoleHelper.PrintConsoleNewline("   ┌──────────────────────────────┐");
        ConsoleHelper.PrintConsoleNewline("   │          MAIN MENU           │");
        ConsoleHelper.PrintConsoleNewline("   ├──────────────────────────────┤");
        ConsoleHelper.PrintConsoleNewline("   │  [P] Play                    │");
        ConsoleHelper.PrintConsoleNewline("   │  [F] Friends                 │");
        ConsoleHelper.PrintConsoleNewline("   │  [G] Games and Replays       │");
        ConsoleHelper.PrintConsoleNewline("   │  [L] Logout                  │");
        ConsoleHelper.PrintConsoleNewline("   │  [Q] Quit Game               │");
        ConsoleHelper.PrintConsoleNewline("   └──────────────────────────────┘");
        Console.WriteLine();

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleHelper.PrintConsoleNewline($"   ⚠ {errorMessage}");
            Console.ResetColor();
            Console.WriteLine();
        }

        ConsoleHelper.PrintConsoleNewline("   Your choice: ");
    }

    /// <summary>
    /// Displays a generic status message to the user.
    /// </summary>
    /// <param name="message">Message text to display.</param>
    public static void ShowMessage(string message)
    {
        Console.WriteLine();
        ConsoleHelper.PrintConsoleNewline($"   ℹ {message}");
    }

    /// <summary>
    /// Reads a key press asynchronously and supports cancellation for realtime background events.
    /// </summary>
    /// <param name="token">Cancellation token used to interrupt waiting for input.</param>
    /// <returns>The pressed key information.</returns>
    public async Task<ConsoleKeyInfo> ReadKeyAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (Console.KeyAvailable)
            {
                return Console.ReadKey(true);
            }

            await Task.Delay(20, token);
        }

        token.ThrowIfCancellationRequested();
        return default;
    }
}