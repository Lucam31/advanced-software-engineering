namespace Shared.Pieces;

public class Rook(string position, bool isWhite) : Piece(position, "Rook", isWhite)
{
    public override string UnicodeSymbol => "♜";
    
}