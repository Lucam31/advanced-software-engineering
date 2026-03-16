using Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation and console interactions for the friendship menu.
/// </summary>
public class FriendshipMenuUi
{
    public void DrawMainMenu(string? errorMessage = null)
    {
        CliOutput.ClearTerminal();
        Console.WriteLine();
        CliOutput.PrintConsoleNewline("        === FRIENDS ===");
        Console.WriteLine();

        CliOutput.PrintConsoleNewline("   ┌──────────────────────────────┐");
        CliOutput.PrintConsoleNewline("   │       FRIEND MENU            │");
        CliOutput.PrintConsoleNewline("   ├──────────────────────────────┤");
        CliOutput.PrintConsoleNewline("   │  [S] Search Users            │");
        CliOutput.PrintConsoleNewline("   │  [L] Friend List             │");
        CliOutput.PrintConsoleNewline("   │  [B] Back to Dashboard       │");
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

    public void DrawSearchPrompt(string? errorMessage = null)
    {
        CliOutput.ClearTerminal();
        Console.WriteLine();
        CliOutput.PrintConsoleNewline("   === SEARCH USERS ===");
        Console.WriteLine();

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CliOutput.PrintConsoleNewline($"   ⚠ {errorMessage}");
            Console.ResetColor();
            Console.WriteLine();
        }

        CliOutput.PrintConsoleNewline("   Please enter the username you want to search for.");
        CliOutput.PrintConsoleNewline("   (Type 'B' to go back, 'Q' to quit)");
        Console.Write("   > ");
    }

    public void DrawSearchResults(List<string> users, string? errorMessage = null)
    {
        CliOutput.ClearTerminal();
        Console.WriteLine();
        CliOutput.PrintConsoleNewline("   === SEARCH RESULTS ===");
        Console.WriteLine();

        if (users.Count == 0)
        {
            CliOutput.PrintConsoleNewline("   No users found.");
        }
        else
        {
            for (int i = 0; i < users.Count; i++)
            {
                CliOutput.PrintConsoleNewline($"   [{i + 1}] {users[i]}");
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

        CliOutput.PrintConsoleNewline("   Enter the number to add to your friendlist.");
        CliOutput.PrintConsoleNewline("   (Type 'B' to go back, 'Q' to quit)");
        Console.Write("   > ");
    }

    public void DrawListView(List<string> friendNames, string? errorMessage = null)
    {
        CliOutput.ClearTerminal();
        Console.WriteLine();
        CliOutput.PrintConsoleNewline("   === FRIEND LIST ===");
        Console.WriteLine();

        if (friendNames.Count == 0)
        {
            CliOutput.PrintConsoleNewline("   You have no friends yet. Try adding some!");
        }
        else
        {
            for (var i = 0; i < friendNames.Count; i++)
            {
                CliOutput.PrintConsoleNewline($"   [{i + 1}] {friendNames[i]}");
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

        CliOutput.PrintConsoleNewline("   Actions: <number>D = delete, <number>P = play");
        CliOutput.PrintConsoleNewline("   (Type 'B' to go back, 'Q' to quit, or press Enter to refresh)");
        Console.Write("   > ");
    }

    public void ShowMessage(string message, bool isError = false)
    {
        Console.WriteLine();
        if (isError) Console.ForegroundColor = ConsoleColor.Red;
        CliOutput.PrintConsoleNewline($"   {(isError ? "⚠" : "ℹ")} {message}");
        Console.ResetColor();
    }

    public void ShowMessageAndWait(string message, bool isError = false)
    {
        ShowMessage(message, isError);
        Console.WriteLine();
        CliOutput.PrintConsoleNewline("   Press ENTER to continue...");
        Console.ReadLine();
    }

    public ConsoleKeyInfo ReadKey()
    {
        return Console.ReadKey(true);
    }

    public string? ReadInput()
    {
        return Console.ReadLine()?.Trim();
    }

    public async Task<string?> ReadInputAsync(CancellationToken token)
    {
        return await ConsoleHelper.ReadLineAsync(token);
    }
}