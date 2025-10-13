namespace Shared.Pieces;
using Pieces;

public class Pawn : Pieces.Piece
{
    public Pawn(string position, bool isWhite, bool isCaptured) : base(position, "Pawn", isWhite, isCaptured)
    {
    }
    
}