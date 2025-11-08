using Shared;
using Shared.Pieces;

namespace UnitTesting.Shared;

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Unit tests for the <see cref="Gameboard"/> class
/// The tests simulate console in and outputs using <see cref="Console.SetIn(TextReader)"/>
/// and <see cref="Console.SetOut(TextWriter)"/>
/// </summary>
[DoNotParallelize]
[TestClass]
public class GameboardTests
{
    /// <summary>
    /// Saved original console input stream, restored in test cleanup
    /// </summary>
    private TextReader? _originalIn;
    private TextWriter? _originalOut;
    private readonly StringWriter _output = new StringWriter();
    private readonly Gameboard _gameboard = new Gameboard();

    /// <summary>
    /// Initializes each test by saving the current console streams and
    /// redirecting the console output to a <see cref="StringWriter"/>
    /// </summary>
    [TestInitialize]
    public void SetUp()
    {
        _originalIn = Console.In;
        _originalOut = Console.Out;
        Console.SetOut(_output);
    }

    /// <summary>
    /// Restores the original console in and outputs after each test
    /// executed automatically after every test
    /// </summary>
    [TestCleanup]
    public void TearDown()
    {
        if (_originalIn != null) Console.SetIn(_originalIn);
        if (_originalOut != null) Console.SetOut(_originalOut);
    }

    /// <summary>
    /// Verifies that <see cref="Gameboard"/> can be constructed successfully
    /// </summary>
    [TestMethod]
    public void TestGameboard_Construction()
    {
        Assert.IsNotNull(_gameboard);
    }

    /// <summary>
    /// Verifies that <see cref="Gameboard.PrintBoard()"/> outputs the expected initial board state
    /// </summary>
    [TestMethod]
    public void TestPrintBoard()
    {
        _gameboard.PrintBoard();
        var gameboardExpected = @"    A  B  C  D  E  F  G  H 
 8  ♜  ♞  ♝  ♛  ♚  ♝  ♞  ♜  8
 7  ♟  ♟  ♟  ♟  ♟  ♟  ♟  ♟  7
 6                          6
 5                          5
 4                          4
 3                          3
 2  ♟  ♟  ♟  ♟  ♟  ♟  ♟  ♟  2
 1  ♜  ♞  ♝  ♛  ♚  ♝  ♞  ♜  1
    A  B  C  D  E  F  G  H ";
        // var gameboardOutput = _output.ToString().Substring(0, gameboardExpected.Length);
        var gameboardOutput = _output.ToString();
        
        Assert.AreEqual(gameboardExpected, gameboardOutput);
    }
    
    /// <summary>
    /// Verifies that <see cref="Gameboard.GetPieceAtPosition(string)"/> can be constructed successfully
    /// </summary>
    [TestMethod]
    public void TestGameboard_GetPieceAtPosition_NullAndPiece()
    {
        var pieceAtE2 = _gameboard.GetPieceAtPosition("E2");
        var pieceAtE4 = _gameboard.GetPieceAtPosition("E4");
        
        Assert.IsNotNull(pieceAtE2);
        Assert.AreEqual("Pawn", pieceAtE2?.GetType().Name);
        Assert.IsNull(pieceAtE4);
    }
    
    /// <summary>
    /// Verifies that <see cref="Gameboard.Move(Move)"/> returns true
    /// the move will be valid in the main code so it won't be validated here
    /// </summary>
    [TestMethod]
    public void TestGameboard_Move_ReturnsTrue()
    {
        var move = new Move("E2", "E4");
        var moveResult = _gameboard.Move(move);
        
        Assert.IsTrue(moveResult);
    }
    
    /// <summary>
    /// Verifies that <see cref="Gameboard.GetPieceAtPosition(string)"/> can be constructed successfully
    /// </summary>
    [TestMethod]
    public void TestGameboard_GetPieceAtPosition_AfterMove()
    {
        var pieceAtE2Before = _gameboard.GetPieceAtPosition("E2");
        Assert.IsNotNull(pieceAtE2Before);
        var pieceAtE4Before = _gameboard.GetPieceAtPosition("E4");
        Assert.IsNull(pieceAtE4Before);
        
        var move = new Move("E2", "E4");
        _gameboard.Move(move);
        
        var pieceAtE2After = _gameboard.GetPieceAtPosition("E2");
        Assert.IsNull(pieceAtE2After);
        var pieceAtE4After = _gameboard.GetPieceAtPosition("E4");
        Assert.IsNotNull(pieceAtE4After);
    }
    
    // /// <summary>
    // /// Verifies that <see cref="Gameboard.UndoLastMove(int)"/> can be constructed successfully
    // /// </summary>
    // [TestMethod]
    // public void TestGameboard_UndoLastMove()
    // {
    //     var pieceAtE2Before = _gameboard.GetPieceAtPosition("E2");
    //     Assert.IsNotNull(pieceAtE2Before);
    //     var pieceAtE4Before = _gameboard.GetPieceAtPosition("E4");
    //     Assert.IsNull(pieceAtE4Before);
    //     
    //     var move = new Move("E2", "E4");
    //     _gameboard.Move(move);
    //     
    //     var pieceAtE2After = _gameboard.GetPieceAtPosition("E2");
    //     Assert.IsNull(pieceAtE2After);
    //     var pieceAtE4After = _gameboard.GetPieceAtPosition("E4");
    //     Assert.IsNotNull(pieceAtE4After);
    //     
    //     var undoCount = _gameboard.UndoLastMove(2);
    //     Assert.AreEqual(1, undoCount);
    //     
    //     var pieceAtE2AfterUndo = _gameboard.GetPieceAtPosition("E2");
    //     Assert.IsNotNull(pieceAtE2AfterUndo);
    //     var pieceAtE4AfterUndo = _gameboard.GetPieceAtPosition("E4");
    //     Assert.IsNull(pieceAtE4AfterUndo);
    // }
}