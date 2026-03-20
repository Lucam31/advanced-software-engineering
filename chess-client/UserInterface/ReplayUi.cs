using Shared;

namespace chess_client.UserInterface;

/// <summary>
/// Renders the interactive replay screen and related status messages.
/// </summary>
public class ReplayUi : BaseMenuUi
{
    private const string ReplayHeader = "     === GAME REPLAY ===";

    // Raw String Literal für einen sauberen, leicht anpassbaren Kasten
    private const string ReplayControls =
        """
           ┌──────────────────────┐
           │  [A] Previous Move   │
           │  [D] Next Move       │
           │  [Q] Quit Replay     │
           └──────────────────────┘
        """;

    /// <summary>
    /// Clears the screen and draws the complete replay view including board, counter, and controls.
    /// </summary>
    /// <param name="board">The board state to render.</param>
    /// <param name="currentMoveIndex">The current replay move index.</param>
    /// <param name="totalMoves">The total number of moves available in the replay.</param>
    /// <param name="statusMessage">Optional status or error message to display under the controls.</param>
    /// <param name="isError">If true, the message is drawn in red with a warning icon.</param>
    public void DrawScreen(Gameboard board, int currentMoveIndex, int totalMoves, string? statusMessage = null,
        bool isError = false)
    {
        ConsoleHelper.ClearTerminal();

        ConsoleHelper.PrintConsoleNewline(ReplayHeader);
        ConsoleHelper.WriteEmptyLine();

        ConsoleHelper.PrintConsoleNewline(ReplayControls);
        ConsoleHelper.WriteEmptyLine();

        board.PrintBoard();
        ConsoleHelper.WriteEmptyLine();

        ConsoleHelper.PrintConsoleNewline($"   Move {currentMoveIndex}/{totalMoves}");
        ConsoleHelper.PrintConsoleNewline("   ────────────────────────────────");

        ConsoleHelper.WriteEmptyLine();
        var statusRow = Console.CursorTop;

        if (isError && !string.IsNullOrWhiteSpace(statusMessage))
        {
            Console.SetCursorPosition(0, statusRow);
            ConsoleHelper.SetForegroundColor(ConsoleColor.Red);
            ConsoleHelper.PrintConsole($"   ⚠ {statusMessage}");
            ConsoleHelper.ResetColor();
        }

        // Console.SetCursorPosition(0, statusRow + 1);

        ConsoleHelper.PrintConsole("   Command: ");
    }
}