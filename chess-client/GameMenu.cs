namespace chess_client;

public class GameMenu
{
    public int DisplayMainMenu()
    {
        CLIOutput.PrintConsoleNewline(ConsoleHelper.Menu);
        CLIOutput.PrintConsoleNewline("Please enter your choice: ");
        string? input = Console.ReadLine()?.Trim().ToUpper();
        if (input == "P" || input == "PLAY")
        {
            return 1;
        }
        else if (input == "R" || input == "REPLAY")
        {
            return 2;
        }
        else if (input == "Q" || input == "QUIT")
        {
            return -1;
        }
        else
        {
            CLIOutput.PrintConsoleNewline("Invalid input. Please try again.");
            return DisplayMainMenu();
        }
    }
}