using System.Text.RegularExpressions;

namespace Shared.Pieces;
using Shared;

public abstract class Piece
{
    
    public string Name { get; protected set; }
    public string Position { get; protected set; }
    public bool IsWhite { get; protected set; }
    public bool IsCaptured { get; set; }
    public bool Moved { get; protected set; }

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

    public virtual void Move(string newPosition)
    {
        this.Position = newPosition;
        Moved = true;
    }
}