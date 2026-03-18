using Shared;
using Shared.Dtos;
using Shared.Logger;

namespace chess_client;

public class ReplayLogic
{
    readonly PlayedGame _game;
    Gameboard _gameboard;
    
    public ReplayLogic(PlayedGame game)
    {
        _game = game;
        _gameboard = new Gameboard();
    }

    /// <summary>
    /// Rebuilds the board from scratch and replays all moves up to the given index
    /// </summary>
    /// <param name="moves">The list of move strings</param>
    /// <param name="targetIndex">The number of moves to replay</param>
    private void ReplayUpToMove(List<string> moves, int targetIndex)
    {
        _gameboard = new Gameboard();
        for (int i = 0; i < targetIndex; i++)
        {
            var moveStr = moves[i];
            Move move = new Move(moveStr[..2], moveStr[2..4]);
            MoveValidator.MoveValidationResult result = MoveValidator.ValidateMove(move, _gameboard);
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

    
    public void StartReplay()
    {
        List<string> moves = _game.Moves;
        int currentMoveIndex = 0;
        ConsoleHelper.PrintConsoleNewline("Starting Replay...");
        Console.Clear();
        _gameboard.PrintBoard();
        // CLIOutput.PrintConsoleNewline($"Move {currentMoveIndex}/{moves.Length}");
        ConsoleHelper.PrintConsoleNewline($"Move {currentMoveIndex}/{moves.Count}");
        // new method in InputParser to read single character input with validation???
        ConsoleHelper.PrintConsoleNewline("Revert last Move (A), Next Move (D), (Q)uit.");
        ConsoleHelper.PrintConsoleNewline("Enter your choice: ");
        while (true)
        {
            var userInput = Console.ReadKey();
            switch (userInput.KeyChar.ToString().ToUpper())
            {
                case "Q":
                    ConsoleHelper.PrintConsoleNewline("Exiting Replay...");
                    Console.Clear();
                    return;
                case "A":
                    if (currentMoveIndex == 0)
                    {
                        ConsoleHelper.OverwriteLine("No moves to revert: ");
                        continue;
                    }

                    currentMoveIndex--;
                    ReplayUpToMove(moves, currentMoveIndex);
                    ConsoleHelper.RewriteBoard(_gameboard);
                    ConsoleHelper.OverwriteLineRelative(2, $"Move {currentMoveIndex}/{moves.Count}");
                    ConsoleHelper.OverwriteLine("Reverted last move: ");
                    continue;
                case "D":
                    if (currentMoveIndex >= moves.Count)
                    {
                        ConsoleHelper.OverwriteLine("No more moves to play: ");
                        continue;
                    }
                    var moveToPlay = moves[currentMoveIndex];
                    Move move = new Move(moveToPlay[..2], moveToPlay[2..4]);
                    MoveValidator.MoveValidationResult result = MoveValidator.ValidateMove(move, _gameboard);
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
                    currentMoveIndex++;
                    ConsoleHelper.RewriteBoard(_gameboard);
                    ConsoleHelper.OverwriteLineRelative(2, $"Move {currentMoveIndex}/{moves.Count}");
                    ConsoleHelper.OverwriteLine("Enter your choice: ");
                    continue;
                default:
                    ConsoleHelper.OverwriteLine($"{userInput.KeyChar} is not a valid choice: ");
                    continue;
            }
        }
        ConsoleHelper.PrintConsoleNewline("Replay Finished. Press any key to return to the replay menu: ");
        Console.ReadKey();
        Console.Clear();
    }
    
}