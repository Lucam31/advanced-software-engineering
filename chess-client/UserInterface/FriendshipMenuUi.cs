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
        CliOutput.PrintConsoleNewline(ConsoleHelper.FriendsMenu);

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CliOutput.PrintConsoleNewline(errorMessage);
            Console.ResetColor();
            Console.WriteLine();
        }

        CliOutput.PrintConsoleNewline("Please enter your choice: ");
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

        CliOutput.PrintConsoleNewline("Please enter the username you want to search for (or 'Q' to quit): ");
    }

    public void DrawSearchResults(List<string> users, string? errorMessage = null)
    {
        CliOutput.ClearTerminal();
        Console.WriteLine();
        CliOutput.PrintConsoleNewline("   === SEARCH RESULTS ===");
        Console.WriteLine();

        for (int i = 0; i < users.Count; i++)
        {
            CliOutput.PrintConsoleNewline($"[{i + 1}] {users[i]}");
        }

        Console.WriteLine();

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CliOutput.PrintConsoleNewline($"⚠ {errorMessage}");
            Console.ResetColor();
            Console.WriteLine();
        }

        CliOutput.PrintConsoleNewline("Enter the number to add to your friendlist (or 'Q' to quit): ");
    }

    public void DrawListView(List<string> friendNames, string? errorMessage = null)
    {
        CliOutput.ClearTerminal();
        Console.WriteLine();
        CliOutput.PrintConsoleNewline("   === FRIEND LIST ===");
        Console.WriteLine();

        if (friendNames.Count == 0)
        {
            CliOutput.PrintConsoleNewline("You have no friends yet. Try adding some!");
        }
        else
        {
            for (var i = 0; i < friendNames.Count; i++)
            {
                CliOutput.PrintConsoleNewline($"[{i + 1}] {friendNames[i]}");
            }
        }

        Console.WriteLine();

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CliOutput.PrintConsoleNewline($"⚠ {errorMessage}");
            Console.ResetColor();
            Console.WriteLine();
        }

        CliOutput.PrintConsoleNewline("Actions: <number>D = delete, <number>P = play");
        CliOutput.PrintConsoleNewline("Enter action, press 'Q' to go back, or just Enter to refresh friendlist: ");
    }

    public void ShowMessage(string message, bool isError = false)
    {
        Console.WriteLine();
        if (isError) Console.ForegroundColor = ConsoleColor.Red;
        CliOutput.PrintConsoleNewline(message);
        Console.ResetColor();
    }

    public void ShowMessageAndWait(string message, bool isError = false)
    {
        ShowMessage(message, isError);
        Console.WriteLine();
        CliOutput.PrintConsoleNewline("Press ENTER to continue...");
        Console.ReadLine();
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