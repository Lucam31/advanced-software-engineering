using chess_client.States;
using chess_client.UserInterface;
using Shared.Logger;
using Shared;
using Shared.WebSocketMessages;

namespace chess_client;

public class GameLogic
{
    private Gameboard _gameboard;
    private readonly GameUi _gameUi;
    private bool _isWhiteTurn;
    private bool _gameOver;
    private bool _isWhite;
    private readonly JsonParser _parser = new();

    public GameLogic()
    {
        _gameboard = new Gameboard();
        _gameUi = new GameUi();
        _isWhiteTurn = true;
        _gameOver = false;
        _isWhite = true;
    }

    public GameLogic(Gameboard gameboard)
    {
        _gameboard = gameboard;
        _gameUi = new GameUi();
        _isWhiteTurn = true;
        _gameOver = false;
        _isWhite = true;
    }

    public async Task StartGame(WebSocketService webSocketService, StartGamePayload startGamePayload)
    {
        var test = false;

        GameplayState? gameplayState = null;
        var gameId = Guid.Empty;

        if (test)
        {
            _isWhite = true;
            _gameUi.WhitePlayerName = "Local Player (White)";
            _gameUi.BlackPlayerName = "Local Player (Black)";
            GameLogger.Info("Test mode enabled. Skipping server communication.");
        }
        else
        {
            gameplayState = new GameplayState();
            webSocketService.TransitionTo(gameplayState);

            gameId = startGamePayload.GameId;
            _isWhite = startGamePayload.Color.Equals("white", StringComparison.OrdinalIgnoreCase);

            _gameUi.WhitePlayerName = _isWhite ? "You" : "Opponent";
            _gameUi.BlackPlayerName = !_isWhite ? "You" : "Opponent";
            GameLogger.Info($"Game {gameId} started. Playing as {startGamePayload.Color}.");
        }

        _gameboard = new Gameboard();
        _isWhiteTurn = true;
        _gameOver = false;
        var isMyTurn = _isWhite;

        while (!_gameOver)
        {
            var currentPlayer = _isWhiteTurn ? "White" : "Black";
            GameLogger.Info($"New turn started. Current player: {currentPlayer}");
            _gameUi.StatusMessage = "Your turn...";

            if (test || isMyTurn)
            {
                var currentIsWhite = test ? _isWhiteTurn : _isWhite;

                if (MoveValidator.IsKingInCheck(currentIsWhite, _gameboard))
                {
                    _gameUi.StatusMessage = "CHECK! Protect your King!";
                }

                Move move;
                MoveValidator.MoveValidationResult validationResult;

                _gameUi.PromptMessage = "Enter your move (e.g. E2E4): ";
                _gameUi.ErrorMessage = ""; // Alten Fehler löschen
                DrawBoard();

                while (true)
                {
                    try
                    {
                        move = InputParser.ReadMove();
                    }
                    catch (Exception)
                    {
                        _gameUi.ErrorMessage = "Your input is invalid. Enter your move using the format E2E4.";
                        DrawBoard();
                        continue;
                    }

                    var piece = _gameboard.GetPieceAtPosition(move.From);
                    if (piece == null)
                    {
                        _gameUi.ErrorMessage = "Your input is invalid. No piece at the selected position. Try again.";
                        DrawBoard();
                        continue;
                    }

                    if (!test && piece.IsWhite != _isWhite)
                    {
                        _gameUi.ErrorMessage = "Your input is invalid. You can only move your own pieces. Try again.";
                        DrawBoard();
                        continue;
                    }

                    if (test && piece.IsWhite != _isWhiteTurn)
                    {
                        _gameUi.ErrorMessage = "Your input is invalid. You can only move your own pieces. Try again.";
                        DrawBoard();
                        continue;
                    }

                    MoveValidator.WhitePlayer = test ? _isWhiteTurn : _isWhite;
                    validationResult = MoveValidator.ValidateMove(move, _gameboard);

                    if (validationResult == MoveValidator.MoveValidationResult.Invalid)
                    {
                        _gameUi.ErrorMessage = "Your move is invalid. Try again.";
                        DrawBoard();
                        continue;
                    }

                    if (!MoveValidator.TryMoveEscapesCheck(move, validationResult, currentIsWhite, _gameboard))
                    {
                        _gameUi.ErrorMessage = "This move leaves your king in check. Try again.";
                        DrawBoard();
                        continue;
                    }

                    break;
                }

                // Eingabefeld für die Verarbeitung wieder säubern
                _gameUi.PromptMessage = "";
                _gameUi.ErrorMessage = "";

                _gameUi.AddMoveToHistory(move.From, move.To, currentIsWhite);
                ExecuteMove(move, validationResult);

                var opponentIsWhite = test ? !_isWhiteTurn : !_isWhite;
                if (MoveValidator.IsCheckmate(opponentIsWhite, _gameboard))
                {
                    var gameFinishedMessage = new WebSocketMessage
                    {
                        Type = MessageType.GameOver,
                        Payload = _parser.SerializeToJsonElement(new GameOverPayload
                        {
                            GameId = gameId,
                            Winner = _isWhite ? "White" : "Black",
                        })
                    };

                    await webSocketService.SendAsync(gameFinishedMessage);
                    var gameOverMessage = await gameplayState!.WaitForGameOverAsync();

                    _gameUi.StatusMessage = $"CHECKMATE! {gameOverMessage.Winner} wins!";
                    GameLogger.Info($"Checkmate detected. {gameOverMessage.Winner} wins.");

                    _gameUi.PromptMessage = "Press ENTER to return...";
                    DrawBoard(); 
                    Console.ReadLine();

                    _gameOver = true;
                    break;
                }

                if (test)
                {
                    _isWhiteTurn = !_isWhiteTurn;
                }
                else
                {
                    var turnPayload = new GameTurnPayload
                    {
                        GameId = gameId,
                        CurrentBoard = _gameboard.ToDto(),
                        LastMove = $"{move.From}{move.To}"
                    };

                    await webSocketService.SendAsync(new WebSocketMessage
                    {
                        Type = MessageType.GameTurn,
                        Payload = _parser.SerializeToJsonElement(turnPayload)
                    });

                    GameLogger.Info($"Sent move {move.From}{move.To} to server.");
                    isMyTurn = false;
                }
            }
            else
            {
                _gameUi.StatusMessage = "Waiting for opponent...";
                _gameUi.PromptMessage = ""; // Kein Input vom Nutzer erwartet
                DrawBoard();

                var opponentTurnTask = gameplayState!.WaitForOpponentTurnAsync();
                var gameOverTask = gameplayState!.WaitForGameOverAsync();

                var completedTask = await Task.WhenAny(opponentTurnTask, gameOverTask);

                if (completedTask == gameOverTask)
                {
                    var gameOverMessage = await gameOverTask;
                    _gameUi.StatusMessage = $"CHECKMATE! {gameOverMessage.Winner} wins!";
                    GameLogger.Info($"Opponent caused checkmate. {gameOverMessage.Winner} wins.");

                    _gameUi.PromptMessage = "Press ENTER to return...";
                    DrawBoard();
                    Console.ReadLine();

                    _gameOver = true;
                    break;
                }

                var opponentTurn = await opponentTurnTask;
                _gameboard = Gameboard.FromDto(opponentTurn.CurrentBoard);

                var fromMove = opponentTurn.LastMove[..2];
                var toMove = opponentTurn.LastMove.Substring(2, 2);
                _gameUi.AddMoveToHistory(fromMove, toMove, !_isWhite);

                GameLogger.Info($"Received opponent move: {opponentTurn.LastMove}");

                isMyTurn = true;
            }
        }
        
        GameLogger.Info("Game ended.");
    }

    private void DrawBoard()
    {
        _gameUi.DrawGameScreen(_gameboard, _isWhite);
    }

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