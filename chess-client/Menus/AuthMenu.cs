using chess_client.Services;
using Shared.Logger;
using chess_client.UserInterface;

namespace chess_client.Menus;

/// <summary>
/// Manages the authentication logic (Login and Registration).
/// </summary>
public class AuthMenu(IAuthService authService, UserContainer userContainer)
{
    private readonly AuthMenuUi _ui = new();

    /// <summary>
    /// Displays the auth menu and handles logic flow.
    /// </summary>
    /// <returns>True if login was successful, false otherwise.</returns>
    public async Task<bool> DisplayMenu()
    {
        string? currentErrorMessage = null;

        while (true)
        {
            GameLogger.Info("Displaying auth menu.");

            _ui.DrawMainMenu(currentErrorMessage);
            currentErrorMessage = null; // Fehler für den nächsten Durchlauf zurücksetzen

            var input = _ui.ReadInput()?.ToUpper();
            GameLogger.Debug($"User entered menu input: '{input}'");

            switch (input)
            {
                case "L":
                case "LOGIN":
                    GameLogger.Info("User selected 'Login'.");
                    var loginSuccess = await LoginFlow();
                    if (loginSuccess) return true;
                    break;

                case "R":
                case "REGISTER":
                    GameLogger.Info("User selected 'Register'.");
                    await RegisterFlow();
                    break;

                case "Q":
                case "QUIT":
                    GameLogger.Info("User selected 'Quit'.");
                    return false;

                default:
                    GameLogger.Warning($"Invalid menu input: '{input}'");
                    currentErrorMessage = "Invalid input. Please try again.";
                    break;
            }
        }
    }

    private async Task<bool> LoginFlow()
    {
        GameLogger.Info("Displaying login view.");

        var username = _ui.PromptForUsername(
            "Please enter your username:\n> ",
            "Username cannot be empty. Please try again:\n> ");

        var password = _ui.PromptForPassword(
            "Please enter your password:\n> ",
            "Password cannot be empty. Please try again:\n> ");

        GameLogger.Info($"Attempting login for user '{username}'...");

        try
        {
            var id = await authService.Login(username, password);
            userContainer.Id = id;
            return true;
        }
        catch (Exception ex)
        {
            GameLogger.Warning($"Login failed: {ex.Message}");
            _ui.ShowMessageAndWait($"Login failed: {ex.Message}", isError: true);
            return false;
        }
    }

    private async Task RegisterFlow()
    {
        GameLogger.Info("Displaying registration view.");

        var username = _ui.PromptForUsername(
            "Please enter your desired username:\n> ",
            "Username cannot be empty. Please try again:\n> ");

        var password = _ui.PromptForPassword(
            "Please enter your desired password:\n> ",
            "Password cannot be empty. Please try again:\n> ");

        GameLogger.Info($"Registering new user '{username}'...");

        try
        {
            await authService.Register(username, password);
            _ui.ShowSuccessAndWait("Registration successful! You can now log in with your credentials.");
        }
        catch (Exception ex)
        {
            GameLogger.Warning($"Registration failed: {ex.Message}");
            _ui.ShowMessageAndWait($"Registration failed: {ex.Message}", isError: true);
        }
    }
}