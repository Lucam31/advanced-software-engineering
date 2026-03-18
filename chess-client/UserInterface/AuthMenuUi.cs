namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation and console interactions for authentication (Login/Register).
/// </summary>
public class AuthMenuUi : BaseMenuUi
{
    private const string PromptChoice = "   Your choice: ";
    private const string UsernamePrompt = "   Username: ";
    private const string PasswordPrompt = "   Password: ";
    private const string EmptyUsernameError = "Username cannot be empty.";
    private const string EmptyPasswordError = "Password cannot be empty.";
    private const string UsernameDisplayFormat = "   Username: {0}";

    private const string MainMenuLayout =
        """
               === AUTHENTICATION ===

           ┌──────────────────────────────┐
           │       LOGIN & REGISTER       │
           ├──────────────────────────────┤
           │  [L] Login                   │
           │  [R] Register                │
           │  [B] Back to Main Menu       │
           │  [Q] Quit Game               │
           └──────────────────────────────┘
        """;

    /// <summary>
    /// Draws the main menu with login/register options.
    /// </summary>
    /// <param name="errorMessage">Optional error message to display prominently on the menu.</param>
    public static void DrawMainMenu(string? errorMessage = null)
    {
        ConsoleHelper.ClearTerminal();
        ConsoleHelper.PrintConsoleNewline(MainMenuLayout);
        ConsoleHelper.WriteEmptyLine();

        DrawOptionalError(errorMessage);
        ConsoleHelper.PrintConsoleNewline(PromptChoice);
    }

    /// <summary>
    /// Prompts the user for a username.
    /// </summary>
    /// <param name="actionTitle">The title of the action being performed (e.g., "Login" or "Register") to display in the prompt.</param>
    /// <returns>The username entered by the user.</returns>
    public static string PromptForUsername(string actionTitle)
    {
        string? username = null;
        string? errorMessage = null;

        while (true)
        {
            ConsoleHelper.ClearTerminal();
            DrawAuthHeader(actionTitle);
            BaseMenuUi.DrawOptionalError(errorMessage);

            ConsoleHelper.PrintConsoleNewline(UsernamePrompt);
            username = Console.ReadLine()?.Trim();

            if (!string.IsNullOrEmpty(username)) break;

            errorMessage = EmptyUsernameError;
        }

        return username;
    }

    /// <summary>
    /// Prompts the user for a password.
    /// </summary>
    /// <param name="actionTitle">The title of the action being performed (e.g., "Login" or "Register") to display in the prompt.</param>
    /// <param name="enteredUsername">The username that was entered, to display in the prompt for context.</param>
    /// <returns>The password entered by the user.</returns>
    public static string PromptForPassword(string actionTitle, string enteredUsername)
    {
        string? password = null;
        string? errorMessage = null;

        while (true)
        {
            ConsoleHelper.ClearTerminal();
            DrawAuthHeader(actionTitle);
            DrawDisplayedUsername(enteredUsername);
            DrawOptionalError(errorMessage);

            password = ConsoleHelper.ReadPassword(PasswordPrompt);

            if (!string.IsNullOrEmpty(password)) break;

            errorMessage = EmptyPasswordError;
        }

        return password;
    }

    /// <summary>
    /// Draws an authentication action header centered on screen.
    /// </summary>
    /// <param name="actionTitle">The action name to display (e.g., "Login", "Register").</param>
    private static void DrawAuthHeader(string actionTitle)
    {
        ConsoleHelper.WriteEmptyLine();
        ConsoleHelper.PrintConsoleNewline($"   === {actionTitle.ToUpper()} ===");
        ConsoleHelper.WriteEmptyLine();
    }

    /// <summary>
    /// Displays a previously entered username in muted style.
    /// </summary>
    /// <param name="username">The username to display.</param>
    private static void DrawDisplayedUsername(string username)
    {
        ConsoleHelper.SetForegroundColor(ConsoleColor.DarkGray);
        ConsoleHelper.PrintConsoleNewline(string.Format(UsernameDisplayFormat, username));
        ConsoleHelper.ResetColor();
        ConsoleHelper.WriteEmptyLine();
    }
}