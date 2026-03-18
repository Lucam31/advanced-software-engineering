namespace chess_client.UserInterface;

/// <summary>
/// Base class providing shared console interactions and rendering logic for all menus.
/// </summary>
public abstract class BaseMenuUi
{
    /// <summary>
    /// Displays a message with optional error styling.
    /// </summary>
    public static void ShowMessage(string message, bool isError = false)
    {
        Console.WriteLine();
        if (isError) Console.ForegroundColor = ConsoleColor.Red;
        ConsoleHelper.PrintConsoleNewline($"   {(isError ? "⚠" : "ℹ")} {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Displays a message and waits for the user to press ENTER.
    /// </summary>
    public static void ShowMessageAndWait(string message, bool isError = false)
    {
        ShowMessage(message, isError);
        Console.WriteLine();
        ConsoleHelper.PrintConsoleNewline("   Press ENTER to continue...");
        Console.ReadLine();
    }

    /// <summary>
    /// Displays a success message and waits for the user to continue.
    /// </summary>
    public static void ShowSuccessAndWait(string message)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        ConsoleHelper.PrintConsoleNewline($"   ✔ {message}");
        Console.ResetColor();

        Console.WriteLine();
        ConsoleHelper.PrintConsoleNewline("   Press ENTER to continue...");
        Console.ReadLine();
    }

    /// <summary>
    /// Reads a single key press without echoing it to the console.
    /// </summary>
    public static ConsoleKeyInfo ReadKey()
    {
        return Console.ReadKey(true);
    }

    /// <summary>
    /// Reads a line of input and trims surrounding whitespace.
    /// </summary>
    public static string? ReadInput()
    {
        return Console.ReadLine()?.Trim();
    }

    /// <summary>
    /// Reads a line of input asynchronously and supports cancellation.
    /// </summary>
    public static async Task<string?> ReadInputAsync(CancellationToken token)
    {
        return await ConsoleHelper.ReadLineAsync(token);
    }
    
    /// <summary>
    /// Reads a key press asynchronously, allowing the action to be cancelled by background events.
    /// </summary>
    public static async Task<ConsoleKeyInfo> ReadKeyAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (Console.KeyAvailable)
            {
                return Console.ReadKey(true);
            }
            
            await Task.Delay(20, token);
        }
        
        token.ThrowIfCancellationRequested();
        return default; 
    }
}