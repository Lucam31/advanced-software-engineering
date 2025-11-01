namespace Shared.Pieces;

public class Queen(string position, bool isWhite) : Piece(position, "Queen", isWhite)
{
    public override string UnicodeSymbol => "♛";
    
    public override string UnicodeSymbol => IsWhite ? "\u2655" : "\u265B";
}