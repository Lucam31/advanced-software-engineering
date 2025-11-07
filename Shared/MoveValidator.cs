namespace Shared;

using Pieces;

// TODO: König check ob neues feld im Schach wäre

/// <summary>
/// Provides static methods for validating chess moves.
/// </summary>
public static class MoveValidator
{
    /// <summary>
    /// Validates a given move on the specified game board.
    /// </summary>
    /// <param name="move">The move to validate.</param>
    /// <param name="gameboard">The current game board state.</param>
    /// <returns>True if the move is valid; otherwise, false.</returns>
    public static bool ValidateMove(Move move, Gameboard gameboard)
    {
        var oldPosition = move.From;
        var newPosition = move.To;
        if (oldPosition == newPosition) return false;
        var movePiece = gameboard.GetPieceAtPosition(oldPosition);
        var targetPiece = gameboard.GetPieceAtPosition(newPosition);

        if (movePiece != null && targetPiece != null && movePiece.IsWhite == targetPiece.IsWhite) return false;

        var colDiff = Math.Abs(newPosition[0] - oldPosition[0]);
        var rawRowDiff = newPosition[1] - oldPosition[1];
        var rowDiff = Math.Abs(rawRowDiff);

        if (movePiece is Pawn)
        {
            // Pawn Promotion
            if (colDiff > 1 && movePiece.Moved || (colDiff == 1 && rowDiff > 1)) return false;
            if (colDiff == 1 && rawRowDiff == 1) // Pawns can't move sideways unless capturing
            {
                return targetPiece is not null || CheckEnPassant();
            }

            if (!movePiece.Moved)
            {
                return (rawRowDiff is > 0 and < 3 &&
                        ValidateTilesBetween(oldPosition, newPosition, movePiece, gameboard));
            }

            return (rawRowDiff is > 0 and < 2 && ValidateTilesBetween(oldPosition, newPosition, movePiece, gameboard));
        }

        if (movePiece is Bishop)
        {
            // Bishop moves diagonally, so the difference between the file and rank should be the same
            return colDiff == rowDiff && ValidateTilesBetween(oldPosition, newPosition, movePiece, gameboard);
        }


        if (movePiece is Rook)
        {
            return ((colDiff == 0 && rowDiff != 0) || (colDiff != 0 && rowDiff == 0)) &&
                   ValidateTilesBetween(oldPosition, newPosition, movePiece, gameboard);
        }

        if (movePiece is Knight)
        {
            // check for castling
            return (colDiff == 2 && rowDiff == 1) || (colDiff == 1 && rowDiff == 2);
        }

        if (movePiece is Queen)
        {
            return (colDiff == rowDiff) || (colDiff == 0 || rowDiff != 0) &&
                ValidateTilesBetween(oldPosition, newPosition, movePiece, gameboard);
        }

        if (movePiece is King)
        {
            return (colDiff == 0 && rowDiff == 1) || (colDiff == 1 && rowDiff == 0) ||
                   (colDiff == 1 && rowDiff == 1);
        }

        return false;
    }

    /// <summary>
    /// Validates that the path between two tiles is clear for a given piece.
    /// </summary>
    /// <param name="start">The starting position in algebraic notation.</param>
    /// <param name="end">The ending position in algebraic notation.</param>
    /// <param name="piece">The piece that is moving.</param>
    /// <param name="gameboard">The current game board state.</param>
    /// <returns>True if the path is clear; otherwise, false.</returns>
    private static bool ValidateTilesBetween(string start, string end, Piece piece, Gameboard gameboard)
    {
        var startRow = start[1] - '0';
        var startCol = start[0];
        var endRow = end[1] - '0';
        var endCol = end[0];

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
    /// Checks if the path is clear for a rook's move (horizontally or vertically).
    /// </summary>
    /// <param name="startRow">The starting row.</param>
    /// <param name="startCol">The starting column.</param>
    /// <param name="endRow">The ending row.</param>
    /// <param name="endCol">The ending column.</param>
    /// <param name="gameboard">The game board.</param>
    /// <returns>True if the path is clear, otherwise false.</returns>
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
    /// Checks if the path is clear for a bishop's move (diagonally).
    /// </summary>
    /// <param name="startRow">The starting row.</param>
    /// <param name="startCol">The starting column.</param>
    /// <param name="endRow">The ending row.</param>
    /// <param name="endCol">The ending column.</param>
    /// <param name="gameboard">The game board.</param>
    /// <returns>True if the path is clear, otherwise false.</returns>
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

    /// <summary>
    /// Checks if an en passant move is valid.
    /// </summary>
    /// <returns>True if the en passant move is valid, otherwise false. Currently a placeholder.</returns>
    private static bool CheckEnPassant()
    {
        // Placeholder for en passant logic
        return false;
    }
}