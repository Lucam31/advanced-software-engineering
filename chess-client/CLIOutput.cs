using Shared.Logger;

namespace chess_client;

using Shared;

/// <summary>
/// Provides helper methods for command-line interface output.
/// </summary>
public static class CliOutput
{
    /// <summary>
    /// Clears the current line in the console.
    /// </summary>
    private static void ClearCurrentConsoleLine()
    {
        GameLogger.Debug("Clearing current console line.");
        var currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor);
    }

    /// <summary>
    /// Prints a message to the console, followed by a new line.
    /// </summary>
    /// <param name="message">The message to print.</param>
    public static void PrintConsoleNewline(string message)
    {
        GameLogger.Debug($"PrintConsoleNewline: '{message}'");
        Console.Write("\n{0}", message);
    }

    /// <summary>
    /// Prints a message to the console.
    /// </summary>
    /// <param name="message">The message to print.</param>
    public static void PrintConsole(string message)
    {
        GameLogger.Debug($"PrintConsole: '{message}'");
        Console.Write(message);
    }

    /// <summary>
    /// Overwrites the current console line with a new message.
    /// </summary>
    /// <param name="message">The message to write.</param>
    public static void OverwriteLine(string message)
    {
        GameLogger.Debug($"OverwriteLine: '{message}'");
        ClearCurrentConsoleLine();
        Console.Write(message);
    }

    /// <summary>
    /// Writes an error message to the console, overwriting the previous line.
    /// </summary>
    /// <param name="message">The error message to write.</param>
    public static void WriteErrorMessage(string message)
    {
        GameLogger.Warning($"WriteErrorMessage: '{message}'");
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        OverwriteLine(message);
    }

    /// <summary>
    /// Overwrites a line at a position relative to the current cursor position.
    /// </summary>
    /// <param name="targetLine">The relative line number to overwrite.</param>
    /// <param name="message">The message to write.</param>
    public static void OverwriteLineRelative(int targetLine, string message)
    {
        var prevLine = Console.CursorTop;
        if (targetLine < 0 || targetLine >= Console.WindowHeight)
        {
            GameLogger.Error(
                $"OverwriteLineRelative out of range: targetLine={targetLine}, WindowHeight={Console.WindowHeight}");
            throw new ArgumentOutOfRangeException(nameof(targetLine));
        }

        GameLogger.Debug($"OverwriteLineRelative targetLine={targetLine}, message='{message}'");
        Console.SetCursorPosition(0, Console.CursorTop - targetLine);
        OverwriteLine(message);
        Console.SetCursorPosition(0, prevLine);
    }

    /// <summary>
    /// Clears the console and redraws the game board.
    /// </summary>
    /// <param name="board">The game board to redraw.</param>
    public static void RewriteBoard(Gameboard board)
    {
        GameLogger.Debug("RewriteBoard called.");
        var prevLine = Console.CursorTop - 1;

        Console.SetCursorPosition(0, 0);
        board.PrintBoard();
        Console.SetCursorPosition(0, prevLine);
        ClearCurrentConsoleLine();
        Console.Write("Enter your next move: ");
    }
}