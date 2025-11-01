namespace Shared;

using Shared.Pieces;

public class Tile(int row, int col, bool isWhite, Piece? piece = null)
{
    public int Row { get; } = row;
    public int Col { get; } = col;
    public bool IsWhite { get; } = isWhite;

    public Piece? CurrentPiece { get; set; } = piece;

    public bool IsOccupied => CurrentPiece != null;
}