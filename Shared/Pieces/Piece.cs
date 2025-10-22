using System.Text.RegularExpressions;

namespace Shared.Pieces;
using Shared;

public abstract class Piece
{
    private readonly MoveValidator _mv = new MoveValidator();
    
    public string Name { get; private set; }
    public string Position { get; private set; }
    protected bool IsWhite { get; private set; }
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

    public bool Move(string newPosition)
    {
        if (_mv.ValidateMove(this, newPosition))
        {
            this.Position = newPosition;
            Moved = true;
            return true;
        }
        return false;
    }
}