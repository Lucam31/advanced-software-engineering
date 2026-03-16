using Shared;
using System;

namespace chess_client.UserInterface; // oder chess_client.Menus, je nach deiner Ordnerstruktur

/// <summary>
/// Handles the visual representation and console interactions for authentication (Login/Register).
/// </summary>
public class AuthMenuUi
{
    /// <summary>
    /// Draws the main authentication menu.
    /// </summary>
    public void DrawMainMenu(string? errorMessage = null)
    {
        CliOutput.ClearTerminal();
        CliOutput.PrintConsoleNewline(ConsoleHelper.LoginMenu);

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CliOutput.PrintConsoleNewline(errorMessage);
            Console.ResetColor();
            Console.WriteLine();
        }

        CliOutput.PrintConsoleNewline("Please enter your choice: ");
    }

    public string? ReadInput()
    {
        return Console.ReadLine()?.Trim();
    }

    /// <summary>
    /// Prompts the user for a username until a non-empty string is provided.
    /// </summary>
    public string PromptForUsername(string prompt, string errorPrompt)
    {
        string? username;
        var currentPrompt = prompt;

        while (true)
        {
            CliOutput.ClearTerminal();
            CliOutput.PrintConsoleNewline(currentPrompt);
            username = Console.ReadLine()?.Trim();

            if (!string.IsNullOrEmpty(username)) break;

            currentPrompt = errorPrompt;
        }

        return username;
    }

    /// <summary>
    /// Prompts the user for a password until a non-empty string is provided.
    /// </summary>
    public string PromptForPassword(string prompt, string errorPrompt)
    {
        string? password;
        var currentPrompt = prompt;

        while (true)
        {
            CliOutput.ClearTerminal();
            password = CliOutput.ReadPassword(currentPrompt);

            if (!string.IsNullOrEmpty(password)) break;

            currentPrompt = errorPrompt;
        }

        return password;
    }

    /// <summary>
    /// Shows a message and waits for the user to press a key.
    /// </summary>
    public void ShowMessageAndWait(string message, bool isError = false)
    {
        CliOutput.ClearTerminal();
        
        if (isError) Console.ForegroundColor = ConsoleColor.Red;
        CliOutput.PrintConsoleNewline(message);
        Console.ResetColor();
        
        Console.WriteLine();
        CliOutput.PrintConsoleNewline("Press any key to return to the menu...");
        Console.ReadKey(true);
    }

    /// <summary>
    /// Shows a success message in green and waits for the user to press a key.
    /// </summary>
    public void ShowSuccessAndWait(string message)
    {
        CliOutput.ClearTerminal();
        
        Console.ForegroundColor = ConsoleColor.Green;
        CliOutput.PrintConsoleNewline(message);
        Console.ResetColor();
        
        Console.WriteLine();
        CliOutput.PrintConsoleNewline("Press any key to continue...");
        Console.ReadKey(true);
    }
}