namespace chess_client;

public class GameMenu
{
    public bool DisplayMainMenu()
    {
        CLIOutput.PrintConsoleNewline(ConsoleHelper.Menu);
        CLIOutput.PrintConsoleNewline("Please enter your choice: ");
        string? input = Console.ReadLine()?.Trim().ToUpper();
        if (input == "P" || input == "PLAY")
        {
            return true;
        }
        else if (input == "Q" || input == "QUIT")
        {
            return false;
        }
        else
        {
            CLIOutput.PrintConsoleNewline("Invalid input. Please try again.");
            return DisplayMainMenu();
        }
    }
}