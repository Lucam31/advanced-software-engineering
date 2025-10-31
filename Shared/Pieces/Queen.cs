namespace Shared.Pieces;

public class Queen(string position, bool isWhite) : Piece(position, "Queen", isWhite)
{
    public override string UnicodeSymbol => "♛";
    
}