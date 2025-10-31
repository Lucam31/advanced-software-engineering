namespace Shared.Pieces;

public class Bishop(string position, bool isWhite) : Piece(position, "Bishop", isWhite)
{
    public override string UnicodeSymbol => "♝";
}