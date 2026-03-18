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
        ConsoleHelper.ClearTerminal();
        Console.WriteLine();
        ConsoleHelper.PrintConsoleNewline("   === REPLAY MENU ===");
        Console.WriteLine();

        if (gameDisplays.Count == 0)
        {
            ConsoleHelper.PrintConsoleNewline("   No games found.");
        }
        else
        {
            ConsoleHelper.PrintConsoleNewline("   Select a game to replay:");
            Console.WriteLine();

            for (var i = 0; i < gameDisplays.Count; i++)
            {
                ConsoleHelper.PrintConsoleNewline($"   [{i + 1}] {gameDisplays[i]}");
            }
        }

        Console.WriteLine();

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleHelper.PrintConsoleNewline($"   ⚠ {errorMessage}");
            Console.ResetColor();
            Console.WriteLine();
        }

        ConsoleHelper.PrintConsoleNewline("   [Q] Return to main menu");
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