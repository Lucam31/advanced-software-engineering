namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation of the initial startup menu.
/// </summary>
public class MainMenuUi : BaseMenuUi
{
    private const string PromptChoice = "   Your choice: ";

    private const string MenuLayout =
        """
               === WELCOME TO CHESS ===

           ┌──────────────────────────────┐
           │          MAIN MENU           │
           ├──────────────────────────────┤
           │  [A] Authenticate            │
           │  [Q] Quit Game               │
           └──────────────────────────────┘
        """;

    /// <summary>
    /// Draws the startup menu and optionally shows an error message.
    /// </summary>
    /// <param name="errorMessage">Optional error message shown above the input prompt.</param>
    public static void DrawMenu(string? errorMessage = null)
    {
        ConsoleHelper.ClearTerminal();
        ConsoleHelper.PrintConsoleNewline(MenuLayout);
        ConsoleHelper.WriteEmptyLine();

        BaseMenuUi.DrawOptionalError(errorMessage);
        ConsoleHelper.PrintConsoleNewline(PromptChoice);
    }
}