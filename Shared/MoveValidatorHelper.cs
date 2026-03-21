using Shared.Logger;
using Shared.Pieces;

namespace Shared;

public static class MoveValidatorHelper
{
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
    
    // =========================================
    // Check, Checkmate, and Stalemate Detection
    // =========================================
    
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

        // Temporarily flip MoveValidator.WhitePlayer to check from the king's perspective
        var originalWhitePlayer = MoveValidator.WhitePlayer;
        MoveValidator.WhitePlayer = isWhite;
        var inCheck = IsCheck(kingPos, gameboard);
        MoveValidator.WhitePlayer = originalWhitePlayer;
        return inCheck;
    }
    
    /// <summary>
    /// Checks whether the given color's king is in checkmate
    /// The king is in checkmate when it is in check and no legal move can escape the check
    /// </summary>
    /// <param name="isWhite">True to check if white is in checkmate, false for black</param>
    /// <param name="gameboard">The current game board state</param>
    /// <param name="stalemateCheck">Set to true for Stalemate Check</param>
    /// <returns>True if the king is in checkmate; otherwise, false</returns>
    public static bool IsCheckmate(bool isWhite, Gameboard gameboard, bool stalemateCheck = false)
    {
        // First check: the king must be in check
        if (!IsKingInCheck(isWhite, gameboard) && !stalemateCheck) return false;

        // Try every possible move for every piece of the given color
        // If any move results in the king no longer being in check, it's not checkmate
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

                        // Save the original MoveValidator.WhitePlayer and set it for validation
                        var originalMoveValidator = MoveValidator.WhitePlayer;
                        MoveValidator.WhitePlayer = isWhite;
                        var result = MoveValidator.ValidateMove(testMove, gameboard);
                        MoveValidator.WhitePlayer = originalMoveValidator;

                        // If the move is not invalid, simulate it and check if the king is still in check
                        if (result == MoveValidator.MoveValidationResult.Invalid) continue;

                        // Simulate the move on a copy of the board
                        if (TryMoveEscapesCheck(testMove, result, isWhite, gameboard))
                        {
                            GameLogger.Info($"Escaping {testMove}");
                            return false; // Found a legal move that escapes check
                        }
                    }
                }
            }
        }

        // No legal move can escape check —> checkmate
        return true;
    }

    /// <summary>
    /// Checks whether there is a stalemate for the given color
    /// A stalemate occurs when the player is not in check but has no legal moves available
    /// </summary>
    /// <param name="isWhite">True to check if white is in checkmate, false for black</param>
    /// <param name="gameboard">The current game board state</param>
    /// <returns>True if it is a stalemate; otherwise, false</returns>
    public static bool IsStalemate(bool isWhite, Gameboard gameboard)
    {
        return IsCheckmate(isWhite, gameboard, true);
    }

    /// <summary>
    /// Check if the given position is attacked by any opponent's piece, indicating that the king is in check
    /// </summary>
    /// <param name="position">Board coordinate</param>
    /// <param name="gameboard">Current board state</param>
    /// <returns>True if the square is attacked by an opponent piece</returns>
    public static bool IsCheck(string position, Gameboard gameboard)
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
    
    
    
    // ==================================
    // Helper methods for check detection
    // ==================================
    
    /// <summary>
    /// Simulates a move on a copy of the gameboard and checks if the king is still in check afterwards
    /// </summary>
    /// <param name="move">The move to simulate</param>
    /// <param name="result">The validation result of the move</param>
    /// <param name="isWhite">The color of the player making the move</param>
    /// <param name="gameboard">The current game board state</param>
    /// <returns>True if the move results in the king no longer being in check</returns>
    public static bool TryMoveEscapesCheck(Move move, MoveValidator.MoveValidationResult result, bool isWhite, Gameboard gameboard)
    {
        // Create a copy of the board via DTO round-trip
        var boardCopy = Gameboard.FromDto(gameboard.ToDto());

        // Execute the move on the copy
        var isEnPassant = result == MoveValidator.MoveValidationResult.EnPassant;
        var isCastling = result == MoveValidator.MoveValidationResult.Castling;
        boardCopy.Move(move, isEnPassant, isCastling);

        // Check if the king is still in check after the move
        return !IsKingInCheck(isWhite, boardCopy);
    }
    
    
    /// <summary>
    /// Checks if the given position is attacked by an opponent's pawn
    /// </summary>
    /// <param name="position">Board coordinate</param>
    /// <param name="gameboard">Current board state</param>
    /// <returns>True if an opponent pawn attacks the square</returns>
    private static bool CheckPawnAttack(string position, Gameboard gameboard)
    {
        var col = position[0];
        var row = position[1];

        // Black pawns sit above and attack downward into this square
        if (row < '8')
        {
            if (col > 'A')
            {
                var piece = gameboard.GetPieceAtPosition($"{(char)(col - 1)}{(char)(row + 1)}");
                if (piece is Pawn && !piece.IsWhite && MoveValidator.WhitePlayer)
                    return true;
            }

            if (col < 'H')
            {
                var piece = gameboard.GetPieceAtPosition($"{(char)(col + 1)}{(char)(row + 1)}");
                if (piece is Pawn && !piece.IsWhite && MoveValidator.WhitePlayer)
                    return true;
            }
        }

        // White pawns sit below and attack upward into this square
        if (row > '1')
        {
            if (col > 'A')
            {
                var piece = gameboard.GetPieceAtPosition($"{(char)(col - 1)}{(char)(row - 1)}");
                if (piece is Pawn && piece.IsWhite && !MoveValidator.WhitePlayer)
                    return true;
            }

            if (col < 'H')
            {
                var piece = gameboard.GetPieceAtPosition($"{(char)(col + 1)}{(char)(row - 1)}");
                if (piece is Pawn && piece.IsWhite && !MoveValidator.WhitePlayer)
                    return true;
            }
        }

        return false;
    }
    
    /// <summary>
    /// Check if the given position is attacked by an opponent's knight
    /// </summary>
    /// <param name="position">Board coordinate</param>
    /// <param name="gameboard">Current board state</param>
    /// <returns>True if an opponent knight attacks the square</returns>
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
            if (piece is Knight && (MoveValidator.WhitePlayer ? !piece.IsWhite : piece.IsWhite))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Check if the given position is attacked by an opponent's bishop or queen along the diagonals
    /// </summary>
    /// <param name="position">Board coordinate</param>
    /// <param name="gameboard">Current board state</param>
    /// <returns>True if attacked diagonally by an opponent bishop or queen</returns>
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
                            (MoveValidator.WhitePlayer ? !piece.IsWhite : piece.IsWhite))
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
    
    /// <summary>
    /// Check if the given position is attacked by an opponent's rook or queen vertically or horizontally
    /// </summary>
    /// <param name="position">Board coordinate</param>
    /// <param name="gameboard">Current board state</param>
    /// <returns>True if attacked in a line by an opponent rook or queen</returns>
    private static bool CheckRookQueenAttack(string position, Gameboard gameboard)
    {
        for (var i = -1; i <= 1; i += 2)
        {
            var newCol = (char)(position[0] + i);
            var newRow = (char)(position[1] + i);
            for (var col = newCol; col is >= 'A' and <= 'H'; col = (char)(col + i))
            {
               var piece = gameboard.GetPieceAtPosition($"{col}{position[1]}");
               if (piece is (Rook or Queen) && (MoveValidator.WhitePlayer ? !piece.IsWhite : piece.IsWhite))
               {
                   return true;
               }
               if (piece != null) break; // Blocked by another piece
            }

            for (var row = newRow; row is >= '1' and <= '8'; row = (char)(row + i))
            {
               var piece = gameboard.GetPieceAtPosition($"{position[0]}{row}");
               if (piece is (Rook or Queen) && (MoveValidator.WhitePlayer ? !piece.IsWhite : piece.IsWhite))
               {
                   return true;
               }
               if (piece != null) break; // Blocked by another piece
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if the given position is attacked by an opponent's king
    /// </summary>
    /// <param name="position">Board coordinate</param>
    /// <param name="gameboard">Current board state</param>
    /// <returns>True if an opponent king attacks the square</returns>
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
                if (piece is King && (MoveValidator.WhitePlayer ? !piece.IsWhite : piece.IsWhite))
                {
                    return true;
                }
            }
        }

        return false;
    }
}