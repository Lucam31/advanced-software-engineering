using chess_client.Services;
using Shared.Logger;
using chess_client.UserInterface;

namespace chess_client.Menus;

/// <summary>
/// Controls the startup menu and routes users into authentication or client exit.
/// </summary>
/// <param name="authService">Authentication service used by the nested auth menu.</param>
/// <param name="userContainer">Shared user state container passed into authentication flows.</param>
public class MainMenu(IAuthService authService, UserContainer userContainer)
{
    private readonly MainMenuUi _ui = new();
    private readonly AuthMenu _authMenu = new(authService, userContainer);

    /// <summary>
    /// Displays the startup menu until the user authenticates successfully or quits.
    /// </summary>
    /// <returns><c>true</c> when authentication succeeds; otherwise <c>false</c> when the user quits.</returns>
    public async Task<bool> DisplayMenu()
    {
        string? currentErrorMessage = null;

        while (true)
        {
            GameLogger.Info("Displaying startup menu.");

            MainMenuUi.DrawMenu(currentErrorMessage);
            currentErrorMessage = null;

            var input = MainMenuUi.ReadKey();
            GameLogger.Debug($"User pressed key: '{input.Key}'");

            switch (input.Key)
            {
                case ConsoleKey.A:
                    GameLogger.Info("User selected 'Authenticate'.");

                    var authResult = await _authMenu.DisplayMenu();
                    
                    if (authResult == AuthResult.Success)
                    {
                        return true;
                    }
                    if (authResult == AuthResult.Quit)
                    {
                        return false;
                    }
                    
                    break;

                case ConsoleKey.Q:
                    GameLogger.Info("User selected 'Quit'.");
                    return false;

                default:
                    GameLogger.Warning($"Invalid menu input: '{input.Key}'");
                    currentErrorMessage = "Invalid input. Please press A or Q.";
                    break;
            }
        }
    }
}