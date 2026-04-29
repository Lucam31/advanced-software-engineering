using Shared.Logger;
using Shared.Pieces;

namespace Shared;


// TODO: König check ob neues feld im Schach wäre

/// <summary>
/// Provides static methods for validating chess moves
/// </summary>
public static class MoveValidator
{
    public static bool WhitePlayer { get; set; } = true;
    
    public enum MoveValidationResult
    {
        Invalid = 0,
        Valid = 1,
        EnPassant = 2,
        Castling = 3,
        Check = 4,
        Checkmate = 5
    }
    
    // TODO: change ValidateMove to return an enum (valid, invalid, check, checkmate, enPassant, castling)
    /// <summary>
    /// Validates a given move on the specified game board
    /// </summary>
    /// <param name="move">The move to validate</param>
    /// <param name="gameboard">The current game board state</param>
    /// <returns>True if the move is valid; otherwise, false</returns>
    public static MoveValidationResult ValidateMove(Move move, Gameboard gameboard)
    {
        var oldPosition = move.From;
        var newPosition = move.To;
        if (oldPosition == newPosition) return MoveValidationResult.Invalid;
        var movePiece = gameboard.GetPieceAtPosition(oldPosition);
        var targetPiece = gameboard.GetPieceAtPosition(newPosition);

        if (movePiece != null && targetPiece != null && movePiece.IsWhite == targetPiece.IsWhite) return MoveValidationResult.Invalid;

        var colDiff = Math.Abs(newPosition[0] - oldPosition[0]);
        var rawRowDiff = newPosition[1] - oldPosition[1];
        var rowDiff = Math.Abs(rawRowDiff);
        
        var startRow = oldPosition[1] - '0';
        var startCol = oldPosition[0];
        var endRow = newPosition[1] - '0';
        var endCol = newPosition[0];

        if (movePiece is Pawn pawn)
        {
            // Enforce pawn direction: white moves up, black moves down
            if (pawn.IsWhite && rawRowDiff <= 0) return MoveValidationResult.Invalid;
            if (!pawn.IsWhite && rawRowDiff >= 0) return MoveValidationResult.Invalid;
            
            if (colDiff > 1 && movePiece.Moved) return MoveValidationResult.Invalid;
            if (colDiff == 1 && rowDiff == 1) // Pawns can't move sideways unless capturing
            {
                if (targetPiece is not null || CheckEnPassant(startRow, startCol, endRow, endCol, gameboard))
                {
                    pawn.EnPassantEligible = false;
                    return targetPiece is null ? MoveValidationResult.EnPassant : MoveValidationResult.Valid;
                }
                return MoveValidationResult.Invalid; // Diagonal without capture or en passant
            }

            if (!pawn.Moved && rowDiff == 2 && colDiff == 0 && ValidateTilesBetween(startRow, startCol, endRow, endCol, pawn, gameboard))
            {
                pawn.EnPassantEligible = true;
                return MoveValidationResult.Valid;
            }

            if (colDiff == 0 && rowDiff > 0 && rowDiff is < 2 && ValidateTilesBetween(startRow, startCol, endRow, endCol, movePiece, gameboard))
            {
                pawn.EnPassantEligible = false;
                return MoveValidationResult.Valid;
            }

            return MoveValidationResult.Invalid;
        }

        if (movePiece is Bishop)
        {
            // Bishop moves diagonally, so the difference between the file and rank should be the same
            return (colDiff == rowDiff && ValidateTilesBetween(startRow, startCol, endRow, endCol, movePiece, gameboard)) ? MoveValidationResult.Valid : MoveValidationResult.Invalid;
        }


        if (movePiece is Rook)
        {
            return ((colDiff == 0 && rowDiff != 0) || (colDiff != 0 && rowDiff == 0)) &&
                   ValidateTilesBetween(startRow, startCol, endRow, endCol, movePiece, gameboard) ? MoveValidationResult.Valid : MoveValidationResult.Invalid;
        }

        if (movePiece is Knight)
        {
            // check for castling
            return (colDiff == 2 && rowDiff == 1) || (colDiff == 1 && rowDiff == 2) ? MoveValidationResult.Valid : MoveValidationResult.Invalid;
        }

        if (movePiece is Queen)
        {
            return ((colDiff == rowDiff) && ValidateTilesBetween(startRow, startCol, endRow, endCol, movePiece, gameboard)) || (
                (colDiff == 0 || rowDiff == 0) &&
                ValidateTilesBetween(startRow, startCol, endRow, endCol, movePiece, gameboard)) ? MoveValidationResult.Valid : MoveValidationResult.Invalid;
        }

        if (movePiece is King)
        {
            if (colDiff == 2 && rowDiff == 0)
            {
                return (CheckCastling(startRow, startCol, endRow, endCol, gameboard) && !MoveValidatorHelper.IsCheck(newPosition, gameboard)) ? MoveValidationResult.Castling : MoveValidationResult.Invalid;
            }

            if (colDiff <= 1 && rowDiff <= 1)
            {
                return MoveValidatorHelper.IsCheck(newPosition, gameboard) ? MoveValidationResult.Check : MoveValidationResult.Valid;
            }
        }

        return MoveValidationResult.Invalid;
    }

