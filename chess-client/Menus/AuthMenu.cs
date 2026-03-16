using chess_client.Services;
using Shared.Logger;
using chess_client.UserInterface;
using System;
using System.Threading.Tasks;

namespace chess_client.Menus;

public enum AuthResult
{
    Success,
    Back,
    Quit
}

public class AuthMenu(IAuthService authService, UserContainer userContainer)
{
    private readonly AuthMenuUi _ui = new();

    public async Task<AuthResult> DisplayMenu()
    {
        string? currentErrorMessage = null;

        while (true)
        {
            GameLogger.Info("Displaying auth menu.");

            _ui.DrawMainMenu(currentErrorMessage);
            currentErrorMessage = null;

            var input = _ui.ReadKey();
            GameLogger.Debug($"User pressed key: '{input.Key}'");

            switch (input.Key)
            {
                case ConsoleKey.L:
                    GameLogger.Info("User selected 'Login'.");
                    var loginSuccess = await LoginFlow();
                    if (loginSuccess) return AuthResult.Success;
                    break;

                case ConsoleKey.R:
                    GameLogger.Info("User selected 'Register'.");
                    await RegisterFlow();
                    break;

                case ConsoleKey.B:
                case ConsoleKey.Escape:
                    GameLogger.Info("User selected 'Back'.");
                    return AuthResult.Back;

                case ConsoleKey.Q:
                    GameLogger.Info("User selected 'Quit'.");
                    return AuthResult.Quit;

                default:
                    GameLogger.Warning($"Invalid menu input: '{input.Key}'");
                    currentErrorMessage = "Invalid input. Please press L, R, B, or Q.";
                    break;
            }
        }
    }

    private async Task<bool> LoginFlow()
    {
        GameLogger.Info("Displaying login view.");

        var username = _ui.PromptForUsername("Login");
        var password = _ui.PromptForPassword("Login", username);

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

        var username = _ui.PromptForUsername("Register");
        var password = _ui.PromptForPassword("Register", username);

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