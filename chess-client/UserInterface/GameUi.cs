using Shared;

namespace chess_client.UserInterface;

/// <summary>
/// Renders the terminal game screen and stores transient UI state such as status text and move history.
/// </summary>
public class GameUi : BaseMenuUi
{
    private const int BoardWidth = 36;
    private const int MiddleColumnWidth = 38;
    private const int BoardSectionMinLines = 10;
    private const int SeparatorWidth = 85;
    private const string FooterPadding = "   ";
    private const string DefaultWhitePlayerLabel = "Player 1 (White)";
    private const string DefaultBlackPlayerLabel = "Player 2 (Black)";
    private const string DefaultStatusMessage = "Game starting...";
    private const string EmptyValue = "";
    private const string NoneLabel = "None";
    private const string WhitePerspectiveFiles = "A  B  C  D  E  F  G  H";
    private const string BlackPerspectiveFiles = "H  G  F  E  D  C  B  A";
    private const string MatchInfoHeader = "│  MATCH INFO";
    private const string StatusHeader = "│  STATUS";
    private const string CapturedPiecesHeader = "│  CAPTURED PIECES";
    private const string LastMovesHeader = "│  LAST MOVES";
    private const string SectionSeparator = "│  ───────────────────────";
    private const string EmptySectionLine = "│";

    /// <summary>
    /// Gets or sets the label shown for the white side in the match info panel.
    /// </summary>
    public string WhitePlayerName { get; set; } = DefaultWhitePlayerLabel;

    /// <summary>
    /// Gets or sets the label shown for the black side in the match info panel.
    /// </summary>
    public string BlackPlayerName { get; set; } = DefaultBlackPlayerLabel;

    /// <summary>
    /// Gets or sets the current game status text shown in the middle information column.
    /// </summary>
    public string StatusMessage { get; set; } = DefaultStatusMessage;

    private List<string> WhiteMoves { get; } = [];
    private List<string> BlackMoves { get; } = [];

    /// <summary>
    /// Gets or sets the error message displayed under the board; an empty value hides the error line.
    /// </summary>
    public string ErrorMessage { get; set; } = EmptyValue;

    /// <summary>
    /// Gets or sets the input prompt shown at the bottom of the screen.
    /// </summary>
    public string PromptMessage { get; set; } = EmptyValue;

    /// <summary>
    /// Adds a move to the local move history that is rendered in the "Last Moves" panel.
    /// </summary>
    /// <param name="from">The source square in algebraic coordinates (for example, <c>e2</c>).</param>
    /// <param name="to">The destination square in algebraic coordinates (for example, <c>e4</c>).</param>
    /// <param name="isWhite"><c>true</c> to append to white's move list; otherwise appends to black's move list.</param>
    public void AddMoveToHistory(string from, string to, bool isWhite)
    {
        var move = $"{from}{to}";

        if (isWhite)
            WhiteMoves.Add(move);
        else
            BlackMoves.Add(move);
    }

    /// <summary>
    /// Draws the full game view, including the board, match/status panels, captured pieces, and move history.
    /// </summary>
    /// <param name="board">The current board state used for piece and capture rendering.</param>
    /// <param name="isWhitePerspective"><c>true</c> to render files/ranks from white's perspective; otherwise from black's perspective.</param>
    public void DrawGameScreen(Gameboard board, bool isWhitePerspective)
    {
        Console.Clear();
        Console.WriteLine();

        var middleLines = GenerateMiddleColumnLines();
        var rightLines = GenerateRightColumnLines(board);

        var maxLines = Math.Max(BoardSectionMinLines, Math.Max(middleLines.Count, rightLines.Count));

        for (var i = 0; i < maxLines; i++)
        {
            PrintBoardLine(i, board, isWhitePerspective);

            var middleLine = i < middleLines.Count ? middleLines[i] : "";
            Console.Write(middleLine.PadRight(MiddleColumnWidth));

            var rightLine = i < rightLines.Count ? rightLines[i] : "";
            Console.WriteLine(rightLine);
        }

        Console.WriteLine();
        Console.WriteLine(new string('─', SeparatorWidth));

        // Reserve exactly one status line below the separator.
        Console.WriteLine();
        var statusRow = Console.CursorTop - 1;

        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            Console.SetCursorPosition(0, statusRow);
            ConsoleHelper.SetForegroundColor(ConsoleColor.Red);
            ConsoleHelper.PrintConsole($"{FooterPadding}⚠ {ErrorMessage}");
            ConsoleHelper.ResetColor();
        }

        Console.SetCursorPosition(0, statusRow + 1);

        if (!string.IsNullOrEmpty(PromptMessage))
        {
            DrawInputPrompt($"{FooterPadding}{PromptMessage.TrimStart()}");
        }
        else
        {
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Prints one visual row of the board area, including file headers and rank numbers.
    /// </summary>
    /// <param name="lineIndex">The zero-based row index in the complete board section output.</param>
    /// <param name="board">The board model used to resolve piece symbols per tile.</param>
    /// <param name="isWhitePerspective"><c>true</c> to print coordinates from white's side; otherwise from black's side.</param>
    private static void PrintBoardLine(int lineIndex, Gameboard board, bool isWhitePerspective)
    {
        switch (lineIndex)
        {
            case 0 or 9:
            {
                var letters = isWhitePerspective
                    ? WhitePerspectiveFiles
                    : BlackPerspectiveFiles;

                var topLetters = $"    {letters}";
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
    /// Builds the text lines for the middle column containing player labels and status information.
    /// </summary>
    /// <returns>A list of formatted lines for the middle panel.</returns>
    private List<string> GenerateMiddleColumnLines()
    {
        return
        [
            MatchInfoHeader,
            SectionSeparator,
            $"│  White: {WhitePlayerName}",
            $"│  Black: {BlackPlayerName}",
            EmptySectionLine,
            StatusHeader,
            SectionSeparator,
            $"│  {StatusMessage}",
            EmptySectionLine
        ];
    }

    /// <summary>
    /// Builds the text lines for the right column containing captured pieces and recent moves.
    /// </summary>
    /// <param name="board">The board model used to read captured piece collections.</param>
    /// <returns>A list of formatted lines for the right panel.</returns>
    private List<string> GenerateRightColumnLines(Gameboard board)
    {
        var whiteCaptured = string.Join(", ", board.CapturedWhitePieces.Select(p => p.UnicodeSymbol));
        var blackCaptured = string.Join(", ", board.CapturedBlackPieces.Select(p => p.UnicodeSymbol));

        if (string.IsNullOrEmpty(whiteCaptured)) whiteCaptured = NoneLabel;
        if (string.IsNullOrEmpty(blackCaptured)) blackCaptured = NoneLabel;

        var whiteMoves = WhiteMoves.Count > 0 ? string.Join(", ", WhiteMoves) : NoneLabel;
        var blackMoves = BlackMoves.Count > 0 ? string.Join(", ", BlackMoves) : NoneLabel;

        return
        [
            CapturedPiecesHeader,
            SectionSeparator,
            $"│  White: {whiteCaptured}",
            $"│  Black: {blackCaptured}",
            EmptySectionLine,
            LastMovesHeader,
            SectionSeparator,
            $"│  White: {whiteMoves}",
            $"│  Black: {blackMoves}"
        ];
    }
}