namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation of the matchmaking queue.
/// </summary>
public class MatchmakingUi
{
    /// <summary>
    /// Clears the screen and draws the matchmaking queue view.
    /// </summary>
    /// <param name="errorMessage">Optional error message shown above the queue input prompt.</param>
    public static void DrawQueueScreen(string? errorMessage = null)
    {
        ConsoleHelper.ClearTerminal();
        Console.WriteLine();
        ConsoleHelper.PrintConsoleNewline("   === MATCHMAKING ===");
        Console.WriteLine();
        ConsoleHelper.PrintConsoleNewline("   Searching for an opponent...");
        ConsoleHelper.PrintConsoleNewline("   Please wait.");
        Console.WriteLine();

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleHelper.PrintConsoleNewline($"   {errorMessage}");
            Console.ResetColor();
            Console.WriteLine();
        }

        ConsoleHelper.PrintConsoleNewline("   [Q] Quit Queue");
        Console.WriteLine();
        ConsoleHelper.PrintConsoleNewline("   Your input: ");
    }

    /// <summary>
    /// Displays a temporary message before transitioning away.
    /// </summary>
    /// <param name="message">Message text to display.</param>
    public static void ShowMessage(string message)
    {
        Console.WriteLine();
        ConsoleHelper.PrintConsoleNewline($"   {message}");
    }

    /// <summary>
    /// Reads queue input asynchronously and supports cancellation.
    /// </summary>
    /// <param name="token">Cancellation token used to interrupt waiting for input.</param>
    /// <returns>The entered input, or <c>null</c> when no input is available.</returns>
    public static async Task<string?> ReadInputAsync(CancellationToken token)
    {
        return await ConsoleHelper.ReadLineAsync(token);
    }

    /// <summary>
    /// Reads a key press asynchronously, allowing the action to be cancelled by background events (like finding a match).
    /// </summary>
    public async Task<ConsoleKeyInfo> ReadKeyAsync(CancellationToken token)
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