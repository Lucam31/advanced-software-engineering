using Shared;
using System;
using System.Collections.Generic;

namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation and console interactions for the replay menu.
/// </summary>
public class ReplayMenuUi
{
    /// <summary>
    /// Clears the screen and draws the replay menu with selectable game entries.
    /// </summary>
    /// <param name="gameDisplays">Formatted game labels shown as numbered options.</param>
    /// <param name="errorMessage">Optional error message shown above the input prompt.</param>
    public static void DrawMenu(List<string> gameDisplays, string? errorMessage = null)
    {
        CliOutput.ClearTerminal();
        Console.WriteLine();
        CliOutput.PrintConsoleNewline("   === REPLAY MENU ===");
        Console.WriteLine();

        if (gameDisplays.Count == 0)
        {
            CliOutput.PrintConsoleNewline("   No games found.");
        }
        else
        {
            CliOutput.PrintConsoleNewline("   Select a game to replay:");
            Console.WriteLine();

            for (int i = 0; i < gameDisplays.Count; i++)
            {
                CliOutput.PrintConsoleNewline($"   [{i + 1}] {gameDisplays[i]}");
            }
        }

        Console.WriteLine();

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CliOutput.PrintConsoleNewline($"   ⚠ {errorMessage}");
            Console.ResetColor();
            Console.WriteLine();
        }

        CliOutput.PrintConsoleNewline("   [Q] Return to main menu");
        Console.WriteLine();
        Console.Write("   Your choice: ");
    }

    /// <summary>
    /// Reads a single key press from the user without echoing it to the console.
    /// </summary>
    /// <returns>The pressed key information.</returns>
    public static ConsoleKeyInfo ReadKey()
    {
        return Console.ReadKey(true);
    }
}