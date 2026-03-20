using System.Text;
using chess_client.Menus;
using chess_client.Services;
using Shared.Logger;
using LogLevel = Shared.Logger.LogLevel;

namespace chess_client;

/// <summary>
/// Provides the console application entry point and orchestrates client startup and shutdown.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Configures logging, initializes services and menus, and runs the main application loop.
    /// </summary>
    private static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var enableConsoleLog = !string.Equals(Environment.GetEnvironmentVariable("CONSOLE_LOG"), "false",
            StringComparison.OrdinalIgnoreCase);
        var enableFileLog = !string.Equals(Environment.GetEnvironmentVariable("FILE_LOG"), "false",
            StringComparison.OrdinalIgnoreCase);

        GameLogger.Configure(
            minLevel: LogLevel.Debug,
            logToConsole: enableConsoleLog,
            logToFile: enableFileLog,
            logFilePath: "logs/client_log.txt"
        );

        try
        {
            GameLogger.Info("Client application starting...");

            var userContainer = new UserContainer();
            var webSocketService = new WebSocketService();
            var gameService = new GameService(userContainer, webSocketService);
            var authService = new AuthService();
            var friendshipServices = new FriendshipServices();

            var startupMenu = new MainMenu(authService, userContainer);
            var friendshipMenu = new FriendshipMenu(userContainer, friendshipServices, gameService, webSocketService);
            var gameMenu = new GameMenu(userContainer, friendshipMenu, gameService, webSocketService);

            while (true)
            {
                GameLogger.Info("Displaying startup menu...");

                var isLoggedIn = await startupMenu.DisplayMenu();

                if (!isLoggedIn)
                {
                    GameLogger.Info("User requested to quit the application.");
                    break;
                }

                GameLogger.Info("User logged in successfully.");

                if (!webSocketService.IsConnected)
                {
                    var wsUri = $"ws://localhost:8080/ws?userId={userContainer.Id}";
                    var connected = await webSocketService.ConnectAsync(wsUri);
                    if (!connected)
                        GameLogger.Warning("WebSocket-Verbindung fehlgeschlagen. Läuft ohne Echtzeit-Updates.");
                }

                var menuResult = await gameMenu.DisplayMainMenu();

                if (menuResult == GameMenuResult.Quit)
                {
                    GameLogger.Info("User requested to quit the application from the dashboard.");
                    break;
                }
            }

            GameLogger.Info("Client shutting down normally.");
        }
        catch (Exception ex)
        {
            GameLogger.Fatal("An unhandled exception occurred! Application will crash.", ex);
            throw;
        }
    }
}