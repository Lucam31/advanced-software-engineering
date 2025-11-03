namespace Shared;

using Pieces;

/// <summary>
/// Represents a single tile on the chessboard.
/// </summary>
/// <param name="row">The zero-based row index of the tile.</param>
/// <param name="col">The zero-based column index of the tile.</param>
/// <param name="isWhite">A value indicating whether the tile is white.</param>
/// <param name="piece">The piece currently on the tile, if any.</param>
public class Tile(int row, int col, bool isWhite, Piece? piece = null)
{
    /// <summary>
    /// Gets the row index of the tile.
    /// </summary>
    public int Row { get; } = row;
    
    /// <summary>
    /// Gets the column index of the tile.
    /// </summary>
    public int Col { get; } = col;
    
    /// <summary>
    /// Gets a value indicating whether the tile is white.
    /// </summary>
    public bool IsWhite { get; } = isWhite;

    /// <summary>
    /// Gets or sets the piece currently on this tile.
    /// </summary>
    public Piece? CurrentPiece { get; set; } = piece;

    /// <summary>
    /// Gets a value indicating whether the tile is occupied by a piece.
    /// </summary>
    public bool IsOccupied => CurrentPiece != null;
}