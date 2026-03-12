using Shared;

namespace chess_client;

/// <summary>
/// Renders the command-line chess game screen.
///
/// The UI consists of three aligned areas: the board on the left,
/// match/status information in the middle, and captured pieces plus move history on the right.
/// Rendering supports both White and Black perspectives.
/// </summary>
public static class GameUi
{
    private const int BoardWidth = 36;
    private const int MiddleColumnWidth = 38;

    /// <summary>
    /// Clears the console and draws one full game frame.
    ///
    /// The frame combines board rows with two side panels and keeps all columns aligned by
    /// iterating over the largest panel height.
    /// </summary>
    /// <param name="board">Current board state, including active and captured pieces.</param>
    /// <param name="stats">Presentation data such as player names, status text, and move history.</param>
    /// <param name="isWhitePerspective">
    /// <see langword="true"/> to render from White's viewpoint; otherwise render from Black's viewpoint.
    /// </param>
    public static void DrawGameScreen(Gameboard board, GameStats stats, bool isWhitePerspective)
    {
        Console.Clear();
        Console.WriteLine();

        var middleLines = GenerateMiddleColumnLines(stats);
        var rightLines = GenerateRightColumnLines(board, stats);

        var maxLines = Math.Max(10, Math.Max(middleLines.Count, rightLines.Count));

        for (var i = 0; i < maxLines; i++)
        {
            PrintBoardLine(i, board, isWhitePerspective);

            var middleLine = i < middleLines.Count ? middleLines[i] : "";
            Console.Write(middleLine.PadRight(MiddleColumnWidth));

            var rightLine = i < rightLines.Count ? rightLines[i] : "";
            Console.WriteLine(rightLine);
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Prints a single line of the board column.
    ///
    /// Index <c>0</c> and <c>9</c> are the file labels, <c>1</c>-<c>8</c> are board ranks,
    /// and other indices print blank padding to keep side panels aligned.
    /// </summary>
    /// <param name="lineIndex">Line index in the board area (expected range: 0-9).</param>
    /// <param name="board">Board state used to resolve pieces for rank lines.</param>
    /// <param name="isWhitePerspective">Whether rank/file order is shown from White's side.</param>
    private static void PrintBoardLine(int lineIndex, Gameboard board, bool isWhitePerspective)
    {
        switch (lineIndex)
        {
            case 0 or 9:
            {
                const string topLetters = "    A  B  C  D  E  F  G  H";
                Console.Write(topLetters.PadRight(BoardWidth));
                break;
            }
            case >= 1 and <= 8:
            {
                var rank = isWhitePerspective ? 8 - (lineIndex - 1) : (lineIndex - 1) + 1;
                Console.Write($" {rank} ");

                for (var j = 0; j < 8; j++)
                {
                    var file = isWhitePerspective ? j : 7 - j;
                    var isDark = (file + rank) % 2 == 0;

                    Console.BackgroundColor = isDark ? ConsoleColor.Gray : ConsoleColor.DarkGray;

                    var tile = board[rank - 1, file];
                    if (tile.CurrentPiece != null)
                    {
                        Console.ForegroundColor = tile.CurrentPiece.IsWhite ? ConsoleColor.White : ConsoleColor.Black;
                        Console.Write($" {tile.CurrentPiece.UnicodeSymbol} ");
                    }
                    else
                    {
                        Console.Write("   ");
                    }
                }

                Console.ResetColor();
                Console.Write($" {rank}".PadRight(BoardWidth - 27));
                break;
            }
            default:
            {
                Console.Write(new string(' ', BoardWidth));
                break;
            }
        }
    }

    /// <summary>
    /// Creates the middle panel lines (match information and current status).
    /// </summary>
    /// <param name="stats">Game metadata used for player labels and the status message.</param>
    /// <returns>Display lines in print order for the middle column.</returns>
    private static List<string> GenerateMiddleColumnLines(GameStats stats)
    {
        return
        [
            "│  MATCH INFO",
            "│  ───────────────────────",
            $"│  White: {stats.WhitePlayerName}",
            $"│  Black: {stats.BlackPlayerName}",
            "│",
            "│  STATUS",
            "│  ───────────────────────",
            $"│  {stats.StatusMessage}"
        ];
    }

    /// <summary>
    /// Creates the right panel lines (captured pieces and recent moves for each side).
    /// </summary>
    /// <param name="board">Board state used to read captured piece collections.</param>
    /// <param name="stats">Game statistics that provide move history for both players.</param>
    /// <returns>
    /// Display lines in print order for the right column. Empty captured/move lists are shown as <c>None</c>.
    /// </returns>
    private static List<string> GenerateRightColumnLines(Gameboard board, GameStats stats)
    {
        var whiteCaptured = string.Join(", ", board.CapturedWhitePieces.Select(p => p.UnicodeSymbol));
        var blackCaptured = string.Join(", ", board.CapturedBlackPieces.Select(p => p.UnicodeSymbol));

        if (string.IsNullOrEmpty(whiteCaptured)) whiteCaptured = "None";
        if (string.IsNullOrEmpty(blackCaptured)) blackCaptured = "None";

        var whiteMoves = stats.WhiteMoves.Count > 0 ? string.Join(", ", stats.WhiteMoves) : "None";
        var blackMoves = stats.BlackMoves.Count > 0 ? string.Join(", ", stats.BlackMoves) : "None";

        return
        [
            "│  CAPTURED PIECES",
            "│  ───────────────────────",
            $"│  White: {whiteCaptured}",
            $"│  Black: {blackCaptured}",
            "│",
            "│  LAST MOVES",
            "│  ───────────────────────",
            $"│  White: {whiteMoves}",
            $"│  Black: {blackMoves}"
        ];
    }
}