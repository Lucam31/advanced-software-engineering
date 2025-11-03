namespace Shared.Pieces;

public abstract class Piece(string position, string name, bool isWhite, bool isCaptured = false)
{
    public string Name { get; private set; } = name;
    public string Position { get; private set; } = position;
    public bool IsWhite { get; private set; } = isWhite;

    public bool IsCaptured { get; set; } = isCaptured;
    public bool Moved { get; private set; } = false;

    public abstract string UnicodeSymbol { get; }

    public override string ToString() => UnicodeSymbol;

    public void Move(string newPosition)
    {
        Position = newPosition;
        Moved = true;
    }
}