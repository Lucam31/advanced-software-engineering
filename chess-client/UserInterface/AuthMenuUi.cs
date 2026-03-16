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
        CliOutput.ClearTerminal();
        CliOutput.PrintConsoleNewline("        === AUTHENTICATION ===");
        Console.WriteLine();

        CliOutput.PrintConsoleNewline("   ┌──────────────────────────────┐");
        CliOutput.PrintConsoleNewline("   │       LOGIN & REGISTER       │");
        CliOutput.PrintConsoleNewline("   ├──────────────────────────────┤");
        CliOutput.PrintConsoleNewline("   │  [L] Login                   │");
        CliOutput.PrintConsoleNewline("   │  [R] Register                │");
        CliOutput.PrintConsoleNewline("   │  [B] Back to Main Menu       │");
        CliOutput.PrintConsoleNewline("   │  [Q] Quit Game               │");
        CliOutput.PrintConsoleNewline("   └──────────────────────────────┘");
        Console.WriteLine();

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CliOutput.PrintConsoleNewline($"   ⚠ {errorMessage}");
            Console.ResetColor();
            Console.WriteLine();
        }

        CliOutput.PrintConsoleNewline("   Your choice: ");
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
            CliOutput.ClearTerminal();
            Console.WriteLine();
            CliOutput.PrintConsoleNewline($"   === {actionTitle.ToUpper()} ===");
            Console.WriteLine();

            if (!string.IsNullOrEmpty(errorMessage))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                CliOutput.PrintConsoleNewline($"   ⚠ {errorMessage}");
                Console.ResetColor();
                Console.WriteLine();
            }

            CliOutput.PrintConsoleNewline("   Username: ");
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
            CliOutput.ClearTerminal();
            Console.WriteLine();
            CliOutput.PrintConsoleNewline($"   === {actionTitle.ToUpper()} ===");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            CliOutput.PrintConsoleNewline($"   Username: {enteredUsername}");
            Console.ResetColor();
            Console.WriteLine();

            if (!string.IsNullOrEmpty(errorMessage))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                CliOutput.PrintConsoleNewline($"   ⚠ {errorMessage}");
                Console.ResetColor();
                Console.WriteLine();
            }

            password = CliOutput.ReadPassword("   Password: ");

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
        CliOutput.PrintConsoleNewline($"   {(isError ? "⚠" : "ℹ")} {message}");
        Console.ResetColor();

        Console.WriteLine();
        CliOutput.PrintConsoleNewline("   Press ENTER to return...");
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
        CliOutput.PrintConsoleNewline($"   ✔ {message}");
        Console.ResetColor();

        Console.WriteLine();
        CliOutput.PrintConsoleNewline("   Press ENTER to continue...");
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