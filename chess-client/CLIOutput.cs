using System;
using Shared.Logger;

namespace chess_client;

using Shared;

/// <summary>
/// Provides helper methods for command-line interface output.
/// </summary>
public static class CliOutput
{
    /// <summary>
    /// The console adapter used for output
    /// </summary>
    private static IConsoleAdapter _console = SystemConsoleAdapter.Instance;

    /// <summary>
    /// For testing: replace the console adapter.
    /// </summary>
    public static void SetConsole(IConsoleAdapter console) => _console = console;

    /// <summary>
    /// For testing: reset the console adapter to the default system console.
    /// </summary>
    public static void ResetConsole() => _console = SystemConsoleAdapter.Instance;

    /// <summary>
    /// Clears the current line in the console.
    /// </summary>
    public static void ClearCurrentConsoleLine()
    {
        GameLogger.Debug("Clearing current console line.");
        var currentLineCursor = _console.CursorTop;
        _console.SetCursorPosition(0, _console.CursorTop);
        _console.Write(new string(' ', _console.WindowWidth));
        _console.SetCursorPosition(0, currentLineCursor);
    }

    /// <summary>
    /// Prints a message to the console, followed by a new line.
    /// </summary>
    /// <param name="message">The message to print.</param>
    public static void PrintConsoleNewline(string message)
    {
        GameLogger.Debug($"PrintConsoleNewline: '{message}'");
        _console.Write("\n{0}".Replace("{0}", message));
    }

    /// <summary>
    /// Prints a message to the console.
    /// </summary>
    /// <param name="message">The message to print.</param>
    public static void PrintConsole(string message)
    {
        GameLogger.Debug($"PrintConsole: '{message}'");
        _console.Write(message);
    }

    /// <summary>
    /// Overwrites the current console line with a new message.
    /// </summary>
    /// <param name="message">The message to write.</param>
    public static void OverwriteLine(string message)
    {
        GameLogger.Debug($"OverwriteLine: '{message}'");
        ClearCurrentConsoleLine();
        _console.Write(message);
    }

    /// <summary>
    /// Writes an error message to the console, overwriting the previous line.
    /// </summary>
    /// <param name="message">The error message to write.</param>
    public static void WriteErrorMessage(string message)
    {
        GameLogger.Warning($"WriteErrorMessage: '{message}'");
        _console.SetCursorPosition(0, _console.CursorTop - 1);
        OverwriteLine(message);
    }

    /// <summary>
    /// Overwrites a line at a position relative to the current cursor position.
    /// </summary>
    /// <param name="targetLine">The relative line number to overwrite.</param>
    /// <param name="message">The message to write.</param>
    public static void OverwriteLineRelative(int targetLine, string message)
    {
        var prevLine = _console.CursorTop;
        if (targetLine < 0 || targetLine >= _console.WindowHeight)
        {
            GameLogger.Error(
                $"OverwriteLineRelative out of range: targetLine={targetLine}, WindowHeight={_console.WindowHeight}");
            throw new ArgumentOutOfRangeException(nameof(targetLine));
        }

        GameLogger.Debug($"OverwriteLineRelative targetLine={targetLine}, message='{message}'");
        _console.SetCursorPosition(0, _console.CursorTop - targetLine);
        OverwriteLine(message);
        _console.SetCursorPosition(0, prevLine);
    }

    /// <summary>
    /// Clears the console and redraws the game board.
    /// </summary>
    /// <param name="board">The game board to redraw.</param>
    public static void RewriteBoard(Gameboard board)
    {
        GameLogger.Debug("RewriteBoard called.");
        var prevLine = _console.CursorTop - 1;

        _console.SetCursorPosition(0, 0);
        board.PrintBoard();
        _console.SetCursorPosition(0, prevLine);
        ClearCurrentConsoleLine();
        _console.Write("Enter your next move: ");
    }
}