namespace Shared.Pieces;

public class Knight(string position, bool isWhite) : Piece(position, "Knight", isWhite)
{
    public override string UnicodeSymbol => "♞";
}