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
                    return await LoginView();
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
                    CliOutput.PrintConsoleNewline("Invalid input. Please try again.");
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

        string username;
        string password;

        while (true)
        {
            CliOutput.PrintConsoleNewline("Please enter your username: ");
            var input = Console.ReadLine()?.Trim();
            
            GameLogger.Debug($"User entered username: '{input}'");
            if (input == null)
            {
                GameLogger.Warning("User entered null username.");
                CliOutput.PrintConsoleNewline("Username cannot be empty. Please try again.");
                continue;
            }
            username = input;
            break;
        }
        
        while (true)
        {
            CliOutput.PrintConsoleNewline("Please enter your password: ");
            var input = Console.ReadLine()?.Trim();
            
            GameLogger.Debug($"User entered password: '{input}'");
            if (input == null)
            {
                GameLogger.Warning("User entered null password.");
                CliOutput.PrintConsoleNewline("Password cannot be empty. Please try again.");
                continue;
            }
            password = input;
            break;
        }
        
        var id = await _authService.Login(username, password);
        _userContainer.Id = id;
        
        return true;
    }

    /// <summary>
    /// Handles the registration view.
    /// </summary>
    private async Task RegisterView()
    {
        GameLogger.Info("Displaying registration view.");

        string username;
        string password;

        while (true)
        {
            CliOutput.PrintConsoleNewline("Please enter your desired username: ");
            var input = Console.ReadLine()?.Trim();
            
            GameLogger.Debug($"User entered username: '{input}'");
            if (input == null)
            {
                GameLogger.Warning("User entered null username.");
                CliOutput.PrintConsoleNewline("Username cannot be empty. Please try again.");
                continue;
            }
            username = input;
            break;
        }
        
        while (true)
        {
            CliOutput.PrintConsoleNewline("Please enter your desired password: ");
            var input = Console.ReadLine()?.Trim();
            
            GameLogger.Debug($"User entered password: '{input}'");
            if (input == null)
            {
                GameLogger.Warning("User entered null password.");
                CliOutput.PrintConsoleNewline("Password cannot be empty. Please try again.");
                continue;
            }
            password = input;
            break;
        }
        
        await _authService.Register(username, password);
        
        CliOutput.PrintConsoleNewline("Registration successful! You can now log in with your credentials.");
    }
}