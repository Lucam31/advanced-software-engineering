namespace Shared.Pieces;

using Shared;

public abstract class Piece(string position, string name, bool isWhite, bool isCaptured = false)
{
    private readonly MoveValidator _mv = new MoveValidator();

    public string Name { get; private set; } = name;
    public string Position { get; private set; } = position;
    public bool IsWhite { get; private set; } = isWhite;

    public bool IsCaptured { get; set; } = isCaptured;
    public bool Moved { get; private set; } = false;

    public abstract string UnicodeSymbol { get; }

    public override string ToString() => UnicodeSymbol;

    public bool Move(string newPosition)
    {
        if (!_mv.ValidateMove(this, newPosition)) return false;
        Position = newPosition;
        Moved = true;
        return true;
    }
}