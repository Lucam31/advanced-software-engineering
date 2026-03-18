namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation and console interactions for authentication (Login/Register).
/// </summary>
public class AuthMenuUi
{
    /// <summary>
    /// Draws the main authentication menu with a clean boxed layout.
    /// </summary>
    /// <param name="errorMessage">Optional error message shown above the input prompt.</param>
    public static void DrawMainMenu(string? errorMessage = null)
    {
        ConsoleHelper.ClearTerminal();
        ConsoleHelper.PrintConsoleNewline("        === AUTHENTICATION ===");
        Console.WriteLine();

        ConsoleHelper.PrintConsoleNewline("   ┌──────────────────────────────┐");
        ConsoleHelper.PrintConsoleNewline("   │       LOGIN & REGISTER       │");
        ConsoleHelper.PrintConsoleNewline("   ├──────────────────────────────┤");
        ConsoleHelper.PrintConsoleNewline("   │  [L] Login                   │");
        ConsoleHelper.PrintConsoleNewline("   │  [R] Register                │");
        ConsoleHelper.PrintConsoleNewline("   │  [B] Back to Main Menu       │");
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
    /// Prompts the user for a non-empty username and repeats until valid input is provided.
    /// </summary>
    /// <param name="actionTitle">Context label displayed in the prompt header (for example, Login or Register).</param>
    /// <returns>The entered username.</returns>
    public static string PromptForUsername(string actionTitle)
    {
        string? username = null;
        string? errorMessage = null;

        while (true)
        {
            ConsoleHelper.ClearTerminal();
            Console.WriteLine();
            ConsoleHelper.PrintConsoleNewline($"   === {actionTitle.ToUpper()} ===");
            Console.WriteLine();

            if (!string.IsNullOrEmpty(errorMessage))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                ConsoleHelper.PrintConsoleNewline($"   ⚠ {errorMessage}");
                Console.ResetColor();
                Console.WriteLine();
            }

            ConsoleHelper.PrintConsoleNewline("   Username: ");
            username = Console.ReadLine()?.Trim();

            if (!string.IsNullOrEmpty(username)) break;

            errorMessage = "Username cannot be empty.";
        }

        return username;
    }

    /// <summary>
    /// Prompts the user for a non-empty password while showing the selected username as context.
    /// </summary>
    /// <param name="actionTitle">Context label displayed in the prompt header (for example, Login or Register).</param>
    /// <param name="enteredUsername">Username shown above the password prompt.</param>
    /// <returns>The entered password.</returns>
    public static string PromptForPassword(string actionTitle, string enteredUsername)
    {
        string? password = null;
        string? errorMessage = null;

        while (true)
        {
            ConsoleHelper.ClearTerminal();
            Console.WriteLine();
            ConsoleHelper.PrintConsoleNewline($"   === {actionTitle.ToUpper()} ===");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            ConsoleHelper.PrintConsoleNewline($"   Username: {enteredUsername}");
            Console.ResetColor();
            Console.WriteLine();

            if (!string.IsNullOrEmpty(errorMessage))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                ConsoleHelper.PrintConsoleNewline($"   ⚠ {errorMessage}");
                Console.ResetColor();
                Console.WriteLine();
            }

            password = ConsoleHelper.ReadPassword("   Password: ");

            if (!string.IsNullOrEmpty(password)) break;

            errorMessage = "Password cannot be empty.";
        }

        return password;
    }

    /// <summary>
    /// Displays a status message and waits for the user to acknowledge it.
    /// </summary>
    /// <param name="message">Message to display.</param>
    /// <param name="isError"><c>true</c> to display the message as an error; otherwise informational styling is used.</param>
    public static void ShowMessageAndWait(string message, bool isError = false)
    {
        Console.WriteLine();
        if (isError) Console.ForegroundColor = ConsoleColor.Red;
        ConsoleHelper.PrintConsoleNewline($"   {(isError ? "⚠" : "ℹ")} {message}");
        Console.ResetColor();

        Console.WriteLine();
        ConsoleHelper.PrintConsoleNewline("   Press ENTER to return...");
        Console.ReadLine();
    }

    /// <summary>
    /// Displays a success message and waits for the user to continue.
    /// </summary>
    /// <param name="message">Success message to display.</param>
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
    /// <returns>The pressed key information.</returns>
    public static ConsoleKeyInfo ReadKey()
    {
        return Console.ReadKey(true);
    }
}