    /// <summary>
    /// Validates that the path between two tiles is clear for a given piece
    /// </summary>
    /// <param name="startRow">The starting row</param>
    /// <param name="startCol">The starting row</param>
    /// <param name="endRow">The ending position in algebraic notation</param>
    /// <param name="endCol">The ending position in algebraic notation</param>
    /// <param name="piece">The piece that is moving</param>
    /// <param name="gameboard">The current game board state</param>
    /// <returns>True if the path is clear; otherwise, false</returns>
    private static bool ValidateTilesBetween(int startRow, char startCol, int endRow, char endCol, Piece piece, Gameboard gameboard)
    {

        switch (piece)
        {
            case Pawn when startRow < endRow:
                return (gameboard.GetPieceAtPosition($"{startCol}{startRow + 1}") is null &&
                        gameboard.GetPieceAtPosition($"{startCol}{endRow}") is null);
            case Pawn:
                return (gameboard.GetPieceAtPosition($"{startCol}{startRow - 1}") is null &&
                        gameboard.GetPieceAtPosition($"{startCol}{endRow}") is null);
            case Rook:
                return RookCheck(startRow, startCol, endRow, endCol, gameboard);
            case Knight:
                return true; // Knights are Sigma males and can jump over the other Beta pieces
            case Queen:
            {
                // Slayy Queen
                var colDiff = Math.Abs(endCol - startCol);
                var rowDiff = Math.Abs(endRow - startRow);
                if (rowDiff is < 2 && colDiff is < 2) return true; // adjacent move
                if (colDiff == rowDiff) return BishopCheck(startRow, startCol, endRow, endCol, gameboard);
                if (colDiff == 0 || rowDiff == 0) return RookCheck(startRow, startCol, endRow, endCol, gameboard);
                return false;
            }
            case King:
                // Kings can only move one square, so there are no tiles between start and end to check
                return true;
            default:
                return piece is Bishop && BishopCheck(startRow, startCol, endRow, endCol, gameboard);
        }
    }

