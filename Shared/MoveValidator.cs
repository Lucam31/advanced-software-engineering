namespace Shared;

using Pieces;

public static class MoveValidator
{
    public static bool ValidateMove(Move move, Gameboard gameboard)
    {
        var oldPosition = move.From;
        var newPosition = move.To;
        if (oldPosition == newPosition) return false;
        Piece? movePiece = gameboard.GetPieceAtPosition(oldPosition);
        Piece? targetPiece = gameboard.GetPieceAtPosition(newPosition);

        if (movePiece != null && targetPiece != null && movePiece.IsWhite == targetPiece.IsWhite) return false;
        
        int colDiff = Math.Abs(newPosition[0] - oldPosition[0]);
        int rawRowDiff = newPosition[1] - oldPosition[1];
        int rowDiff = Math.Abs(rawRowDiff);

        if (movePiece is Pawn)
        {
            // Pawn Promotion
            if (colDiff > 1 && movePiece.Moved || (colDiff == 1 && rowDiff > 1)) return false;
            if (colDiff == 1 && rawRowDiff == 1) // Pawns can't move sideways unless capturing
            {
                if (targetPiece is not null || CheckEnPassant()) return true;
                return false;
            }
            if (!movePiece.Moved)
            {
                return (rawRowDiff is > 0 and < 3 && ValidateTilesBetween(oldPosition, newPosition, movePiece, gameboard));
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
            return ((colDiff == 0 && rowDiff != 0) || (colDiff != 0 && rowDiff == 0)) && ValidateTilesBetween(oldPosition, newPosition, movePiece, gameboard);
        }

        if (movePiece is Knight)
        {
            // check for castling
            return (colDiff == 2 && rowDiff == 1) || (colDiff == 1 && rowDiff == 2);
        }

        if (movePiece is Queen)
        {
            return (colDiff == rowDiff) || (colDiff == 0 || rowDiff != 0) && ValidateTilesBetween(oldPosition, newPosition, movePiece, gameboard);
        }

        if (movePiece is King)
        {
            return (colDiff == 0 && rowDiff == 1) || (colDiff == 1 && rowDiff == 0) ||
                   (colDiff == 1 && rowDiff == 1);
        }

        return false;
    }
    
    private static bool ValidateTilesBetween(string start, string end, Piece piece, Gameboard gameboard)
    {
        int startRow = start[1]-'0';
        char startCol = start[0];
        int endRow = end[1]-'0';
        char endCol = end[0];
        
        if (piece is Pawn)
        {
            if (startRow < endRow)
            {
                return (gameboard.GetPieceAtPosition($"{startCol}{startRow+1}") is null && gameboard.GetPieceAtPosition($"{startCol}{endRow}") is null);
            }
            else
            {
                return (gameboard.GetPieceAtPosition($"{startCol}{startRow-1}") is null && gameboard.GetPieceAtPosition($"{startCol}{endRow}") is null);
            }
        }
        else if (piece is Rook)
        {
            return RookCheck(startRow, startCol, endRow, endCol, gameboard);
        }
        else if (piece is Knight)
        {
            return true; // Knights are Sigma males and can jump over the other Beta pieces
        }
        else if (piece is Queen)
        {
            // Slayy Queen
            int colDiff = Math.Abs(endCol - startCol);
            int rowDiff = Math.Abs(endRow - startRow);
            if (rowDiff is < 2 && colDiff is < 2) return true; // adjacent move
            else if (colDiff == rowDiff) return BishopCheck(startRow, startCol, endRow, endCol, gameboard);
            else if (colDiff == 0 || rowDiff == 0) return RookCheck(startRow, startCol, endRow, endCol, gameboard);
            else return false;
        }
        else if (piece is King)
        {
            // Kings can only move one square, so there are no tiles between start and end to check
            return true;
        }
        else if (piece is Bishop)
        {
            return BishopCheck(startRow, startCol, endRow, endCol, gameboard);
        }
        
        return false;
    }

    private static bool RookCheck(int startRow, char startCol, int endRow, char endCol, Gameboard gameboard)
    {
        if (startRow < endRow)
        {
            for (int row = startRow + 1; row < endRow; row++)
            {
                if (gameboard.GetPieceAtPosition($"{startCol}{row}") is not null) return false;
            }
            return true;
        }
        else if (startRow > endRow)
        {
            for (int row = startRow - 1; row > endRow; row--)
            {
                if (gameboard.GetPieceAtPosition($"{startCol}{row}") is not null) return false;
            }
            return true;
        }
        else if (startCol < endCol)
        {
            for (char col = Convert.ToChar(startCol + 1); col < endCol; col++)
            {
                if (gameboard.GetPieceAtPosition($"{col}{startRow}") is not null) return false;
            }
            return true;
        }
        else if (startCol > endCol)
        {
            for (char col = Convert.ToChar(startCol - 1); col > endCol; col--)
            {
                if (gameboard.GetPieceAtPosition($"{col}{startRow}") is not null) return false;
            }
            return true;
        }
        return false;
    }

    private static bool BishopCheck(int startRow, char startCol, int endRow, char endCol, Gameboard gameboard)
    {
        int test1 = 0;
        if (startRow < endRow && startCol < endCol)
        {
            char col = Convert.ToChar(startCol + 1);
            for (int row = startRow + 1; row < endRow; row++)
            {
                Piece? test = gameboard.GetPieceAtPosition($"{col}{row}");
                if (gameboard.GetPieceAtPosition($"{col}{row}") is not null) return false;
                col++;
            }
            return true;
        }
        else if (startRow < endRow && startCol > endCol)
        {
            char col = Convert.ToChar(startCol - 1);
            for (int row = startRow + 1; row < endRow; row++)
            {
                Piece? test = gameboard.GetPieceAtPosition($"{col}{row}");
                if (gameboard.GetPieceAtPosition($"{col}{row}") is not null) return false;
                col--;
            }
            return true;
        }
        else if (startRow > endRow && startCol < endCol)
        {
            char col = Convert.ToChar(startCol + 1);
            for (int row = startRow - 1; row > endRow; row--)
            {
                Piece? test = gameboard.GetPieceAtPosition($"{col}{row}");
                if (gameboard.GetPieceAtPosition($"{col}{row}") is not null) return false;
                col++;
            }
            return true;
        }
        else if (startRow > endRow && startCol > endCol)
        {
            char col = Convert.ToChar(startCol - 1);
            for (int row = startRow - 1; row > endRow; row--)
            {
                Piece? test = gameboard.GetPieceAtPosition($"{col}{row}");
                if (gameboard.GetPieceAtPosition($"{col}{row}") is not null) return false;
                col--;
            }
            return true;
        }

        return false;
    }
    private static bool CheckEnPassant()
    {
        // Placeholder for en passant logic
        return false;
    }
}