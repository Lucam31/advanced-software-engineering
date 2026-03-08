using System.Text.Json;
using chess_client.States;
using Shared.Logger;
using Shared;
using Shared.WebSocketMessages;

namespace chess_client;

/// <summary>
/// Manages the core logic of the chess game
/// </summary>
public class GameLogic
{
    private Gameboard _gameboard;
    private readonly GameStats _gameStats;
    private bool _isWhiteTurn;
    private bool _gameOver;
    private bool _isWhite;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameLogic"/> class with a fresh gameboard
    /// </summary>
    public GameLogic()
    {
        _gameboard = new Gameboard();
        _gameStats = new GameStats();
        _isWhiteTurn = true;
        _gameOver = false;
        _isWhite = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameLogic"/> class with an existing gameboard
    /// </summary>
    /// <param name="gameboard">The gameboard to use</param>
    public GameLogic(Gameboard gameboard)
    {
        _gameboard = gameboard;
        _gameStats = new GameStats();
        _isWhiteTurn = true;
        _gameOver = false;
        _isWhite = true;
    }

    /// <summary>
    /// Starts a new online game by sending a CreateGame message and running the game loop
    /// </summary>
    /// <param name="webSocketService">The WebSocket service for server communication</param>
    public async Task StartGame(WebSocketService webSocketService)
    {
        // Set to true to play locally without server communication
        var test = true;

        GameplayState? gameplayState = null;
        var gameId = Guid.Empty;

        if (test)
        {
            _isWhite = true;
            _gameStats.WhitePlayerName = "Local Player (White) (temp)";
            _gameStats.BlackPlayerName = "Local Player (Black) (temp)";
            CliOutput.PrintConsoleNewline("Test mode: starting local game as white.");
            GameLogger.Info("Test mode enabled. Skipping server communication.");
        }
        else
        {
            // Work in progress, add server communication to create game and receive game start info
            // create GameplayState and transition to it
            gameplayState = new GameplayState();
            webSocketService.TransitionTo(gameplayState);

            // Send CreateGame message to the server
            // not sure if these are set correctly yet, any more data needed?
            var createGamePayload = new CreateGamePayload { };
            await webSocketService.SendAsync(new WebSocketMessage
            {
                Type = MessageType.CreateGame,
                Payload = JsonSerializer.SerializeToElement(createGamePayload)
            });

            CliOutput.PrintConsoleNewline("Waiting for game to start...");

            // Wait for the server to respond with StartGame
            var startPayload = await gameplayState.WaitForGameStartAsync();
            gameId = startPayload.GameId;
            _isWhite = startPayload.Color.Equals("white", StringComparison.OrdinalIgnoreCase);

            _gameStats.WhitePlayerName = _isWhite ? "You" : "Opponent";
            _gameStats.BlackPlayerName = !_isWhite ? "You" : "Opponent";

            CliOutput.PrintConsoleNewline($"Game started! You are playing as {startPayload.Color}.");
            GameLogger.Info($"Game {gameId} started. Playing as {startPayload.Color}.");
        }

        // Initialize the gameboard and run the game loop
        _gameboard = new Gameboard();
        _isWhiteTurn = true;
        _gameOver = false;
        var isMyTurn = _isWhite; // White always moves first

        while (!_gameOver)
        {
            var currentPlayer = _isWhiteTurn ? "White" : "Black";
            _gameStats.StatusMessage = $"Waiting for {currentPlayer}...";

            DrawBoard();

            if (test || isMyTurn)
            {
                // Check if the current player's king is in checkmate or check
                var currentIsWhite = test ? _isWhiteTurn : _isWhite;

                // Checkmate Check
                if (MoveValidator.IsCheckmate(currentIsWhite, _gameboard))
                {
                    var winner = currentIsWhite ? "Black" : "White";
                    _gameStats.StatusMessage = $"CHECKMATE! {winner} wins!";
                    DrawBoard();
                    GameLogger.Info($"Checkmate detected. {winner} wins.");
                    _gameOver = true;
                    break;
                }

                // Stalemate Check
                if (MoveValidator.IsStalemate(currentIsWhite, _gameboard))
                {
                    _gameStats.StatusMessage = "STALEMATE! No one wins!";
                    DrawBoard();
                    GameLogger.Info("Stalemate detected. No one wins.");
                    _gameOver = true;
                    break;
                }

                // Check Check
                if (MoveValidator.IsKingInCheck(currentIsWhite, _gameboard))
                {
                    _gameStats.StatusMessage = "CHECK! Protect your King!";
                    DrawBoard();
                }

                Move move;
                MoveValidator.MoveValidationResult validationResult;

                CliOutput.PrintConsoleNewline("Your turn. Enter your move (e.g. E2E4): ");
                while (true)
                {
                    try
                    {
                        move = InputParser.ReadMove();
                    }
                    catch (Exception)
                    {
                        CliOutput.OverwriteLineRelativeKeepCursorAtEnd(1,
                            "Your input is invalid. Enter your move using the format E2E4: ");
                        continue;
                    }

                    var piece = _gameboard.GetPieceAtPosition(move.From);
                    if (piece == null)
                    {
                        CliOutput.OverwriteLineRelativeKeepCursorAtEnd(1,
                            "Your input is invalid. No piece at the selected position. Try again: ");
                        continue;
                    }

                    if (!test && piece.IsWhite != _isWhite)
                    {
                        CliOutput.OverwriteLineRelativeKeepCursorAtEnd(1,
                            "Your input is invalid. You can only move your own pieces. Try again: ");
                        continue;
                    }

                    if (test && piece.IsWhite != _isWhiteTurn)
                    {
                        CliOutput.OverwriteLineRelativeKeepCursorAtEnd(1,
                            "Your input is invalid. You can only move your own pieces. Try again: ");
                        continue;
                    }

                    MoveValidator.WhitePlayer = test ? _isWhiteTurn : _isWhite;

                    validationResult = MoveValidator.ValidateMove(move, _gameboard);

                    if (validationResult == MoveValidator.MoveValidationResult.Invalid)
                    {
                        CliOutput.OverwriteLineRelativeKeepCursorAtEnd(1, "Your move is invalid. Try again: ");
                        continue;
                    }

                    // Verify the move does not leave the own king in check
                    if (!MoveValidator.TryMoveEscapesCheck(move, validationResult, currentIsWhite, _gameboard))
                    {
                        CliOutput.OverwriteLineRelativeKeepCursorAtEnd(1,
                            "This move leaves your king in check. Try again: ");
                        continue;
                    }

                    break;
                }

                // Save move to the history
                _gameStats.AddMoveToHistory(move.From, move.To, currentIsWhite);

                // Execute the move locally
                ExecuteMove(move, validationResult);

                // Check if the opponent is in checkmate
                var opponentIsWhite = test ? !_isWhiteTurn : !_isWhite;
                if (MoveValidator.IsCheckmate(opponentIsWhite, _gameboard))
                {
                    var winner = opponentIsWhite ? "Black" : "White";
                    _gameStats.StatusMessage = $"CHECKMATE! {winner} wins!";
                    GameLogger.Info($"Checkmate detected. {winner} wins.");
                    _gameOver = true;
                }

                if (test)
                {
                    // In test mode just alternate turns locally
                    _isWhiteTurn = !_isWhiteTurn;
                }
                else
                {
                    // Send the updated board to the server
                    var turnPayload = new GameTurnPayload
                    {
                        GameId = gameId,
                        CurrentBoard = _gameboard.ToDto(),
                        LastMove = $"{move.From}{move.To}"
                    };

                    await webSocketService.SendAsync(new WebSocketMessage
                    {
                        Type = MessageType.GameTurn,
                        Payload = JsonSerializer.SerializeToElement(turnPayload)
                    });

                    GameLogger.Info($"Sent move {move.From}{move.To} to server.");
                    isMyTurn = false;
                }
            }
            else
            {
                // Opponent's turn: wait for server response
                _gameStats.StatusMessage = "Waiting for opponent's move...";
                DrawBoard();

                var opponentTurn = await gameplayState!.WaitForOpponentTurnAsync();
                _gameboard = Gameboard.FromDto(opponentTurn.CurrentBoard);

                var fromMove = opponentTurn.LastMove[..2];
                var toMove = opponentTurn.LastMove.Substring(2, 2);
                _gameStats.AddMoveToHistory(fromMove, toMove, !_isWhite);

                GameLogger.Info($"Received opponent move: {opponentTurn.LastMove}");

                isMyTurn = true;
            }
        }

        _gameStats.StatusMessage = "Game Over!";
        DrawBoard();

        CliOutput.PrintConsoleNewline("Game over! Returning to main menu...");
        GameLogger.Info("Game ended.");
    }

    /// <summary>
    /// Nutzt die GameUi-Klasse, um das Spielbrett und das Dashboard zu zeichnen.
    /// </summary>
    private void DrawBoard()
    {
        GameUi.DrawGameScreen(_gameboard, _gameStats, _isWhite);
    }

    /// <summary>
    /// Executes a validated move on the gameboard, handling special moves like en passant and castling
    /// </summary>
    /// <param name="move">The move to execute</param>
    /// <param name="result">The validation result indicating the type of move</param>
    private void ExecuteMove(Move move, MoveValidator.MoveValidationResult result)
    {
        switch (result)
        {
            case MoveValidator.MoveValidationResult.Valid:
            case MoveValidator.MoveValidationResult.Check:
            case MoveValidator.MoveValidationResult.Checkmate:
                _gameboard.Move(move);
                GameLogger.Info($"Move executed: {move}");
                break;

            case MoveValidator.MoveValidationResult.EnPassant:
                _gameboard.Move(move, enPassant: true);
                GameLogger.Info($"En passant executed: {move}");
                break;

            case MoveValidator.MoveValidationResult.Castling:
                _gameboard.Move(move, castling: true);
                GameLogger.Info($"Castling executed: {move}");
                break;

            default:
                GameLogger.Warning($"Unexpected validation result: {result}");
                break;
        }
    }
}