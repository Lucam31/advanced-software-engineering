namespace Shared.Pieces;
using Pieces;

public class Rook : Pieces.Piece
{
   
    public Rook(string position, bool isWhite, bool isCaptured) : base(position, "Rook", isWhite, isCaptured)
    {
    }
    public override string UnicodeSymbol => IsWhite ? "\u2656" : "\u265C";
}