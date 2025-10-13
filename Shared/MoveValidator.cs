namespace Shared;

using Shared.Pieces;

public class MoveValidator
{
    public bool ValidateMove(Piece piece, string newPosition)
    {
        int fileDiff = Math.Abs(newPosition[0] - piece.Position[0]);
        int rankDiff = Math.Abs(newPosition[1] - piece.Position[1]);

        if (piece is Pawn)
        {
            if (fileDiff > 0) return false; // Pawns can't move sideways unless capturing, not yet handled here
            if (piece.Moved)
            {
                return (0 < rankDiff && rankDiff < 3);
            }

            return (0 < rankDiff && rankDiff < 2);
        }

        if (piece is Bishop)
        {
            // Bishop moves diagonally, so the difference between the file and rank should be the same
            return fileDiff == rankDiff;
        }


        if (piece is Rook)
        {
            return (fileDiff == 0 && rankDiff != 0) || (fileDiff != 0 && rankDiff == 0);
        }

        if (piece is Knight)
        {
            return (fileDiff == 2 && rankDiff == 1) || (fileDiff == 1 && rankDiff == 2);
        }

        if (piece is Queen)
        {
            return (fileDiff == rankDiff) || (fileDiff == 0 || rankDiff != 0);
        }

        if (piece is King)
        {
            return (fileDiff == 0 && rankDiff == 1) || (fileDiff == 1 && rankDiff == 0) ||
                   (fileDiff == 1 && rankDiff == 1);
        }

        return false;
    }
}