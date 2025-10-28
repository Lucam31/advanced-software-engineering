using System.Text.RegularExpressions;

namespace Shared.Pieces;
using Shared;

public abstract class Piece
{
    
    public string Name { get; private set; }
    public string Position { get; private set; }
    public bool IsWhite { get; private set; }
    public bool IsCaptured { get; set; }
    public bool Moved { get; private set; }

    protected Piece(string position, string name, bool isWhite, bool isCaptured)
    {
        Name = name;
        Position = position;
        IsWhite = isWhite;
        IsCaptured = isCaptured;
        Moved = false;
    }
    
    public abstract string UnicodeSymbol { get; }
    
    public override string ToString() => UnicodeSymbol;

    public void Move(string newPosition)
    {
        this.Position = newPosition;
        Moved = true;
    }
}