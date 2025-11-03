using Shared.Logger;

namespace chess_client;

using Shared;

public static class CliOutput
{
    private static void ClearCurrentConsoleLine()
    {
        GameLogger.Debug("Clearing current console line.");
        var currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor);
    }

    public static void PrintConsoleNewline(string message)
    {
        GameLogger.Debug($"PrintConsoleNewline: '{message}'");
        Console.Write("\n{0}", message);
    }

    public static void PrintConsole(string message)
    {
        GameLogger.Debug($"PrintConsole: '{message}'");
        Console.Write(message);
    }

    public static void OverwriteLine(string message)
    {
        GameLogger.Debug($"OverwriteLine: '{message}'");
        ClearCurrentConsoleLine();
        Console.Write(message);
    }

    public static void WriteErrorMessage(string message)
    {
        GameLogger.Warning($"WriteErrorMessage: '{message}'");
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        OverwriteLine(message);
    }

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