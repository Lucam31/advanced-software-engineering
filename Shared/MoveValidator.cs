namespace Shared;

using Pieces;

// TODO: König check ob neues feld im Schach wäre

/// <summary>
/// Provides static methods for validating chess moves.
/// </summary>
public static class MoveValidator
{
    public static bool WhitePlayer { get; set; } = true;
    
    
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
            return ((colDiff == rowDiff) && ValidateTilesBetween(oldPosition, newPosition, movePiece, gameboard)) ||
                   ((colDiff == 0 || rowDiff == 0) && ValidateTilesBetween(oldPosition, newPosition, movePiece, gameboard));
        }

        if (movePiece is King)
        {
            return ((colDiff == 0 && rowDiff == 1) || (colDiff == 1 && rowDiff == 0) ||
                   (colDiff == 1 && rowDiff == 1)) && !CheckCheckmate(newPosition, gameboard) || 
                   (colDiff == 2 && rowDiff == 0) && !CheckCheckmate(newPosition, gameboard) && CheckCastling();
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
    /// Checks if the passed tile is in checkmate for the king
    /// </summary>
    private static bool CheckCheckmate(string position, Gameboard gameboard)
    {
        // Pawn attack positions
        if (CheckPawnAttack(position, gameboard)) return true;
        // Knight attack positions
        if (CheckKnightAttack(position, gameboard)) return true;
        // Bishop/Queen attack positions
        if (CheckBishopQueenAttack(position, gameboard)) return true;
        // Rook/Queen attack positions
        if (CheckRookQueenAttack(position, gameboard)) return true;
        // King attack positions
        if (CheckKingAttack(position, gameboard)) return true;
        
        return false;
    }
    
    /// <summary>
    /// Checks if castling is valid
    /// </summary>
    private static bool CheckCastling()
    {
        // Placeholder for castling logic
        return false;
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
    
    
    private static bool CheckPawnAttack(string position, Gameboard gameboard)
    {
        var piecePos1 = gameboard.GetPieceAtPosition($"{(char)(position[0] - 1)}{(char)(position[1] - 1)}");
        var piecePos2 = gameboard.GetPieceAtPosition($"{(char)(position[0] + 1)}{(char)(position[1] - 1)}");
        return (piecePos1 is Pawn && (WhitePlayer ? !piecePos1.IsWhite : piecePos1.IsWhite) ||
            piecePos2 is Pawn && (WhitePlayer ? !piecePos2.IsWhite : piecePos2.IsWhite)) ;
    }
    private static bool CheckKnightAttack(string position, Gameboard gameboard)
    {
        int[] colOffsets = [-2, -1, 1, 2, 2, 1, -1, -2];
        int[] rowOffsets = [1, 2, 2, 1, -1, -2, -2, -1];

        for (var i = 0; i < colOffsets.Length; i++)
        {
            var col = (char)(position[0] + colOffsets[i]);
            var row = (char)(position[1] + rowOffsets[i]);
            if (col < 'A' || col > 'H' || row < '1' || row > '8') continue;
            var checkPos = $"{col}{row}";
            var piece = gameboard.GetPieceAtPosition(checkPos);
            if (piece is Knight && (WhitePlayer ? !piece.IsWhite : piece.IsWhite))
            {
                return true;
            }
        }

        return false;
    }

    private static bool CheckBishopQueenAttack(string position, Gameboard gameboard)
    {
        for (var i = -1; i <= 1; i += 2)
        {
            for (var j = -1; j <= 1; j += 2)
            {
                var col = (char)(position[0] + i);
                var row = (char)(position[1] + j);
                while (col is >= 'A' and <= 'H' && row is >= '1' and <= '8')
                {
                    var checkPos = $"{col}{row}";
                    var piece = gameboard.GetPieceAtPosition(checkPos);
                    if (piece != null)
                    {
                        if ((piece is Bishop or Queen) &&
                            (WhitePlayer ? !piece.IsWhite : piece.IsWhite))
                        {
                            return true;
                        }
                        break; // Blocked by another piece
                    }
                    col = (char)(col + i);
                    row = (char)(row + j);
                }
            }
        }
        
        return false;
    }
    private static bool CheckRookQueenAttack(string position, Gameboard gameboard)
    {
        for (var i = -1; i <= 1; i += 2)
        {
            var newCol = (char)(position[0] + i);
            var newRow = (char)(position[1] + i);
            for (var col = newCol; col is >= 'A' and <= 'H'; col = (char)(col + i)) 
            {
               var piece = gameboard.GetPieceAtPosition($"{col}{position[1]}");
               if (piece is Rook or Queen && (WhitePlayer ? !piece.IsWhite : piece.IsWhite))
               {
                   return true;
               }
               if (piece != null) break; // Blocked by another piece
            }
            
            for (var row = newRow; row is >= '1' and <= '8'; row = (char)(row + i)) 
            {
               var piece = gameboard.GetPieceAtPosition($"{position[0]}{row}");
               if (piece is Rook or Queen && (WhitePlayer ? !piece.IsWhite : piece.IsWhite))
               {
                   return true;
               }
               if (piece != null) break; // Blocked by another piece
            }
        }
        
        return false;
    }

    private static bool CheckKingAttack(string position, Gameboard gameboard)
    {
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                if (i == 1 && j == 1) continue; // Skip the kings own position 
                var col = (char)(position[0] - 1 + i);
                var row = (char)(position[1] - 1 + j);
                if (col < 'A' || col > 'H' || row < '1' || row > '8') continue;
                var checkPos = $"{col}{row}";
                var piece = gameboard.GetPieceAtPosition(checkPos);
                if (piece is King && (WhitePlayer ? !piece.IsWhite : piece.IsWhite))
                {
                    return true;
                }
            }
        }

        return false;
    }
}