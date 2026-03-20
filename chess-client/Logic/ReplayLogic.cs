using Shared;
using Shared.Dtos;
using Shared.Logger;
using chess_client.UserInterface;

namespace chess_client.Logic;

/// <summary>
/// Executes an interactive replay of a previously played game.
/// </summary>
/// <param name="game">The played game containing serialized move history.</param>
public class ReplayLogic(PlayedGame game)
{
    private Gameboard _gameboard = new();

    private readonly ReplayUi _ui = new();

    /// <summary>
    /// Rebuilds the board from scratch and reapplies moves up to the given index.
    /// </summary>
    /// <param name="moves">The ordered move list in coordinate notation.</param>
    /// <param name="targetIndex">The exclusive move index to replay to.</param>
    private void ReplayUpToMove(List<string> moves, int targetIndex)
    {
        _gameboard = new Gameboard();
        for (var i = 0; i < targetIndex; i++)
        {
            var moveStr = moves[i];
            var move = new Move(moveStr[..2], moveStr[2..4]);
            var result = MoveValidator.ValidateMove(move, _gameboard);
            switch (result)
            {
                case MoveValidator.MoveValidationResult.EnPassant:
                    _gameboard.Move(move, enPassant: true);
                    break;
                case MoveValidator.MoveValidationResult.Castling:
                    _gameboard.Move(move, castling: true);
                    break;
                default:
                    _gameboard.Move(move);
                    break;
            }
        }
    }

    /// <summary>
    /// Starts the interactive replay loop.
    /// </summary>
    public void StartReplay()
    {
        var moves = game.Moves;
        var currentMoveIndex = 0;

        string? currentMessage = null;
        var isErrorMessage = false;

        while (true)
        {
            _ui.DrawScreen(_gameboard, currentMoveIndex, moves.Count, currentMessage, isErrorMessage);

            currentMessage = null;
            isErrorMessage = false;

            var input = BaseMenuUi.ReadKey();

            switch (input.Key)
            {
                case ConsoleKey.Q:
                case ConsoleKey.Escape:
                    return;

                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    if (currentMoveIndex == 0)
                    {
                        currentMessage = "Already at the first move.";
                        isErrorMessage = true;
                    }
                    else
                    {
                        currentMoveIndex--;
                        ReplayUpToMove(moves, currentMoveIndex);
                    }

                    break;

                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    if (currentMoveIndex >= moves.Count)
                    {
                        currentMessage = "Already at the final move.";
                        isErrorMessage = true;
                    }
                    else
                    {
                        ApplyMove(moves[currentMoveIndex]);
                        currentMoveIndex++;
                    }

                    break;

                default:
                    currentMessage = $"Invalid input '{input.KeyChar}'. Use A, D, or Q.";
                    isErrorMessage = true;
                    break;
            }
        }
    }

    /// <summary>
    /// Validates and applies one move string to the current board state.
    /// </summary>
    /// <param name="moveToPlay">The move string in source-target format (for example, <c>e2e4</c>).</param>
    private void ApplyMove(string moveToPlay)
    {
        var move = new Move(moveToPlay[..2], moveToPlay[2..4]);
        var result = MoveValidator.ValidateMove(move, _gameboard);

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