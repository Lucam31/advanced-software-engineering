using Shared;
using static Shared.MoveValidator;

namespace UnitTesting.chess_client;

/// <summary>
/// Tests for checkmate and stalemate detection starting from a standard board
/// </summary>
[TestClass]
[DoNotParallelize]
public class GameLogicTests
{
    [TestInitialize]
    public void Setup()
    {
        WhitePlayer = true;
    }

    /// <summary>
    /// Plays Fool's Mate (1.f3 e5 2.g4 Qh4#) and verifies checkmate is detected
    /// </summary>
    [TestMethod]
    public void FoolsMate_CheckmateIsDetected()
    {
        var board = new Gameboard();

        // 1. f3
        board.Move(new Move("F2", "F3"));
        // 1... e5
        board.Move(new Move("E7", "E5"));
        // 2. g4
        board.Move(new Move("G2", "G4"));
        // 2... Qh4#
        board.Move(new Move("D8", "H4"));

        Assert.IsTrue(IsKingInCheck(true, board), "White king should be in check");
        Assert.IsTrue(IsCheckmate(true, board), "White king should be in checkmate");
    }

    /// <summary>
    /// Plays the shortest known stalemate and verifies stalemate is detected
    /// </summary>
    [TestMethod]
    public void ShortestStalemate_StalemateIsDetected()
    {
        var board = new Gameboard();

        // 1. e3 a5
        board.Move(new Move("E2", "E3"));
        board.Move(new Move("A7", "A5"));
        // 2. Qh5 Ra6
        board.Move(new Move("D1", "H5"));
        board.Move(new Move("A8", "A6"));
        // 3. Qxa5 h5
        board.Move(new Move("H5", "A5"));
        board.Move(new Move("H7", "H5"));
        // 4. h4 Rah6
        board.Move(new Move("H2", "H4"));
        board.Move(new Move("A6", "H6"));
        // 5. Qxc7 f6
        board.Move(new Move("A5", "C7"));
        board.Move(new Move("F7", "F6"));
        // 6. Qxd7+ Kf7
        board.Move(new Move("C7", "D7"));
        board.Move(new Move("E8", "F7"));
        // 7. Qxb7 Qd3
        board.Move(new Move("D7", "B7"));
        board.Move(new Move("D8", "D3"));
        // 8. Qxb8 Qh7
        board.Move(new Move("B7", "B8"));
        board.Move(new Move("D3", "H7"));
        // 9. Qxc8 Kg6
        board.Move(new Move("B8", "C8"));
        board.Move(new Move("F7", "G6"));
        // 10. Qe6
        board.Move(new Move("C8", "E6"));


        Assert.IsFalse(IsKingInCheck(false, board), "Black king should not be in check");
        Assert.IsTrue(IsStalemate(false, board), "Black should be in stalemate");
    }
}
