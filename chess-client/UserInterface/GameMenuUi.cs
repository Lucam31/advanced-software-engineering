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
        CliOutput.ClearTerminal();
        Console.WriteLine();
        CliOutput.PrintConsoleNewline("       === CHESS DASHBOARD ===");
        Console.WriteLine();

        CliOutput.PrintConsoleNewline("   ┌──────────────────────────────┐");
        CliOutput.PrintConsoleNewline("   │          MAIN MENU           │");
        CliOutput.PrintConsoleNewline("   ├──────────────────────────────┤");
        CliOutput.PrintConsoleNewline("   │  [P] Play                    │");
        CliOutput.PrintConsoleNewline("   │  [F] Friends                 │");
        CliOutput.PrintConsoleNewline("   │  [G] Games and Replays       │");
        CliOutput.PrintConsoleNewline("   │  [L] Logout                  │");
        CliOutput.PrintConsoleNewline("   │  [Q] Quit Game               │");
        CliOutput.PrintConsoleNewline("   └──────────────────────────────┘");
        Console.WriteLine();

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CliOutput.PrintConsoleNewline($"   ⚠ {errorMessage}");
            Console.ResetColor();
            Console.WriteLine();
        }

        CliOutput.PrintConsoleNewline("   Your choice: ");
    }

    /// <summary>
    /// Displays a generic status message to the user.
    /// </summary>
    /// <param name="message">Message text to display.</param>
    public static void ShowMessage(string message)
    {
        Console.WriteLine();
        CliOutput.PrintConsoleNewline($"   ℹ {message}");
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