namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation of the initial startup menu.
/// </summary>
public class MainMenuUi
{
    public void DrawMenu(string? errorMessage = null)
    {
        CliOutput.ClearTerminal();
        CliOutput.PrintConsoleNewline("       === WELCOME TO CHESS ===");
        Console.WriteLine();
        CliOutput.PrintConsoleNewline("   ┌──────────────────────────────┐");
        CliOutput.PrintConsoleNewline("   │          MAIN MENU           │");
        CliOutput.PrintConsoleNewline("   ├──────────────────────────────┤");
        CliOutput.PrintConsoleNewline("   │  [A] Authenticate            │");
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

    public ConsoleKeyInfo ReadKey()
    {
        return Console.ReadKey(true);
    }
}