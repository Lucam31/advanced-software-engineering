using Shared;
using System.Text;
using chess_client.Menus;
using chess_client.Services;
using Shared.Logger;
using LogLevel = Shared.Logger.LogLevel;

namespace chess_client;

/// <summary>
/// The main entry point for the chess client application.
/// </summary>
internal static class Program
{
    /// <summary>
    /// The main method that runs the client application.
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

            GameLogger.Debug("Initializing core components...");
            var gameboard = new Gameboard();
            var gameLogic = new GameLogic(gameboard);
            GameLogger.Debug("Core components initialized.");

            GameLogger.Info("Displaying login menu...");

            var userContainer = new UserContainer();

            var loginMenu = new LoginMenu(new AuthService(), userContainer);
            var gameMenu = new GameMenu(userContainer);
            
            while (true)
            {
                var loggedIn = await loginMenu.DisplayMenu();
                if (!loggedIn)
                    break;
                
                GameLogger.Info("User logged in successfully.");

                gameMenu.DisplayMainMenu();
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