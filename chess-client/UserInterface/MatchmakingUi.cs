namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation of the matchmaking queue.
/// </summary>
public class MatchmakingUi : BaseMenuUi
{
    private const string Header = "   === MATCHMAKING ===";
    private const string SearchLine = "   Searching for an opponent...";
    private const string WaitLine = "   Please wait.";
    private const string QuitInstruction = "   Press [Q] to quit queue.";

    /// <summary>
    /// Clears the screen and draws the matchmaking queue view.
    /// </summary>
    /// <param name="errorMessage">Optional error message shown above the queue input prompt.</param>
    public static void DrawQueueScreen(string? errorMessage = null)
    {
        ConsoleHelper.ClearTerminal();
        DrawSectionHeader(Header);
        ConsoleHelper.PrintConsoleNewline(SearchLine);
        ConsoleHelper.PrintConsoleNewline(WaitLine);
        ConsoleHelper.WriteEmptyLine();

        DrawOptionalError(errorMessage);
        ConsoleHelper.PrintConsoleNewline(QuitInstruction);
        ConsoleHelper.WriteEmptyLine();
    }
}