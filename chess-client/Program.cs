using Shared;
using System.Text;
using LogLevel = Shared.LogLevel;

namespace chess_client;

internal static class Program
{
    private static Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var enableConsole = true;
        var consoleLogEnv = Environment.GetEnvironmentVariable("CONSOLE_LOG");
        if (consoleLogEnv != null && consoleLogEnv.Equals("false", StringComparison.OrdinalIgnoreCase))
        {
            enableConsole = false;
        }

        GameLogger.Configure(
            minLevel: LogLevel.Debug,
            logToConsole: enableConsole,
            logToFile: true,
            logFilePath: "logs/client_log.txt"
        );

        try
        {
            GameLogger.Info("Client application starting...");

            GameLogger.Debug("Initializing core components...");
            var gameboard = new Gameboard();
            var gameLogic = new GameLogic(gameboard);
            GameLogger.Debug("Core components initialized.");

            GameLogger.Info("Displaying main menu...");
            var start = GameMenu.DisplayMainMenu();
            GameLogger.Debug($"Main menu returned decision: {start}");

            if (start)
            {
                GameLogger.Info("User selected 'Start Game'.");
                CliOutput.PrintConsole("Starting a new game...");

                gameLogic.StartNewGame();

                GameLogger.Info("Game loop finished.");
            }
            else
            {
                GameLogger.Info("User selected 'Exit' from main menu.");
            }

            CliOutput.PrintConsoleNewline("Closing Application...");
            GameLogger.Info("Client application shutting down normally.");
        }
        catch (Exception ex)
        {
            GameLogger.Fatal("An unhandled exception occurred! Application will crash.", ex);
            throw;
        }

        return Task.CompletedTask;
    }
}