using Shared;

namespace chess_client.UserInterface;

/// <summary>
/// Manages the UI state and renders the command-line chess game screen.
/// </summary>
public class GameUi
{
    private const int BoardWidth = 36;
    private const int MiddleColumnWidth = 38;

    public string WhitePlayerName { get; set; } = "Player 1 (White)";
    public string BlackPlayerName { get; set; } = "Player 2 (Black)";
    public string StatusMessage { get; set; } = "Game starting...";
    private List<string> WhiteMoves { get; } = [];
    private List<string> BlackMoves { get; } = [];
    public string ErrorMessage { get; set; } = "";
    public string PromptMessage { get; set; } = "";

    public void AddMoveToHistory(string from, string to, bool isWhite)
    {
        var move = $"{from}{to}";

        if (isWhite)
            WhiteMoves.Add(move);
        else
            BlackMoves.Add(move);
    }

    public void DrawGameScreen(Gameboard board, bool isWhitePerspective)
    {
        Console.Clear();
        Console.WriteLine();

        var middleLines = GenerateMiddleColumnLines();
        var rightLines = GenerateRightColumnLines(board);

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
        Console.WriteLine(new string('─', 85));

        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ErrorMessage);
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine();
        }

        if (!string.IsNullOrEmpty(PromptMessage))
        {
            Console.Write(PromptMessage);
        }
        else
        {
            Console.WriteLine();
        }
    }

    private static void PrintBoardLine(int lineIndex, Gameboard board, bool isWhitePerspective)
    {
        switch (lineIndex)
        {
            case 0 or 9:
            {
                var letters = isWhitePerspective
                    ? "A  B  C  D  E  F  G  H"
                    : "H  G  F  E  D  C  B  A";

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

    private List<string> GenerateMiddleColumnLines()
    {
        return
        [
            "│  MATCH INFO",
            "│  ───────────────────────",
            $"│  White: {WhitePlayerName}",
            $"│  Black: {BlackPlayerName}",
            "│",
            "│  STATUS",
            "│  ───────────────────────",
            $"│  {StatusMessage}",
            "│"
        ];
    }

    private List<string> GenerateRightColumnLines(Gameboard board)
    {
        var whiteCaptured = string.Join(", ", board.CapturedWhitePieces.Select(p => p.UnicodeSymbol));
        var blackCaptured = string.Join(", ", board.CapturedBlackPieces.Select(p => p.UnicodeSymbol));

        if (string.IsNullOrEmpty(whiteCaptured)) whiteCaptured = "None";
        if (string.IsNullOrEmpty(blackCaptured)) blackCaptured = "None";

        var whiteMoves = WhiteMoves.Count > 0 ? string.Join(", ", WhiteMoves) : "None";
        var blackMoves = BlackMoves.Count > 0 ? string.Join(", ", BlackMoves) : "None";

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