namespace Shared.Pieces;
using Pieces;

public class Pawn : Pieces.Piece
{
    public Pawn(string position, bool isWhite, bool isCaptured) : base(position, "Pawn", isWhite, isCaptured)
    {
    }
    public override string UnicodeSymbol => IsWhite ? "\u2659" : "\u265F";
}