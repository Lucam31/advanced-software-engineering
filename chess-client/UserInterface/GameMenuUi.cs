namespace chess_client.UserInterface;

/// <summary>
/// Renders the dashboard menu and handles related console input/output interactions.
/// </summary>
public class GameMenuUi : BaseMenuUi
{
    private const string PromptChoice = "   Your choice: ";

    private const string MainMenuLayout =
        """
               === CHESS DASHBOARD ===

           ┌──────────────────────────────┐
           │          MAIN MENU           │
           ├──────────────────────────────┤
           │  [P] Play                    │
           │  [F] Friends                 │
           │  [G] Games and Replays       │
           │  [L] Logout                  │
           │  [Q] Quit Game               │
           └──────────────────────────────┘
        """;

    /// <summary>
    /// Clears the screen and draws the dashboard menu.
    /// </summary>
    /// <param name="errorMessage">Optional error message shown above the input prompt.</param>
    public static void DrawMainMenu(string? errorMessage = null)
    {
        ConsoleHelper.ClearTerminal();
        ConsoleHelper.PrintConsoleNewline(MainMenuLayout);
        ConsoleHelper.WriteEmptyLine();

        DrawOptionalError(errorMessage);
        ConsoleHelper.PrintConsoleNewline(PromptChoice);
    }
}