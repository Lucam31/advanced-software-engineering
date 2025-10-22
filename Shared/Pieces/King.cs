namespace Shared.Pieces;
using Pieces;

public class King : Pieces.Piece
{   
    public King(string position, bool isWhite, bool isCaptured) : base(position, "King", isWhite, isCaptured)
    {
    }
    public override string UnicodeSymbol => IsWhite ? "\u2654" : "\u265A";
}