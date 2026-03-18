namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation of the initial startup menu.
/// </summary>
public class MainMenuUi : BaseMenuUi
{
    /// <summary>
    /// Draws the startup menu and optionally shows an error message.
    /// </summary>
    /// <param name="errorMessage">Optional error message shown above the input prompt.</param>
    public static void DrawMenu(string? errorMessage = null)
    {
        ConsoleHelper.ClearTerminal();
        ConsoleHelper.PrintConsoleNewline("       === WELCOME TO CHESS ===");
        Console.WriteLine();
        ConsoleHelper.PrintConsoleNewline("   ┌──────────────────────────────┐");
        ConsoleHelper.PrintConsoleNewline("   │          MAIN MENU           │");
        ConsoleHelper.PrintConsoleNewline("   ├──────────────────────────────┤");
        ConsoleHelper.PrintConsoleNewline("   │  [A] Authenticate            │");
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