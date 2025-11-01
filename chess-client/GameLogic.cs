namespace chess_client;
using Shared;

public class GameLogic
{
    private Gameboard _gameboard;

    public GameLogic(Gameboard gameboard)
    {
        _gameboard = gameboard;
    }
    public void StartNewGame()
    {
        Console.Clear();
        
        CLIOutput.PrintConsoleNewline("Connecting to server...");
        // establish connection here
        bool connected = true; // placeholder for connection status
        if (connected)
        {
            CLIOutput.OverwriteLine("Connection successful!");
            Console.Clear();
            _gameboard.PrintBoard();
            CLIOutput.PrintConsoleNewline("Game Started! To play a move, enter it in the format 'e2e4' to move from e2 to e4: ");
            GameplayLoop();
        }
    }

    private void GameplayLoop()
    {
        Move move;
        while (true)
        {
            move = InputParser.ReadMove();
            if (MoveValidator.ValidateMove(move, _gameboard))
            {
                _gameboard.Move(move);
                CLIOutput.RewriteBoard(_gameboard);
            }
            else
            {
                CLIOutput.WriteErrorMessage("Invalid move, try again: ");
                continue;
            }
        }
        return;
         
    }
}