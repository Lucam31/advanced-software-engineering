using Shared.Logger;
using Shared;

namespace chess_client;

/// <summary>
/// Provides static methods for parsing user input.
/// </summary>
public static class InputParser
{
    /// <summary>
    /// Reads and validates user input against a set of valid options.
    /// </summary>
    /// <param name="prompt">The message to display to the user.</param>
    /// <param name="newLine">Whether to print the prompt on a new line.</param>
    /// <param name="validInputs">An array of valid input strings.</param>
    /// <returns>The validated user input.</returns>
    public static string? ReadInput(string prompt, bool newLine = false, params string[] validInputs)
    {
        if (validInputs.Length == 0)
        {
            GameLogger.Error("ReadInput called with empty validInputs.");
            throw new ArgumentException("validInputs must contain at least one valid input.", nameof(validInputs));
        }

        if (prompt == "")
        {
            GameLogger.Warning("ReadInput called with empty prompt.");
            CliOutput.WriteErrorMessage("prompt must not be empty.");
        }

        while (true)
        {
            GameLogger.Debug($"Prompting user. newLine={newLine}, validInputs=[{string.Join(",", validInputs)}]");
            if (newLine)
            {
                CliOutput.PrintConsoleNewline(prompt);
            }
            else
            {
                CliOutput.PrintConsole(prompt);
            }

            var input = Console.ReadLine()?.Trim().ToUpper();
            GameLogger.Debug($"User entered: '{input}'");
            if (validInputs.Any(validInput => input == validInput))
            {
                GameLogger.Info($"Valid input received: '{input}'");
                return input;
            }

            GameLogger.Warning($"Invalid input: '{input}'. Expected one of: [{string.Join(",", validInputs)}]");
            CliOutput.PrintConsoleNewline("Invalid input. Please try again.");
        }
    }
    
    public static char ReadSingleCharInput(string prompt, bool newLine = false, params char[] validInputs)
    {
        if (validInputs.Length == 0)
        {
            GameLogger.Error("ReadSingleCharInput called with empty validInputs.");
            throw new ArgumentException("validInputs must contain at least one valid input.", nameof(validInputs));
        }

        if (prompt == "")
        {
            GameLogger.Warning("ReadSingleCharInput called with empty prompt.");
            CliOutput.WriteErrorMessage("prompt must not be empty.");
        }

        while (true)
        {
            GameLogger.Debug($"Prompting user. newLine={newLine}, validInputs=[{string.Join(",", validInputs)}]");
            if (newLine)
            {
                CliOutput.PrintConsoleNewline(prompt);
            }
            else
            {
                CliOutput.PrintConsole(prompt);
            }

            var input = Console.ReadKey().KeyChar;
            GameLogger.Debug($"User entered: '{input}'");
            if (validInputs.Any(validInput => input == validInput))
            {
                GameLogger.Info($"Valid input received: '{input}'");
                return input;
            }

            GameLogger.Warning($"Invalid input: '{input}'. Expected one of: [{string.Join(",", validInputs)}]");
            CliOutput.PrintConsoleNewline("Invalid input. Please try again.");
        }
    }

    /// <summary>
    /// Reads a chess move from the user in algebraic notation (e.g., "e2e4").
    /// </summary>
    /// <returns>A <see cref="Move"/> object representing the user's move.</returns>
    public static Move ReadMove()
    {
        GameLogger.Debug("Reading move from console input.");
        int fromCol, fromRow, toCol, toRow;

        // Console.Clear() can leave a stray newline in the input buffer on macOS,
        // so we skip any empty / whitespace-only lines before reading the real input.
        string? move;
        do
        {
            move = Console.ReadLine()?.Trim().ToUpper();
        } while (string.IsNullOrWhiteSpace(move));

        GameLogger.Debug($"Raw move string: '{move}'");
        if (move.Length != 4)
        {
            GameLogger.Warning($"Move input not length 4: '{move}'");
            throw new ArgumentException("Input was not valid");
        }

        fromCol = move[0];
        fromRow = move[1] - '0';
        toCol = move[2];
        toRow = move[3] - '0';
        GameLogger.Debug($"Parsed move: {Convert.ToChar(fromCol)}{fromRow} -> {Convert.ToChar(toCol)}{toRow}");

        if (fromRow == -1 || fromCol == -1 || toRow == -1 || toCol == -1)
        {
            GameLogger.Error("Parsed move contains invalid -1 values.");
        }

        if (fromCol is > 64 and < 73 && fromRow is > 0 and < 9 && toCol is > 64 and < 73 && toRow is > 0 and < 9)
        {
            var parsed = new Move($"{Convert.ToChar(fromCol)}{fromRow}", $"{Convert.ToChar(toCol)}{toRow}");
            GameLogger.Info($"Move accepted: {parsed.From} -> {parsed.To}");
            return parsed;
        }

        GameLogger.Error("Move input failed final bounds validation.");
        throw new ArgumentException("Input was not valid");
    }
}