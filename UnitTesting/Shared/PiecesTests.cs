using Shared;
using Shared.Pieces;

namespace UnitTesting.Shared;

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Unit tests for the <see cref="Piece"/> class and derived classes
/// The tests simulate console in and outputs using <see cref="Console.SetIn(TextReader)"/>
/// and <see cref="Console.SetOut(TextWriter)"/>
/// </summary>
[DoNotParallelize]
[TestClass]
public class PiecesTests
{
    /// <summary>
    /// Saved original console input stream, restored in test cleanup
    /// </summary>
    private TextReader? _originalIn;
    private TextWriter? _originalOut;

    /// <summary>
    /// Initializes each test by saving the current console streams and
    /// redirecting the console output to a <see cref="StringWriter"/>
    /// </summary>
    [TestInitialize]
    public void SetUp()
    {
        _originalIn = Console.In;
        _originalOut = Console.Out;
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
    /// Verifies that <see cref="Piece.Move(string)"/> updates position and moved status
    /// </summary>
    [TestMethod]
    public void TestMove_UpdatedVariables()
    {
        Piece piece = new Pawn("E2", true);
        Assert.AreEqual("E2", piece.Position);
        Assert.IsFalse(piece.Moved);
        
        piece.Move("E4");
        Assert.AreEqual("E4", piece.Position);
        Assert.IsTrue(piece.Moved);
    }
    
    /// <summary>
    /// Verifies that <see cref="Piece.ToString()"/> returns the correct
    /// Unicode symbol for each piece type
    /// </summary>
    [TestMethod]
    public void TestToString_ReturnsCorrectSymbol()
    {
        var pawn = new Pawn("E2", true);
        var rook = new Rook("A1", true);
        var knight = new Knight("B1", true);
        var bishop = new Bishop("C1", true);
        var queen = new Queen("D1", true);
        var king = new King("E1", true);
        
        Assert.AreEqual("♟", pawn.ToString());
        Assert.AreEqual("♜", rook.ToString());
        Assert.AreEqual("♞", knight.ToString());
        Assert.AreEqual("♝", bishop.ToString());
        Assert.AreEqual("♛", queen.ToString());
        Assert.AreEqual("♚", king.ToString());
    }
    
}