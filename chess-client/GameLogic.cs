using Shared.Logger;

namespace chess_client;

using Shared;

public class GameLogic
{
    private Gameboard _gameboard;

    public GameLogic(Gameboard gameboard)
    {
        _gameboard = gameboard;
        GameLogger.Debug("GameLogic initialized with Gameboard instance.");
    }

    public void StartNewGame()
    {
        GameLogger.Info("Starting new game sequence.");
        Console.Clear();

        CliOutput.PrintConsoleNewline("Connecting to server...");
        GameLogger.Debug("Attempting to connect to server (placeholder)...");
        // establish connection here
        var connected = true; // placeholder for connection status
        if (connected)
        {
            GameLogger.Info("Connection to server established (placeholder).");
            CliOutput.OverwriteLine("Connection successful!");
            Console.Clear();
            GameLogger.Debug("Printing initial gameboard.");
            _gameboard.PrintBoard();
            CliOutput.PrintConsoleNewline(
                "Game Started! To play a move, enter it in the format 'e2e4' to move from e2 to e4: ");
            GameLogger.Debug("Entering gameplay loop.");
            GameplayLoop();
        }
        else
        {
            GameLogger.Error("Failed to connect to server (placeholder).");
        }
    }

    private void GameplayLoop()
    {
        GameLogger.Debug("GameplayLoop started.");
        Move move;
        while (true)
        {
            GameLogger.Debug("Awaiting user move input...");
            move = InputParser.ReadMove();
            GameLogger.Debug($"Received move: {move.From} -> {move.To}");
            if (MoveValidator.ValidateMove(move, _gameboard))
            {
                GameLogger.Info($"Move validated: {move.From} -> {move.To}");
                _gameboard.Move(move);
                GameLogger.Debug("Board updated after move; rewriting board in console.");
                CliOutput.RewriteBoard(_gameboard);
            }
            else
            {
                GameLogger.Warning($"Invalid move attempted: {move.From} -> {move.To}");
                CliOutput.WriteErrorMessage("Invalid move, try again: ");
            }
        }

        return;
    }
}