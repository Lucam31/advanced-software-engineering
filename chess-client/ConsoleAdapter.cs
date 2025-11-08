namespace chess_client;

/// <summary>
/// An interface to abstract console operations for easier testing
/// </summary>
public interface IConsoleAdapter
{
    int CursorTop { get; }
    int WindowWidth { get; }
    int WindowHeight { get; }
    void SetCursorPosition(int left, int top);
    void Write(string value);
}

/// <summary>
/// A console adapter that uses the system console
/// </summary>
internal sealed class SystemConsoleAdapter : IConsoleAdapter
{
    public static readonly SystemConsoleAdapter Instance = new();

    public int CursorTop => Console.CursorTop;
    public int WindowWidth => Console.WindowWidth;
    public int WindowHeight => Console.WindowHeight;

    public void SetCursorPosition(int left, int top) => Console.SetCursorPosition(left, top);

    public void Write(string value) => Console.Write(value);
}

