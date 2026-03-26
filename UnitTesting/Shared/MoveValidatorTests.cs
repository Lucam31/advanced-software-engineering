using Shared;
using Shared.Pieces;
using static Shared.MoveValidator;
using static Shared.MoveValidator.MoveValidationResult;

namespace UnitTesting.Shared;

/// <summary>
/// Tests for the MoveValidator class covering standard piece moves,
/// captures, and special moves (en passant, castling)
/// </summary>
[TestClass]
[DoNotParallelize]
public class MoveValidatorTests
{
    /// <summary>
    /// Creates an empty 8x8 gameboard with no pieces
    /// </summary>
    private static Gameboard CreateEmptyBoard()
    {
        var board = new Gameboard();
        // Clear all pieces from the default board
        for (var row = 0; row < 8; row++)
        {
            for (var col = 0; col < 8; col++)
            {
                board[row, col] = new Tile(row, col, (row + col) % 2 == 0);
            }
        }
        return board;
    }

    /// <summary>
    /// Places a piece on the board at the given position (e.g. "E4")
    /// </summary>
    private static void PlacePiece(Gameboard board, Piece piece, string position)
    {
        var col = position[0] - 'A';
        var row = position[1] - '1';
        board[row, col] = new Tile(row, col, (row + col) % 2 == 0, piece);
    }

    [TestInitialize]
    public void Setup()
    {
        WhitePlayer = true;
    }

    // =====================================================================
    // General Validation
    // =====================================================================

    /// <summary>
    /// Checks that moving to the same position is invalid
    /// </summary>
    [TestMethod]
    public void MoveToSamePosition_IsInvalid()
    {
        // Arrange
        var board = new Gameboard();
        var move = new Move("E2", "E2");

        // Act & Assert
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that capturing own piece is invalid
    /// </summary>
    [TestMethod]
    public void CaptureOwnPiece_IsInvalid()
    {
        // Arrange
        var board = new Gameboard();
        // White rook at A1 trying to move to white knight at B1
        var move = new Move("A1", "B1");

        // Act & Assert
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    // =====================================================================
    // Pawn Tests
    // =====================================================================

    /// <summary>
    /// Checks that a pawn can move one square forward
    /// </summary>
    [TestMethod]
    public void Pawn_SingleForward_IsValid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Pawn("E2", true), "E2");

        // Act & Assert
        var move = new Move("E2", "E3");
        Assert.AreEqual(Valid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a pawn can move two squares forward from its starting position
    /// </summary>
    [TestMethod]
    public void Pawn_DoubleForwardFromStart_IsValid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Pawn("E2", true), "E2");

        // Act & Assert
        var move = new Move("E2", "E4");
        Assert.AreEqual(Valid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a pawn cannot move two squares forward after it has moved
    /// </summary>
    [TestMethod]
    public void Pawn_DoubleForwardAfterMoved_IsInvalid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        var pawn = new Pawn("E3", true);
        pawn.Move("E3"); // Mark as moved
        PlacePiece(board, pawn, "E3");

        // Act & Assert
        var move = new Move("E3", "E5");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a pawn cannot move forward if blocked
    /// </summary>
    [TestMethod]
    public void Pawn_ForwardBlocked_IsInvalid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Pawn("E2", true), "E2");
        PlacePiece(board, new Pawn("E3", false), "E3");

        // Act & Assert
        var move = new Move("E2", "E3");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a pawn cannot move two squares forward if blocked
    /// </summary>
    [TestMethod]
    public void Pawn_DoubleForwardBlocked_IsInvalid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Pawn("E2", true), "E2");
        PlacePiece(board, new Pawn("E3", false), "E3");

