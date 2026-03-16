using Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation and console interactions for the main menu.
/// </summary>
public class GameMenuUi
{
    /// <summary>
    /// Clears the screen and draws the main menu with a clean boxed layout.
    /// </summary>
    public void DrawMainMenu(string? errorMessage = null)
    {
        CliOutput.ClearTerminal();
        Console.WriteLine();
        CliOutput.PrintConsoleNewline("       === CHESS DASHBOARD ===");
        Console.WriteLine();

        CliOutput.PrintConsoleNewline("   ┌──────────────────────────────┐");
        CliOutput.PrintConsoleNewline("   │          MAIN MENU           │");
        CliOutput.PrintConsoleNewline("   ├──────────────────────────────┤");
        CliOutput.PrintConsoleNewline("   │  [P] Play                    │");
        CliOutput.PrintConsoleNewline("   │  [F] Friends                 │");
        CliOutput.PrintConsoleNewline("   │  [G] Games and Replays       │");
        CliOutput.PrintConsoleNewline("   │  [L] Logout                  │");
        CliOutput.PrintConsoleNewline("   │  [Q] Quit Game               │");
        CliOutput.PrintConsoleNewline("   └──────────────────────────────┘");
        Console.WriteLine();

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CliOutput.PrintConsoleNewline($"   ⚠ {errorMessage}");
            Console.ResetColor();
            Console.WriteLine();
        }

        CliOutput.PrintConsoleNewline("   Your choice: ");
    }

    /// <summary>
    /// Displays a generic status message to the user.
    /// </summary>
    public void ShowMessage(string message)
    {
        Console.WriteLine();
        CliOutput.PrintConsoleNewline($"   ℹ {message}");
    }

    /// <summary>
    /// Reads a key press asynchronously, allowing the action to be cancelled by background events.
    /// </summary>
    public async Task<ConsoleKeyInfo> ReadKeyAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (Console.KeyAvailable)
            {
                return Console.ReadKey(true);
            }

            await Task.Delay(20, token);
        }

        token.ThrowIfCancellationRequested();
        return default;
    }
}