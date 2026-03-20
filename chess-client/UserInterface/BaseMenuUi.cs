namespace chess_client.UserInterface;

/// <summary>
/// Base class providing shared console interactions and rendering logic for all menus.
/// </summary>
public abstract class BaseMenuUi
{
    private const string ContinuePrompt = "   Press ENTER to continue...";
    private const string DefaultInputPrompt = "   Your choice: ";
    private const string DefaultErrorPrefix = "   ⚠ ";
    private const string ErrorIcon = "⚠";
    private const string InfoIcon = "ℹ";
    private const string SuccessIcon = "✔";

    /// <summary>
    /// Displays a message with optional error styling.
    /// </summary>
    public static void ShowMessage(string message, bool isError = false)
    {
        var icon = isError ? ErrorIcon : InfoIcon;
        var color = isError ? ConsoleColor.Red : (ConsoleColor?)null;
        DrawStatusMessage(message, icon, color);
    }

    /// <summary>
    /// Displays a message and waits for the user to press ENTER.
    /// </summary>
    public static void ShowMessageAndWait(string message, bool isError = false)
    {
        ShowMessage(message, isError);
        ConsoleHelper.WriteEmptyLine();
        ConsoleHelper.PrintConsoleNewline(ContinuePrompt);
        Console.ReadLine();
    }

    /// <summary>
    /// Displays a success message and waits for the user to continue.
    /// </summary>
    public static void ShowSuccessAndWait(string message)
    {
        DrawStatusMessage(message, SuccessIcon, ConsoleColor.Green);
        ConsoleHelper.WriteEmptyLine();
        ConsoleHelper.PrintConsoleNewline(ContinuePrompt);
        Console.ReadLine();
    }

    /// <summary>
    /// Draws a section header line surrounded by empty lines.
    /// </summary>
    /// <param name="title">The section title text to display.</param>
    public static void DrawSectionHeader(string title)
    {
        ConsoleHelper.WriteEmptyLine();
        ConsoleHelper.PrintConsoleNewline(title);
        ConsoleHelper.WriteEmptyLine();
    }

    /// <summary>
    /// Draws an optional error message using the shared warning style.
    /// </summary>
    /// <param name="errorMessage">The error message to display; null or empty values are ignored.</param>
    /// <param name="addTrailingEmptyLine"><c>true</c> to print an empty line after the error message.</param>
    public static void DrawOptionalError(string? errorMessage, bool addTrailingEmptyLine = true)
    {
        if (string.IsNullOrEmpty(errorMessage))
        {
            return;
        }

        ConsoleHelper.SetForegroundColor(ConsoleColor.Red);
        ConsoleHelper.PrintConsoleNewline($"{DefaultErrorPrefix}{errorMessage}");
        ConsoleHelper.ResetColor();

        if (addTrailingEmptyLine)
        {
            ConsoleHelper.WriteEmptyLine();
        }
    }

    /// <summary>
    /// Draws a numbered list using one-based indexes.
    /// </summary>
    /// <param name="entries">The entries to print.</param>
    public static void DrawIndexedList(IReadOnlyList<string> entries)
    {
        for (var i = 0; i < entries.Count; i++)
        {
            ConsoleHelper.PrintConsoleNewline($"   [{i + 1}] {entries[i]}");
        }
    }

    /// <summary>
    /// Draws a menu input prompt without adding a trailing newline.
    /// </summary>
    /// <param name="prompt">The prompt text to show.</param>
    public static void DrawInputPrompt(string prompt = DefaultInputPrompt)
    {
        ConsoleHelper.PrintConsole(prompt);
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

    /// <summary>
    /// Draws a status message with a prefix icon and optional foreground color.
    /// </summary>
    /// <param name="message">The message text to display.</param>
    /// <param name="icon">The status icon shown before the message.</param>
    /// <param name="color">Optional text color for the message.</param>
    private static void DrawStatusMessage(string message, string icon, ConsoleColor? color = null)
    {
        ConsoleHelper.WriteEmptyLine();

        if (color.HasValue)
        {
            ConsoleHelper.SetForegroundColor(color.Value);
        }

        ConsoleHelper.PrintConsoleNewline($"   {icon} {message}");
        ConsoleHelper.ResetColor();
    }
}