        // Act & Assert
        var move = new Move("E2", "E4");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a pawn can capture diagonally
    /// </summary>
    [TestMethod]
    public void Pawn_DiagonalCapture_IsValid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Pawn("E4", true), "E4");
        PlacePiece(board, new Pawn("D5", false), "D5");

        var pawn = board.GetPieceAtPosition("E4")!;
        pawn.Move("E4"); // Mark as moved

        // Act & Assert
        var move = new Move("E4", "D5");
        Assert.AreEqual(Valid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a pawn cannot move diagonally without capturing
    /// </summary>
    [TestMethod]
    public void Pawn_DiagonalWithoutCapture_IsInvalid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        var pawn = new Pawn("E4", true);
        pawn.Move("E4");
        PlacePiece(board, pawn, "E4");

        // No piece at D5 to capture — diagonal without capture is invalid
        // Act & Assert
        var move = new Move("E4", "D5");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a pawn cannot move backward
    /// </summary>
    [TestMethod]
    public void Pawn_Backward_IsInvalid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        var pawn = new Pawn("E4", true);
        pawn.Move("E4");
        PlacePiece(board, pawn, "E4");

        // Act & Assert
        var move = new Move("E4", "E3");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a pawn cannot move sideways
    /// </summary>
    [TestMethod]
    public void Pawn_Sideways_IsInvalid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        var pawn = new Pawn("E4", true);
        pawn.Move("E4");
        PlacePiece(board, pawn, "E4");

        // Act & Assert
        var move = new Move("E4", "D4");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    // =====================================================================
    // Rook Tests
    // =====================================================================

    /// <summary>
    /// Checks that a rook can move vertically
    /// </summary>
    [TestMethod]
    public void Rook_VerticalMove_IsValid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Rook("A1", true), "A1");

        // Act & Assert
        var move = new Move("A1", "A5");
        Assert.AreEqual(Valid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a rook can move horizontally
    /// </summary>
    [TestMethod]
    public void Rook_HorizontalMove_IsValid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Rook("A1", true), "A1");

        // Act & Assert
        var move = new Move("A1", "H1");
        Assert.AreEqual(Valid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a rook cannot move diagonally
    /// </summary>
    [TestMethod]
    public void Rook_DiagonalMove_IsInvalid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Rook("A1", true), "A1");

        // Act & Assert
        var move = new Move("A1", "C3");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a rook cannot move through its own piece
    /// </summary>
    [TestMethod]
    public void Rook_BlockedByOwnPiece_IsInvalid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Rook("A1", true), "A1");
        PlacePiece(board, new Pawn("A3", true), "A3");

        // Act & Assert
        var move = new Move("A1", "A5");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a rook can capture an opponent piece
    /// </summary>
    [TestMethod]
    public void Rook_CaptureOpponent_IsValid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Rook("A1", true), "A1");
        PlacePiece(board, new Pawn("A5", false), "A5");

        // Act & Assert
        var move = new Move("A1", "A5");
        Assert.AreEqual(Valid, ValidateMove(move, board));
    }

    // =====================================================================
    // Knight Tests
    // =====================================================================

    /// <summary>
    /// Checks that a knight can move to all valid L-shape squares
    /// </summary>
    [TestMethod]
    public void Knight_AllLShapes_AreValid()
    {
        // Arrange
        // Knight at D4 should be able to reach all 8 L-shaped squares
        string[] validTargets = ["C6", "E6", "F5", "F3", "E2", "C2", "B3", "B5"];

        // Act & Assert
        foreach (var target in validTargets)
        {
            var board = CreateEmptyBoard();
            PlacePiece(board, new Knight("D4", true), "D4");

            var move = new Move("D4", target);
            Assert.AreEqual(Valid, ValidateMove(move, board),
                $"Knight D4 -> {target} should be valid");
        }
    }

    /// <summary>
    /// Checks that a knight can jump over other pieces
    /// </summary>
    [TestMethod]
    public void Knight_JumpsOverPieces_IsValid()
    {
        // Arrange
        // Start from default board: knight at B1 can jump to C3 even though B2 has a pawn
        var board = new Gameboard();

        // Act & Assert
        var move = new Move("B1", "C3");
        Assert.AreEqual(Valid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a knight cannot move straight
    /// </summary>
    [TestMethod]
    public void Knight_StraightMove_IsInvalid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Knight("D4", true), "D4");

        // Act & Assert
        var move = new Move("D4", "D6");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a knight can capture an opponent piece
    /// </summary>
    [TestMethod]
    public void Knight_CaptureOpponent_IsValid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Knight("D4", true), "D4");
        PlacePiece(board, new Pawn("C6", false), "C6");

        // Act & Assert
        var move = new Move("D4", "C6");
        Assert.AreEqual(Valid, ValidateMove(move, board));
    }

    // =====================================================================
    // Bishop Tests
    // =====================================================================

    /// <summary>
    /// Checks that a bishop can move diagonally
    /// </summary>
    [TestMethod]
    public void Bishop_DiagonalMove_IsValid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Bishop("C1", true), "C1");

        // Act & Assert
        var move = new Move("C1", "F4");
        Assert.AreEqual(Valid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a bishop cannot move straight
    /// </summary>
    [TestMethod]
    public void Bishop_StraightMove_IsInvalid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Bishop("C1", true), "C1");

        // Act & Assert
        var move = new Move("C1", "C4");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a bishop cannot move through another piece
    /// </summary>
    [TestMethod]
    public void Bishop_BlockedByPiece_IsInvalid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Bishop("A1", true), "A1");
        PlacePiece(board, new Pawn("C3", true), "C3");

        // Act & Assert
        var move = new Move("A1", "D4");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a bishop can capture an opponent piece
    /// </summary>
    [TestMethod]
    public void Bishop_CaptureOpponent_IsValid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Bishop("A1", true), "A1");
        PlacePiece(board, new Pawn("D4", false), "D4");

        // Act & Assert
        var move = new Move("A1", "D4");
        Assert.AreEqual(Valid, ValidateMove(move, board));
    }

    // =====================================================================
    // Queen Tests
    // =====================================================================

    /// <summary>
    /// Checks that a queen can move diagonally
    /// </summary>
    [TestMethod]
    public void Queen_DiagonalMove_IsValid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Queen("D1", true), "D1");

        // Act & Assert
        var move = new Move("D1", "H5");
        Assert.AreEqual(Valid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a queen can move vertically
    /// </summary>
    [TestMethod]
    public void Queen_VerticalMove_IsValid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Queen("D1", true), "D1");

        // Act & Assert
        var move = new Move("D1", "D8");
        Assert.AreEqual(Valid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a queen can move horizontally
    /// </summary>
    [TestMethod]
    public void Queen_HorizontalMove_IsValid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Queen("D4", true), "D4");

        // Act & Assert
        var move = new Move("D4", "A4");
        Assert.AreEqual(Valid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a queen cannot move in L-shape
    /// </summary>
    [TestMethod]
    public void Queen_LShapeMove_IsInvalid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Queen("D4", true), "D4");

        // Act & Assert
        var move = new Move("D4", "E6");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a queen cannot move through another piece
    /// </summary>
    [TestMethod]
    public void Queen_BlockedByPiece_IsInvalid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new Queen("A1", true), "A1");
        PlacePiece(board, new Pawn("A3", true), "A3");

        // Act & Assert
        var move = new Move("A1", "A5");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    // =====================================================================
    // King Tests
    // =====================================================================

    /// <summary>
    /// Checks that a king can move to all adjacent squares
    /// </summary>
    [TestMethod]
    public void King_AllDirections_AreValid()
    {
        // Arrange
        string[] targets = ["D5", "E5", "F5", "D4", "F4", "D3", "E3", "F3"];

        // Act & Assert
        foreach (var target in targets)
        {
            var board = CreateEmptyBoard();
            PlacePiece(board, new King("E4", true), "E4");

            var move = new Move("E4", target);
            var result = ValidateMove(move, board);
            Assert.IsTrue(result is Valid or Check,
                $"King E4 -> {target} should be valid, got {result}");
        }
    }

    /// <summary>
    /// Checks that a king cannot move two squares without castling
    /// </summary>
    [TestMethod]
    public void King_TwoSquareMove_WithoutCastling_IsInvalid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new King("E4", true), "E4");

        // Act & Assert
        var move = new Move("E4", "E6");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a king can capture an opponent piece
    /// </summary>
    [TestMethod]
    public void King_CaptureOpponent_IsValid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new King("E4", true), "E4");
        PlacePiece(board, new Pawn("E5", false), "E5");

        // Act
        var move = new Move("E4", "E5");
        var result = ValidateMove(move, board);

        // Assert
        Assert.IsTrue(result is Valid or Check,
            $"King capture should be valid, got {result}");
    }

    // =====================================================================
    // En Passant Tests
    // =====================================================================

    /// <summary>
    /// Checks that en passant capture is valid when eligible
    /// </summary>
    [TestMethod]
    public void EnPassant_ValidCapture_ReturnsEnPassant()
    {
        // Arrange
        var board = CreateEmptyBoard();

        // White pawn at E5 (moved)
        var whitePawn = new Pawn("E5", true);
        whitePawn.Move("E5");
        PlacePiece(board, whitePawn, "E5");

        // Black pawn at D5 just did a double move (en passant eligible)
        var blackPawn = new Pawn("D5", false);
        blackPawn.Move("D5");
        blackPawn.EnPassantEligible = true;
        PlacePiece(board, blackPawn, "D5");

        // Act & Assert
        var move = new Move("E5", "D6");
        Assert.AreEqual(EnPassant, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that en passant is invalid when not eligible
    /// </summary>
    [TestMethod]
    public void EnPassant_NotEligible_IsInvalid()
    {
        // Arrange
        var board = CreateEmptyBoard();

        // White pawn at E5
        var whitePawn = new Pawn("E5", true);
        whitePawn.Move("E5");
        PlacePiece(board, whitePawn, "E5");

        // Black pawn at D5, but NOT en passant eligible
        var blackPawn = new Pawn("D5", false);
        blackPawn.Move("D5");
        blackPawn.EnPassantEligible = false;
        PlacePiece(board, blackPawn, "D5");

        // No piece at D6, and en passant not eligible → invalid
        // Act & Assert
        var move = new Move("E5", "D6");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that a pawn double move sets en passant eligibility
    /// </summary>
    [TestMethod]
    public void Pawn_DoubleMove_SetsEnPassantEligible()
    {
        // Arrange
        var board = CreateEmptyBoard();
        var pawn = new Pawn("E2", true);
        PlacePiece(board, pawn, "E2");

        // Act
        var move = new Move("E2", "E4");
        var result = ValidateMove(move, board);

        // Assert
        Assert.AreEqual(Valid, result);
        Assert.IsTrue(pawn.EnPassantEligible);
    }

    // =====================================================================
    // Castling Tests
    // =====================================================================

    /// <summary>
    /// Checks that king-side castling is valid
    /// </summary>
    [TestMethod]
    public void Castling_KingSide_IsValid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new King("E1", true), "E1");
        PlacePiece(board, new Rook("H1", true), "H1");

        // Act & Assert
        var move = new Move("E1", "G1");
        Assert.AreEqual(Castling, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that queen-side castling is valid
    /// </summary>
    [TestMethod]
    public void Castling_QueenSide_IsValid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new King("E1", true), "E1");
        PlacePiece(board, new Rook("A1", true), "A1");

        // Act & Assert
        var move = new Move("E1", "C1");
        Assert.AreEqual(Castling, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that castling is invalid if the king has already moved
    /// </summary>
    [TestMethod]
    public void Castling_KingAlreadyMoved_IsInvalid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        var king = new King("E1", true);
        king.Move("E1"); // Mark as moved
        PlacePiece(board, king, "E1");
        PlacePiece(board, new Rook("H1", true), "H1");

        // Act & Assert
        var move = new Move("E1", "G1");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that castling is invalid if the rook has already moved
    /// </summary>
    [TestMethod]
    public void Castling_RookAlreadyMoved_IsInvalid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new King("E1", true), "E1");
        var rook = new Rook("H1", true);
        rook.Move("H1"); // Mark as moved
        PlacePiece(board, rook, "H1");

        // Act & Assert
        var move = new Move("E1", "G1");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that castling is invalid if the path is blocked
    /// </summary>
    [TestMethod]
    public void Castling_PathBlocked_IsInvalid()
    {
        // Arrange
        var board = CreateEmptyBoard();
        PlacePiece(board, new King("E1", true), "E1");
        PlacePiece(board, new Rook("H1", true), "H1");
        PlacePiece(board, new Bishop("F1", true), "F1"); // Blocks path

        // Act & Assert
        var move = new Move("E1", "G1");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    // =====================================================================
    // Move on Default Board
    // Tests to verify that the default board setup correctly validates common opening moves
    // =====================================================================

    /// <summary>
    /// Checks that pawn E2-E4 is valid on the default board
    /// </summary>
    [TestMethod]
    public void DefaultBoard_PawnE2E4_IsValid()
    {
        // Arrange
        var board = new Gameboard();

        // Act & Assert
        var move = new Move("E2", "E4");
        Assert.AreEqual(Valid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that pawn E2-E5 is invalid on the default board
    /// </summary>
    [TestMethod]
    public void DefaultBoard_PawnE2E5_IsInvalid()
    {
        // Arrange
        var board = new Gameboard();

        // Act & Assert
        var move = new Move("E2", "E5");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that knight B1-C3 is valid on the default board
    /// </summary>
    [TestMethod]
    public void DefaultBoard_KnightB1C3_IsValid()
    {
        // Arrange
        var board = new Gameboard();

        // Act & Assert
        var move = new Move("B1", "C3");
        Assert.AreEqual(Valid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that knight B1-A3 is valid on the default board
    /// </summary>
    [TestMethod]
    public void DefaultBoard_KnightB1A3_IsValid()
    {
        // Arrange
        var board = new Gameboard();

        // Act & Assert
        var move = new Move("B1", "A3");
        Assert.AreEqual(Valid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that bishop C1-E3 is invalid on the default board due to blocking pawn
    /// </summary>
    [TestMethod]
    public void DefaultBoard_BishopC1Blocked_IsInvalid()
    {
        // Arrange
        // Bishop at C1 is blocked by pawn at D2
        var board = new Gameboard();

        // Act & Assert
        var move = new Move("C1", "E3");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }

    /// <summary>
    /// Checks that rook A1-A3 is invalid on the default board due to blocking pawn
    /// </summary>
    [TestMethod]
    public void DefaultBoard_RookA1Blocked_IsInvalid()
    {
        // Arrange
        // Rook at A1 is blocked by pawn at A2
        var board = new Gameboard();

        // Act & Assert
        var move = new Move("A1", "A3");
        Assert.AreEqual(Invalid, ValidateMove(move, board));
    }
}
