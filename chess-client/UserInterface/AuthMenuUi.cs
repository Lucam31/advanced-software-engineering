namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation and console interactions for authentication (Login/Register).
/// </summary>
public class AuthMenuUi : BaseMenuUi
{
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
}