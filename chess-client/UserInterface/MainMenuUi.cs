namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation of the initial startup menu.
/// </summary>
public class MainMenuUi
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

    /// <summary>
    /// Reads a single key press without echoing it to the console.
    /// </summary>
    /// <returns>The pressed key information.</returns>
    public static ConsoleKeyInfo ReadKey()
    {
        return Console.ReadKey(true);
    }
}