namespace Shared.Pieces;

public class Pawn : Piece
{
    private bool _enPassant;
    public Pawn(string position, bool isWhite, bool isCaptured) : base(position, "Pawn", isWhite, isCaptured)
    {
    }
    public override string UnicodeSymbol => IsWhite ? "\u2659" : "\u265F";
    
    public override void Move(string newPosition)
    {
        Position = newPosition;
        if (_enPassant == false && Moved == false)
        {
            _enPassant = true;
        }
        else _enPassant = false;
        Moved = true;
    }
}