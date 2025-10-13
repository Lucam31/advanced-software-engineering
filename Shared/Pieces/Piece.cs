namespace Shared.Pieces;
using Shared;

public abstract class Piece
{
    private readonly MoveValidator _mv = new MoveValidator();
    
    public string Name { get; set; }
    public string Position { get; set; }
    public bool IsWhite { get; set; }
    public bool IsCaptured { get; set; }
    public bool Moved { get; set; }

    protected Piece(string position, string name, bool isWhite, bool isCaptured)
    {
        Name = name;
        Position = position;
        IsWhite = isWhite;
        IsCaptured = isCaptured;
        Moved = false;
    }

    public bool Move(string newPosition)
    {
        if (_mv.ValidateMove(this, newPosition))
        {
            this.Position = newPosition;
            return true;
        }
        return false;
    }
}