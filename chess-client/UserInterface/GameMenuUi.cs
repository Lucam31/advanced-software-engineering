namespace chess_client.UserInterface;

/// <summary>
/// Renders the dashboard menu and handles related console input/output interactions.
/// </summary>
public class GameMenuUi : BaseMenuUi
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
}