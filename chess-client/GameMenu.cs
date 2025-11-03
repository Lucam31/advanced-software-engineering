namespace chess_client;

using Shared;
using System;

public class GameMenu
{
    public static bool DisplayMainMenu()
    {
        while (true)
        {
            GameLogger.Info("Displaying main menu.");

            CliOutput.PrintConsoleNewline(ConsoleHelper.Menu);
            CliOutput.PrintConsoleNewline("Please enter your choice: ");
            var input = Console.ReadLine()?.Trim().ToUpper();

            GameLogger.Debug($"User entered menu input: '{input}'");

            switch (input)
            {
                case "P":
                case "PLAY":
                    GameLogger.Info("User selected 'Play'.");
                    return true;
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
}