namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation and console interactions for the friendship menu.
/// </summary>
public class FriendshipMenuUi
{
    /// <summary>
    /// Draws the friendship main menu.
    /// </summary>
    /// <param name="errorMessage">Optional error message shown above the input prompt.</param>
    public static void DrawMainMenu(string? errorMessage = null)
    {
        ConsoleHelper.ClearTerminal();
        ConsoleHelper.PrintConsoleNewline("            === FRIENDS ===");
        Console.WriteLine();
        ConsoleHelper.PrintConsoleNewline("   ┌──────────────────────────────┐");
        ConsoleHelper.PrintConsoleNewline("   │          FRIEND MENU         │");
        ConsoleHelper.PrintConsoleNewline("   ├──────────────────────────────┤");
        ConsoleHelper.PrintConsoleNewline("   │  [S] Search Users            │");
        ConsoleHelper.PrintConsoleNewline("   │  [L] Friend List             │");
        ConsoleHelper.PrintConsoleNewline("   │  [B] Back to Dashboard       │");
        ConsoleHelper.PrintConsoleNewline("   │  [Q] Quit Game               │");
        ConsoleHelper.PrintConsoleNewline("   └──────────────────────────────┘");
        Console.WriteLine();

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleHelper.PrintConsoleNewline($"   ⚠ {errorMessage}");
            Console.ResetColor();
            Console.WriteLine();
        }

        ConsoleHelper.PrintConsoleNewline("   Your choice: ");
    }

    /// <summary>
    /// Draws the user search prompt in the friendship section.
    /// </summary>
    /// <param name="errorMessage">Optional error message shown above the input prompt.</param>
    public static void DrawSearchPrompt(string? errorMessage = null)
    {
        ConsoleHelper.ClearTerminal();
        Console.WriteLine();
        ConsoleHelper.PrintConsoleNewline("   === SEARCH USERS ===");
        Console.WriteLine();

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleHelper.PrintConsoleNewline($"   ⚠ {errorMessage}");
            Console.ResetColor();
            Console.WriteLine();
        }

        ConsoleHelper.PrintConsoleNewline("   Please enter the username you want to search for.");
        ConsoleHelper.PrintConsoleNewline("   (Type 'B' to go back, 'Q' to quit)");
        Console.Write("   > ");
    }

    /// <summary>
    /// Displays search results and prompts for selecting a user to add as a friend.
    /// </summary>
    /// <param name="users">Usernames returned by the current search.</param>
    /// <param name="errorMessage">Optional error message shown above the input prompt.</param>
    public void DrawSearchResults(List<string> users, string? errorMessage = null)
    {
        ConsoleHelper.ClearTerminal();
        Console.WriteLine();
        ConsoleHelper.PrintConsoleNewline("   === SEARCH RESULTS ===");
        Console.WriteLine();

        if (users.Count == 0)
        {
            ConsoleHelper.PrintConsoleNewline("   No users found.");
        }
        else
        {
            for (var i = 0; i < users.Count; i++)
            {
                ConsoleHelper.PrintConsoleNewline($"   [{i + 1}] {users[i]}");
            }
        }

        Console.WriteLine();

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleHelper.PrintConsoleNewline($"   ⚠ {errorMessage}");
            Console.ResetColor();
            Console.WriteLine();
        }

        ConsoleHelper.PrintConsoleNewline("   Enter the number to add to your friendlist.");
        ConsoleHelper.PrintConsoleNewline("   (Type 'B' to go back, 'Q' to quit)");
        Console.Write("   > ");
    }

    /// <summary>
    /// Displays the friend list and available actions for each entry.
    /// </summary>
    /// <param name="friendNames">Names of friends to display.</param>
    /// <param name="errorMessage">Optional error message shown above the input prompt.</param>
    public static void DrawListView(List<string> friendNames, string? errorMessage = null)
    {
        ConsoleHelper.ClearTerminal();
        Console.WriteLine();
        ConsoleHelper.PrintConsoleNewline("   === FRIEND LIST ===");
        Console.WriteLine();

        if (friendNames.Count == 0)
        {
            ConsoleHelper.PrintConsoleNewline("   You have no friends yet. Try adding some!");
        }
        else
        {
            for (var i = 0; i < friendNames.Count; i++)
            {
                ConsoleHelper.PrintConsoleNewline($"   [{i + 1}] {friendNames[i]}");
            }
        }

        Console.WriteLine();

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleHelper.PrintConsoleNewline($"   ⚠ {errorMessage}");
            Console.ResetColor();
            Console.WriteLine();
        }

        ConsoleHelper.PrintConsoleNewline("   Actions: <number>D = delete, <number>P = play");
        ConsoleHelper.PrintConsoleNewline("   (Type 'B' to go back, 'Q' to quit, or press Enter to refresh)");
        Console.Write("   > ");
    }

    /// <summary>
    /// Displays a message with optional error styling.
    /// </summary>
    /// <param name="message">Message to display.</param>
    /// <param name="isError"><c>true</c> to display the message as an error; otherwise informational styling is used.</param>
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
    /// <param name="message">Message to display.</param>
    /// <param name="isError"><c>true</c> to display the message as an error; otherwise informational styling is used.</param>
    public static void ShowMessageAndWait(string message, bool isError = false)
    {
        ShowMessage(message, isError);
        Console.WriteLine();
        ConsoleHelper.PrintConsoleNewline("   Press ENTER to continue...");
        Console.ReadLine();
    }

    /// <summary>
    /// Reads a single key press without echoing it to the console.
    /// </summary>
    /// <returns>The pressed key information.</returns>
    public static ConsoleKeyInfo ReadKey()
    {
        return Console.ReadKey(true);
    }

    /// <summary>
    /// Reads a line of input and trims surrounding whitespace.
    /// </summary>
    /// <returns>The trimmed input or <c>null</c> when no input is available.</returns>
    public static string? ReadInput()
    {
        return Console.ReadLine()?.Trim();
    }

    /// <summary>
    /// Reads a line of input asynchronously and supports cancellation.
    /// </summary>
    /// <param name="token">Cancellation token used to interrupt waiting for input.</param>
    /// <returns>The entered input or <c>null</c>.</returns>
    public static async Task<string?> ReadInputAsync(CancellationToken token)
    {
        return await ConsoleHelper.ReadLineAsync(token);
    }
}