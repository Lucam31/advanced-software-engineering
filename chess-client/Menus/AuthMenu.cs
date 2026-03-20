using chess_client.Services;
using Shared.Logger;
using chess_client.UserInterface;

namespace chess_client.Menus;

/// <summary>
/// Represents the possible outcomes of the authentication menu.
/// </summary>
public enum AuthResult
{
    /// <summary>
    /// Authentication completed successfully.
    /// </summary>
    Success,

    /// <summary>
    /// User returned to the previous menu.
    /// </summary>
    Back,

    /// <summary>
    /// User requested to exit the client.
    /// </summary>
    Quit
}

/// <summary>
/// Coordinates authentication menu navigation and login/register workflows.
/// </summary>
/// <param name="authService">Service used for login and registration requests.</param>
/// <param name="userContainer">Shared user state container that stores the authenticated user id.</param>
public class AuthMenu(IAuthService authService, UserContainer userContainer)
{
    private const string LoginMode = "Login";
    private const string RegisterMode = "Register";
    private const string InvalidMenuInputMessage = "Invalid input. Please press L, R, B, or Q.";
    private const string LoginFailedPrefix = "Login failed:";
    private const string RegistrationFailedPrefix = "Registration failed:";
    private const string RegistrationSuccessMessage =
        "Registration successful! You can now log in with your credentials.";

    /// <summary>
    /// Displays the authentication menu until the user logs in, goes back, or quits.
    /// </summary>
    /// <returns>
    /// <see cref="AuthResult.Success"/> when login succeeds,
    /// <see cref="AuthResult.Back"/> when the user navigates back,
    /// or <see cref="AuthResult.Quit"/> when the user exits the client.
    /// </returns>
    public async Task<AuthResult> DisplayMenu()
    {
        string? currentErrorMessage = null;

        while (true)
        {
            GameLogger.Info("Displaying auth menu.");

            AuthMenuUi.DrawMainMenu(currentErrorMessage);
            currentErrorMessage = null;

            var input = BaseMenuUi.ReadKey();
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
                    currentErrorMessage = InvalidMenuInputMessage;
                    break;
            }
        }
    }

    /// <summary>
    /// Executes the login flow, updates user state on success, and shows an error message on failure.
    /// </summary>
    /// <returns><c>true</c> if login succeeds; otherwise, <c>false</c>.</returns>
    private async Task<bool> LoginFlow()
    {
        GameLogger.Info("Displaying login view.");

        var username = AuthMenuUi.PromptForUsername(LoginMode);
        var password = AuthMenuUi.PromptForPassword(LoginMode, username);

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
            BaseMenuUi.ShowMessageAndWait($"{LoginFailedPrefix} {ex.Message}", isError: true);
            return false;
        }
    }

    /// <summary>
    /// Executes the registration flow and shows feedback about the operation result.
    /// </summary>
    private async Task RegisterFlow()
    {
        GameLogger.Info("Displaying registration view.");

        var username = AuthMenuUi.PromptForUsername(RegisterMode);
        var password = AuthMenuUi.PromptForPassword(RegisterMode, username);

        GameLogger.Info($"Registering new user '{username}'...");

        try
        {
            await authService.Register(username, password);
            BaseMenuUi.ShowSuccessAndWait(RegistrationSuccessMessage);
        }
        catch (Exception ex)
        {
            GameLogger.Warning($"Registration failed: {ex.Message}");
            BaseMenuUi.ShowMessageAndWait($"{RegistrationFailedPrefix} {ex.Message}", isError: true);
        }
    }
}