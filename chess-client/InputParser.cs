namespace chess_client;

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
            throw new ArgumentException("prompt must not be empty.");
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

    public static string ReadMove()
    {
        int fromCol, fromRow, toCol, toRow = -1;
        string? move = Console.ReadLine()?.Trim().ToUpper();
        if (move?.Length < 4 || move?.Length > 4)
        {
            throw new ArgumentException("Input must be in the format 'e2e4'.");
        }
        try
        {
            fromRow = move[0];
            fromCol = move[1];
            toRow = move[2];
            toCol = move[3];
        }
        catch (Exception e)
        {
            CLIOutput.OverwriteLine(e.Message);
            throw;
        }

        if (fromRow == -1 || fromCol == -1 || toRow == -1 || toCol == -1)
        {
            throw new ArgumentException("Input must be in the format 'e2e4'.");
        }

        if (fromRow is > 64 and < 73 && fromCol is > 0 and < 9 && toRow is > 64 and < 73 && toCol is > 0 and < 9)
        {
            return move;
        }

        throw new ArgumentException("Input must be between A-H and 1-8.");
    }
}