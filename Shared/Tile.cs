namespace Shared;
using Shared.Pieces;

public class Tile
{
    public Piece? currentPiece { get; set; }
    public string Position { get; set; }
    public bool IsOccupied => currentPiece != null;
    
}