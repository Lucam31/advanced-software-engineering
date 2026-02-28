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
            // Pawn Promotion
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
                return (CheckCastling(startRow, startCol, endRow, endCol, gameboard) && !CheckCheck(newPosition, gameboard)) ? MoveValidationResult.Castling : MoveValidationResult.Invalid;
            }

            if (colDiff <= 1 && rowDiff <= 1)
            {
                return CheckCheck(newPosition, gameboard) ? MoveValidationResult.Check : MoveValidationResult.Valid;
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

    /// <summary>
    /// Finds the position of the king of the specified color on the board
    /// </summary>
    /// <param name="isWhite">True to find the white king, false for the black king</param>
    /// <param name="gameboard">The current game board state</param>
    /// <returns>The position string of the king, or null if not found</returns>
    private static string? FindKingPosition(bool isWhite, Gameboard gameboard)
    {
        for (var row = 1; row <= 8; row++)
        {
            for (var col = 'A'; col <= 'H'; col++)
            {
                var position = $"{col}{row}";
                var piece = gameboard.GetPieceAtPosition(position);
                if (piece is King && piece.IsWhite == isWhite)
                {
                    return position;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Checks whether the given color's king is currently in check
    /// </summary>
    /// <param name="isWhite">True to check if white king is in check, false for black</param>
    /// <param name="gameboard">The current game board state</param>
    /// <returns>True if the king is in check; otherwise, false</returns>
    public static bool IsKingInCheck(bool isWhite, Gameboard gameboard)
    {
        var kingPos = FindKingPosition(isWhite, gameboard);
        if (kingPos == null) return false;

        // Temporarily flip WhitePlayer to check from the king's perspective
        var originalWhitePlayer = WhitePlayer;
        WhitePlayer = isWhite;
        var inCheck = CheckCheck(kingPos, gameboard);
        WhitePlayer = originalWhitePlayer;
        return inCheck;
    }

    /// <summary>
    /// Checks whether the given color's king is in checkmate
    /// The king is in checkmate when it is in check and no legal move can escape the check
    /// </summary>
    /// <param name="isWhite">True to check if white is in checkmate, false for black</param>
    /// <param name="gameboard">The current game board state</param>
    /// <returns>True if the king is in checkmate; otherwise, false</returns>
    public static bool IsCheckmate(bool isWhite, Gameboard gameboard)
    {
        // First check: the king must be in check
        if (!IsKingInCheck(isWhite, gameboard)) return false;

        // Try every possible move for every piece of the given color.
        // If any move results in the king no longer being in check, it's not checkmate.
        for (var row = 1; row <= 8; row++)
        {
            for (var col = 'A'; col <= 'H'; col++)
            {
                var fromPos = $"{col}{row}";
                var piece = gameboard.GetPieceAtPosition(fromPos);
                if (piece == null || piece.IsWhite != isWhite) continue;

                // Try all possible destination squares
                for (var destRow = 1; destRow <= 8; destRow++)
                {
                    for (var destCol = 'A'; destCol <= 'H'; destCol++)
                    {
                        var toPos = $"{destCol}{destRow}";
                        if (fromPos == toPos) continue;

                        var testMove = new Move(fromPos, toPos);

                        // Save the original WhitePlayer and set it for validation
                        var originalWhitePlayer = WhitePlayer;
                        WhitePlayer = isWhite;
                        var result = ValidateMove(testMove, gameboard);
                        WhitePlayer = originalWhitePlayer;

                        // If the move is not invalid, simulate it and check if the king is still in check
                        if (result == MoveValidationResult.Invalid) continue;

                        // Simulate the move on a copy of the board
                        if (TryMoveEscapesCheck(testMove, result, isWhite, gameboard))
                        {
                            return false; // Found a legal move that escapes check
                        }
                    }
                }
            }
        }

        // No legal move can escape check — checkmate
        return true;
    }

    /// <summary>
    /// Simulates a move on a copy of the gameboard and checks if the king is still in check afterwards
    /// </summary>
    /// <param name="move">The move to simulate</param>
    /// <param name="result">The validation result of the move</param>
    /// <param name="isWhite">The color of the player making the move</param>
    /// <param name="gameboard">The current game board state</param>
    /// <returns>True if the move results in the king no longer being in check</returns>
    private static bool TryMoveEscapesCheck(Move move, MoveValidationResult result, bool isWhite, Gameboard gameboard)
    {
        // Create a copy of the board via DTO round-trip
        var boardCopy = Gameboard.FromDto(gameboard.ToDto());

        // Execute the move on the copy
        var isEnPassant = result == MoveValidationResult.EnPassant;
        boardCopy.Move(move, isEnPassant);

        // For castling, also move the rook
        if (result == MoveValidationResult.Castling)
        {
            var row = move.From[1];
            if (move.To[0] > move.From[0])
            {
                boardCopy.Move(new Move($"H{row}", $"F{row}"));
            }
            else
            {
                boardCopy.Move(new Move($"A{row}", $"D{row}"));
            }
        }

        // Check if the king is still in check after the move
        return !IsKingInCheck(isWhite, boardCopy);
    }

    /// <summary>
    /// Checks if the passed tile is under attack by an opponent piece
    /// </summary>
    private static bool CheckCheck(string position, Gameboard gameboard)
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
    /// <returns>True if the en passant move is valid, otherwise false</returns>
    private static bool CheckEnPassant(int startRow, char startCol, int endRow, char endCol, Gameboard gameboard)
    {
        var pawn = gameboard.GetPieceAtPosition($"{startCol}{startRow}");
        var enemyPawn = gameboard.GetPieceAtPosition($"{endCol}{startRow}");
        if (enemyPawn is Pawn && pawn.IsWhite != enemyPawn.IsWhite && enemyPawn is Pawn { EnPassantEligible: true })
        {
            return true;
        }
        return false;
    }
    
    
    private static bool CheckPawnAttack(string position, Gameboard gameboard)
    {
        var col = position[0];
        var row = position[1];

        // Check diagonal attack from the left
        if (col > 'A' && row > '1')
        {
            var piecePos1 = gameboard.GetPieceAtPosition($"{(char)(col - 1)}{(char)(row - 1)}");
            if (piecePos1 is Pawn && (WhitePlayer ? !piecePos1.IsWhite : piecePos1.IsWhite))
                return true;
        }

        // Check diagonal attack from the right
        if (col < 'H' && row > '1')
        {
            var piecePos2 = gameboard.GetPieceAtPosition($"{(char)(col + 1)}{(char)(row - 1)}");
            if (piecePos2 is Pawn && (WhitePlayer ? !piecePos2.IsWhite : piecePos2.IsWhite))
                return true;
        }

        return false;
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