using Shared;

namespace chess_client;

/// <summary>
/// Provides utility methods to render the chess board and related HUD columns in the CLI client.
/// 
/// This class is responsible for composing the board ASCII rendering together with
/// a middle information column (match info / status) and a right column (captured pieces / moves).
/// It supports rendering the board from either player's perspective.
/// </summary>
public static class GameUi
{
    private const int BoardWidth = 36;
    private const int MiddleColumnWidth = 38;

    /// <summary>
    /// Clears the console and draws the entire game screen: the board on the left,
    /// informational columns in the middle and on the right.
    /// </summary>
    /// <param name="board">The <see cref="Gameboard"/> to render.</param>
    /// <param name="stats">The <see cref="GameStats"/> instance containing display names, status and move lists.</param>
    /// <param name="isWhitePerspective">If true, render the board from White's perspective; otherwise render from Black's perspective.</param>
    public static void DrawGameScreen(Gameboard board, GameStats stats, bool isWhitePerspective)
    {
        Console.Clear();
        Console.WriteLine();

        var middleLines = GenerateMiddleColumnLines(stats);
        var rightLines = GenerateRightColumnLines(board, stats);

        var maxLines = Math.Max(10, Math.Max(middleLines.Count, rightLines.Count));

        for (var i = 0; i < maxLines; i++)
        {
            switch (i)
            {
                case 0 or 9:
                {
                    const string topLetters = "    A  B  C  D  E  F  G  H";
                    Console.Write(topLetters.PadRight(BoardWidth));
                    break;
                }
                case >= 1 and <= 8:
                {
                    var rank = isWhitePerspective ? 8 - (i - 1) : (i - 1) + 1;
                    Console.Write($" {rank} ");

                    for (var j = 0; j < 8; j++)
                    {
                        var file = isWhitePerspective ? j : 7 - j;
                        var isDark = (file + rank) % 2 == 0;

                        Console.BackgroundColor = isDark ? ConsoleColor.Gray : ConsoleColor.DarkGray;

                        var tile = board[rank - 1, file];
                        if (tile.CurrentPiece != null)
                        {
                            Console.ForegroundColor =
                                tile.CurrentPiece.IsWhite ? ConsoleColor.White : ConsoleColor.Black;
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
                    Console.Write(new string(' ', BoardWidth));
                    break;
            }

            var middleLine = i < middleLines.Count ? middleLines[i] : "";
            Console.Write(middleLine.PadRight(MiddleColumnWidth));

            var rightLine = i < rightLines.Count ? rightLines[i] : "";
            Console.WriteLine(rightLine);
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Generates the text lines for the middle column (match info & status).
    /// </summary>
    /// <param name="stats">The current game statistics and display information.</param>
    /// <returns>A list of lines to print in the middle column.</returns>
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
    /// Generates the text lines for the right column (captured pieces & last moves).
    /// </summary>
    /// <param name="board">The gameboard containing captured piece information.</param>
    /// <param name="stats">The current game statistics with per-side move lists.</param>
    /// <returns>A list of lines to print in the right column.</returns>
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