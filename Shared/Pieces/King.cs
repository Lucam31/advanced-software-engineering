namespace Shared.Pieces;
using Pieces;

public class King : Pieces.Piece
{
    public King(string position, bool isWhite, bool isCaptured) : base(position, "King", isWhite, isCaptured)
    {
    }
    
}