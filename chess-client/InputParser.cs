using Shared.Logger;

namespace chess_client;

using Shared;

public static class InputParser
{
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

    public static Move ReadMove()
    {
        GameLogger.Debug("Reading move from console input.");
        int fromCol, fromRow, toCol, toRow;
        var move = Console.ReadLine()?.Trim().ToUpper();
        GameLogger.Debug($"Raw move string: '{move}'");
        if (move?.Length != 4)
        {
            GameLogger.Warning("Move input not length 4.");
            CliOutput.WriteErrorMessage("Input must be in the format 'e2e4'.");
        }

        try
        {
            fromCol = move![0];
            fromRow = move[1] - '0';
            toCol = move[2];
            toRow = move[3] - '0';
            GameLogger.Debug($"Parsed move: {Convert.ToChar(fromCol)}{fromRow} -> {Convert.ToChar(toCol)}{toRow}");
        }
        catch (Exception e)
        {
            GameLogger.Error("Exception while parsing move input.", e);
            CliOutput.OverwriteLine(e.Message);
            throw;
        }

        if (fromRow == -1 || fromCol == -1 || toRow == -1 || toCol == -1)
        {
            GameLogger.Error("Parsed move contains invalid -1 values.");
            CliOutput.WriteErrorMessage("Input must be in the format 'e2e4'.");
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