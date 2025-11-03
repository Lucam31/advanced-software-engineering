using Shared;

namespace chess_client;

public class ReplayHandler(Gameboard gameboard)
{
    readonly Gameboard _gameboard = gameboard;
    
    public void StartReplay(string[] moveList)
    {
        string[] moves = moveList;
        int currentMoveIndex = 0;
        CLIOutput.PrintConsoleNewline("Starting Replay...");
        Console.Clear();
        _gameboard.PrintBoard();
        // CLIOutput.PrintConsoleNewline($"Move {currentMoveIndex}/{moves.Length}");
        Console.WriteLine($"Move {currentMoveIndex}/{moves.Length}");
        Console.WriteLine("test");
        string userInput;
        while (currentMoveIndex < moves.Length)
        {
            Console.SetCursorPosition(0, Console.CursorTop-1);
            CLIOutput.ClearCurrentConsoleLine();
            // new method in InputParser to read single character input with validation???
            userInput = InputParser.ReadInput("Revert last Move (A), Next Move (D), (Q)uit: ", false, ["A", "D", "Q"]);
            switch (userInput)
            {
                case "Q":
                    CLIOutput.PrintConsoleNewline("Exiting Replay...");
                    return;
                case "A":
                    if (currentMoveIndex == 0)
                    {
                        CLIOutput.PrintConsoleNewline("No moves to revert.");
                        continue;
                    }

                    currentMoveIndex--;
                    // add UndoMove method to Gameboard?? captured pieces need to reappear
                    // pass in last move mirrored?
                    // what about special moves like castling, en passant, promotion?
                    // _gameboard.UndoMove();
                    CLIOutput.RewriteBoard(_gameboard);
                    CLIOutput.OverwriteLineRelative(2, $"Reverted to Move {currentMoveIndex}/{moves.Length}");
                    continue;
                case "D":
                    var moveToPlay = moves[currentMoveIndex];
                    _gameboard.Move(new Move(moveToPlay[..2], moveToPlay[2..4]));
                    currentMoveIndex++;
                    CLIOutput.RewriteBoard(_gameboard);
                    CLIOutput.OverwriteLineRelative(2, $"Move {currentMoveIndex}/{moves.Length}");
                    continue;
            }
        }
        CLIOutput.PrintConsoleNewline("Replay Finished.");
    }
}