    /// <summary>
    /// Checks if the path is clear for a rook's move (horizontally or vertically)
    /// </summary>
    /// <param name="startRow">The starting row</param>
    /// <param name="startCol">The starting column</param>
    /// <param name="endRow">The ending row</param>
    /// <param name="endCol">The ending column</param>
    /// <param name="gameboard">The game board</param>
    /// <returns>True if the path is clear, otherwise false</returns>
    private static bool RookCheck(int startRow, char startCol, int endRow, char endCol, Gameboard gameboard)
    {
        if (startRow < endRow)
        {
            for (var row = startRow + 1; row < endRow; row++)
            {
                if (gameboard.GetPieceAtPosition($"{startCol}{row}") is not null) return false;
            }

            return true;
        }

        if (startRow > endRow)
        {
            for (var row = startRow - 1; row > endRow; row--)
            {
                if (gameboard.GetPieceAtPosition($"{startCol}{row}") is not null) return false;
            }

            return true;
        }

        if (startCol < endCol)
        {
            for (var col = Convert.ToChar(startCol + 1); col < endCol; col++)
            {
                if (gameboard.GetPieceAtPosition($"{col}{startRow}") is not null) return false;
            }

            return true;
        }

        if (startCol <= endCol) return false;
        {
            for (var col = Convert.ToChar(startCol - 1); col > endCol; col--)
            {
                if (gameboard.GetPieceAtPosition($"{col}{startRow}") is not null) return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Checks if the path is clear for a bishop's move (diagonally)
    /// </summary>
    /// <param name="startRow">The starting row</param>
    /// <param name="startCol">The starting column</param>
    /// <param name="endRow">The ending row</param>
    /// <param name="endCol">The ending column</param>
    /// <param name="gameboard">The game board</param>
    /// <returns>True if the path is clear, otherwise false</returns>
    private static bool BishopCheck(int startRow, char startCol, int endRow, char endCol, Gameboard gameboard)
    {
        if (startRow < endRow && startCol < endCol)
        {
            var col = Convert.ToChar(startCol + 1);
            for (var row = startRow + 1; row < endRow; row++)
            {
                if (gameboard.GetPieceAtPosition($"{col}{row}") is not null) return false;
                col++;
            }

            return true;
        }

        if (startRow < endRow && startCol > endCol)
        {
            var col = Convert.ToChar(startCol - 1);
            for (var row = startRow + 1; row < endRow; row++)
            {
                if (gameboard.GetPieceAtPosition($"{col}{row}") is not null) return false;
                col--;
            }

            return true;
        }

        if (startRow > endRow && startCol < endCol)
        {
            var col = Convert.ToChar(startCol + 1);
            for (var row = startRow - 1; row > endRow; row--)
            {
                if (gameboard.GetPieceAtPosition($"{col}{row}") is not null) return false;
                col++;
            }

            return true;
        }

        if (startRow <= endRow || startCol <= endCol) return false;
        {
            var col = Convert.ToChar(startCol - 1);
            for (var row = startRow - 1; row > endRow; row--)
            {
                if (gameboard.GetPieceAtPosition($"{col}{row}") is not null) return false;
                col--;
            }

            return true;
        }
    }

    
    
    // ===================
    // Special Move Checks
    // ===================
    
    /// <summary>
    /// Checks if castling is a valid move
    /// </summary>
    /// <param name="startRow">The row of the starting position</param>
    /// <param name="startCol">The column of the starting position</param>
    /// <param name="endRow">The row of the end position</param>
    /// <param name="endCol">The column of the end position</param>
    /// <param name="gameboard">The instance of the gameboard</param>
    /// <returns>True if Castling is valid, false if otherwise</returns>
    private static bool CheckCastling(int startRow, char startCol, int endRow, char endCol, Gameboard gameboard)
    {
        var king = gameboard.GetPieceAtPosition($"{startCol}{startRow}");
        var rook = endCol < startCol
            ? gameboard.GetPieceAtPosition($"A{startRow}")
            : gameboard.GetPieceAtPosition($"H{startRow}");
        if (king is King && rook is Rook && king.IsWhite == rook.IsWhite && !king.Moved && !rook.Moved)
        {            // Check if the tiles between the king and rook are clear
            if (ValidateTilesBetween(startRow, startCol, endRow, endCol, rook, gameboard))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if an en passant move is valid
    /// </summary>
    /// <param name="startRow">The row of the starting position</param>
    /// <param name="startCol">The column of the starting position</param>
    /// <param name="endRow">The row of the end position</param>
    /// <param name="endCol">The column of the end position</param>
    /// <param name="gameboard">The instance of the gameboard</param>
    /// <returns>True if the en passant move is valid, otherwise false</returns>
    private static bool CheckEnPassant(int startRow, char startCol, int endRow, char endCol, Gameboard gameboard)
    {
        var pawn = gameboard.GetPieceAtPosition($"{startCol}{startRow}");
        var enemyPawn = gameboard.GetPieceAtPosition($"{endCol}{startRow}");
        if (pawn is null || enemyPawn is null)
        {
            GameLogger.Warning("Pawn or enemyPawn is null");
            return false;
        }
        
        if (enemyPawn is Pawn && pawn.IsWhite != enemyPawn.IsWhite && enemyPawn is Pawn { EnPassantEligible: true })
        {
            return true;
        }
        return false;
    }
}