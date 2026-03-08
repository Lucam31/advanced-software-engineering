using chess_client.Services;
using Shared.Logger;

namespace chess_client.Menus;

/// <summary>
/// Manages the login menu of the application.
/// </summary>
public class LoginMenu
{
    private readonly IAuthService _authService;
    private readonly UserContainer _userContainer;
    
    /// <summary>
    /// Initializes a new instance of the LoginMenu class.
    /// </summary>
    /// <param name="authService">The authentication service.</param>
    /// <param name="userContainer">The user container.</param>
    public LoginMenu(IAuthService authService, UserContainer userContainer)
    {
        _authService = authService;
        _userContainer = userContainer;
    }
    
    /// <summary>
    /// Displays the login menu and handles user input.
    /// </summary>
    /// <returns>True if login was successful, false otherwise.</returns>
    public async Task<bool> DisplayMenu()
    {
        while (true)
        {
            CliOutput.ClearTerminal(); 
            GameLogger.Info("Displaying login menu.");

            CliOutput.PrintConsoleNewline(ConsoleHelper.LoginMenu);
            CliOutput.PrintConsoleNewline("Please enter your choice: ");
            var input = Console.ReadLine()?.Trim().ToUpper();

            GameLogger.Debug($"User entered menu input: '{input}'");

            switch (input)
            {
                case "L":
                case "LOGIN":
                    GameLogger.Info("User selected 'Login'.");
                    
                    var loginSuccess = await LoginView();
                    if (loginSuccess) return true; 
                    continue;

                case "R":
                case "REGISTER":
                    GameLogger.Info("User selected 'Register'.");
                    await RegisterView();
                    continue;

                case "Q":
                case "QUIT":
                    GameLogger.Info("User selected 'Quit'.");
                    return false;

                default:
                    GameLogger.Warning($"Invalid menu input: '{input}'");
                    CliOutput.PrintConsoleNewline("Invalid input. Press any key to try again...");
                    Console.ReadKey(true);
                    continue;
            }
        }
    }

    /// <summary>
    /// Handles the login view.
    /// </summary>
    /// <returns>True if login was successful.</returns>
    private async Task<bool> LoginView()
    {
        GameLogger.Info("Displaying login view.");

        string? username;
        var usernamePrompt = "Please enter your username:\n> ";

        while (true)
        {
            CliOutput.ClearTerminal();
            CliOutput.PrintConsoleNewline(usernamePrompt);
            username = Console.ReadLine()?.Trim();

            if (!string.IsNullOrEmpty(username)) break;

            usernamePrompt = "Username cannot be empty. Please try again:\n> ";
        }

        string password;
        var passwordPrompt = "Please enter your password:\n> ";

        while (true)
        {
            CliOutput.ClearTerminal();
            password = CliOutput.ReadPassword(passwordPrompt);

            if (!string.IsNullOrEmpty(password)) break;

            passwordPrompt = "Password cannot be empty. Please try again:\n> ";
        }

        GameLogger.Info($"Attempting login for user '{username}'...");
        
        try
        {
            var id = await _authService.Login(username, password);
            _userContainer.Id = id;
            return true;
        }
        catch (Exception ex)
        {
            GameLogger.Warning($"Login failed: {ex.Message}");
            CliOutput.ClearTerminal();
            CliOutput.PrintConsoleNewline($"Login failed: {ex.Message}");
            CliOutput.PrintConsoleNewline("Press any key to return to the menu...");
            Console.ReadKey(true);
            return false;
        }
    }

    /// <summary>
    /// Handles the registration view.
    /// </summary>
    private async Task RegisterView()
    {
        GameLogger.Info("Displaying registration view.");

        string? username;
        var usernamePrompt = "Please enter your desired username:\n> ";

        while (true)
        {
            CliOutput.ClearTerminal();
            CliOutput.PrintConsoleNewline(usernamePrompt);
            username = Console.ReadLine()?.Trim();

            if (!string.IsNullOrEmpty(username)) break;

            usernamePrompt = "Username cannot be empty. Please try again:\n> ";
        }

        string password;
        var passwordPrompt = "Please enter your desired password:\n> ";

        while (true)
        {
            CliOutput.ClearTerminal();
            password = CliOutput.ReadPassword(passwordPrompt);

            if (!string.IsNullOrEmpty(password)) break;

            passwordPrompt = "Password cannot be empty. Please try again:\n> ";
        }

        GameLogger.Info($"Registering new user '{username}'...");
        
        try
        {
            await _authService.Register(username, password);
            
            CliOutput.ClearTerminal();
            CliOutput.PrintConsoleNewline("Registration successful! You can now log in with your credentials.");
            CliOutput.PrintConsoleNewline("Press any key to continue...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            GameLogger.Warning($"Registration failed: {ex.Message}");
            CliOutput.ClearTerminal();
            CliOutput.PrintConsoleNewline($"Registration failed: {ex.Message}");
            CliOutput.PrintConsoleNewline("Press any key to return to the menu...");
            Console.ReadKey(true);
        }
    }
}