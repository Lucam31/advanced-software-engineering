using chess_client.Services;
using Shared.Logger;
using chess_client.UserInterface;

namespace chess_client.Menus;

/// <summary>
/// Manages the very first menu the user sees when starting the application.
/// </summary>
public class MainMenu(IAuthService authService, UserContainer userContainer)
{
    private readonly MainMenuUi _ui = new();
    private readonly AuthMenu _authMenu = new(authService, userContainer);

    /// <summary>
    /// Displays the startup menu.
    /// </summary>
    /// <returns>True if the user successfully logged in, false if they want to quit.</returns>
    public async Task<bool> DisplayMenu()
    {
        string? currentErrorMessage = null;

        while (true)
        {
            GameLogger.Info("Displaying startup menu.");

            _ui.DrawMenu(currentErrorMessage);
            currentErrorMessage = null;

            var input = _ui.ReadKey();
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