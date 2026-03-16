namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation and console interactions for authentication (Login/Register).
/// </summary>
public class AuthMenuUi
{
    /// <summary>
    /// Draws the main authentication menu with a clean boxed layout.
    /// </summary>
    public void DrawMainMenu(string? errorMessage = null)
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
    /// Prompts the user for a username.
    /// </summary>
    public string PromptForUsername(string actionTitle)
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
    /// Prompts the user for a password, showing the entered username.
    /// </summary>
    public string PromptForPassword(string actionTitle, string enteredUsername)
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

    public void ShowMessageAndWait(string message, bool isError = false)
    {
        Console.WriteLine();
        if (isError) Console.ForegroundColor = ConsoleColor.Red;
        CliOutput.PrintConsoleNewline($"   {(isError ? "⚠" : "ℹ")} {message}");
        Console.ResetColor();

        Console.WriteLine();
        CliOutput.PrintConsoleNewline("   Press ENTER to return...");
        Console.ReadLine();
    }

    public void ShowSuccessAndWait(string message)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        CliOutput.PrintConsoleNewline($"   ✔ {message}");
        Console.ResetColor();

        Console.WriteLine();
        CliOutput.PrintConsoleNewline("   Press ENTER to continue...");
        Console.ReadLine();
    }

    public ConsoleKeyInfo ReadKey()
    {
        return Console.ReadKey(true);
    }
}