namespace Shared.Pieces;

public class Pawn(string position, bool isWhite) : Piece(position, "Pawn", isWhite)
{
    public override string UnicodeSymbol => "♟";
}