namespace Shared.Pieces;

public class King(string position, bool isWhite) : Piece(position, "King", isWhite)
{
    public override string UnicodeSymbol => "♚";
    
}