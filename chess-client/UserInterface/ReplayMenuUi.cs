namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation and console interactions for the replay menu.
/// </summary>
public class ReplayMenuUi : BaseMenuUi
{
    private const string Header = "   === REPLAY MENU ===";
    private const string NoGamesMessage = "   No games found.";
    private const string SelectInstruction = "   Select a game to replay:";
    private const string QuitInstruction = "   [Q] Return to main menu";

    /// <summary>
    /// Clears the screen and draws the replay menu with selectable game entries.
    /// </summary>
    /// <param name="gameDisplays">Formatted game labels shown as numbered options.</param>
    /// <param name="errorMessage">Optional error message shown above the input prompt.</param>
    public void DrawMenu(IReadOnlyList<string> gameDisplays, string? errorMessage = null)
    {
        ConsoleHelper.ClearTerminal();
        DrawSectionHeader(Header);

        if (gameDisplays.Count == 0)
        {
            ConsoleHelper.PrintConsoleNewline(NoGamesMessage);
        }
        else
        {
            ConsoleHelper.PrintConsoleNewline(SelectInstruction);
            ConsoleHelper.WriteEmptyLine();
            DrawIndexedList(gameDisplays);
        }

        ConsoleHelper.WriteEmptyLine();
        DrawOptionalError(errorMessage);

        ConsoleHelper.PrintConsoleNewline(QuitInstruction);
        ConsoleHelper.WriteEmptyLine();
        DrawInputPrompt();
    }
}