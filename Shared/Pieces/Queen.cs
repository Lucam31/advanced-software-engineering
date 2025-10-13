namespace Shared.Pieces;
using Pieces;

public class Queen : Pieces.Piece
{
    public Queen(string position, bool isWhite, bool isCaptured) : base(position, "Queen", isWhite, isCaptured)
    {
    }
    
}