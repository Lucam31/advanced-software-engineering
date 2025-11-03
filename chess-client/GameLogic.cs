using Shared.Logger;

namespace chess_client;

using Shared;

/// <summary>
/// Manages the core logic of the chess game.
/// </summary>
public class GameLogic
{
    private readonly Gameboard _gameboard;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameLogic"/> class.
    /// </summary>
    /// <param name="gameboard">The game board to be used.</param>
    public GameLogic(Gameboard gameboard)
    {
        _gameboard = gameboard;
        GameLogger.Debug("GameLogic initialized with Gameboard instance.");
    }

    /// <summary>
    /// Starts a new game, including connecting to the server and entering the main gameplay loop.
    /// </summary>
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

    /// <summary>
    /// The main loop for gameplay, handling user input and move validation.
    /// </summary>
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