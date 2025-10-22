namespace Shared.Pieces;
using Pieces;

public class Knight : Pieces.Piece
{
    public Knight(string position, bool isWhite, bool isCaptured) : base(position, "Knight", isWhite, isCaptured)
    {
    }
    public override string UnicodeSymbol => IsWhite ? "\u2658" : "\u265E";
}