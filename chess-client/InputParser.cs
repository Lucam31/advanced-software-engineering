namespace chess_client;
using Shared;

public static class InputParser
{
    public static string ReadInput(string prompt, bool newLine = false, params string[] validInputs)
    {
        if (validInputs.Length == 0 || validInputs == null)
        {
            throw new ArgumentException("validInputs must contain at least one valid input.", nameof(validInputs));
        }

        if (prompt == "")
        {
            CLIOutput.WriteErrorMessage("prompt must not be empty.");
        }

        while (true)
        {
            if (newLine)
            {
                CLIOutput.PrintConsoleNewline(prompt);
            }
            else
            {
                CLIOutput.PrintConsole(prompt);
            }
            string? input = Console.ReadLine()?.Trim().ToUpper();
            foreach (string validInput in validInputs)
            {
                if (input == validInput) return input;
            }
            CLIOutput.PrintConsoleNewline("Invalid input. Please try again.");
        }
    }

    public static Move ReadMove()
    {
        int fromCol, fromRow, toCol, toRow = -1;
        string? move = Console.ReadLine()?.Trim().ToUpper();
        if (move?.Length < 4 || move?.Length > 4)
        {
            CLIOutput.WriteErrorMessage("Input must be in the format 'e2e4'.");
        }
        try
        {
            fromCol = move[0];
            fromRow = move[1]-'0';
            toCol = move[2];
            toRow = move[3]-'0';
        }
        catch (Exception e)
        {
            CLIOutput.OverwriteLine(e.Message);
            throw;
        }

        if (fromRow == -1 || fromCol == -1 || toRow == -1 || toCol == -1)
        {
            CLIOutput.WriteErrorMessage("Input must be in the format 'e2e4'.");
        }

        if (fromCol is > 64 and < 73 && fromRow is > 0 and < 9 && toCol is > 64 and < 73 && toRow is > 0 and < 9)
        {
            return new Move($"{Convert.ToChar(fromCol)}{fromRow}",$"{Convert.ToChar(toCol)}{toRow}");
        }

        throw new ArgumentException("Input was not valid");
    }
}