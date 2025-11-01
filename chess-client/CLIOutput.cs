namespace chess_client;
using Shared;

public static class CLIOutput
{ 
    public static void ClearCurrentConsoleLine()
    {
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth)); 
        Console.SetCursorPosition(0, currentLineCursor);
    }
    
    public static void PrintConsoleNewline(string message)
    {
        Console.Write("\n{0}", message);
    }
    public static void PrintConsole(string message)
    {
        Console.Write(message);
    }
    public static void OverwriteLine(string message)
    {
        ClearCurrentConsoleLine();
        Console.Write(message);
    }

    public static void WriteErrorMessage(string message)
    {
        Console.SetCursorPosition(0, Console.CursorTop-1);
        OverwriteLine(message);
    }

    public static void OverwriteLineRelative(int targetLine, string message)
    {
        int prevLine = Console.CursorTop;
        if (targetLine < 0 || targetLine >= Console.WindowHeight)
        {
            throw new ArgumentOutOfRangeException(nameof(targetLine));
        }
        
        Console.SetCursorPosition(0, Console.CursorTop-targetLine);
        OverwriteLine(message);
        Console.SetCursorPosition(0, prevLine);
    }
    
    public static void RewriteBoard(Gameboard board)
    {
        int prevLine = Console.CursorTop-1;
        
        Console.SetCursorPosition(0, 0);
        board.PrintBoard();
        Console.SetCursorPosition(0, prevLine);
        ClearCurrentConsoleLine();
        Console.Write("Enter your next move: ");
    }
    
    
}