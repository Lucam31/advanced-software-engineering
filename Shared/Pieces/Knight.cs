namespace Shared.Pieces;

/// <summary>
/// Represents a Knight piece.
/// </summary>
/// <param name="position">The initial position of the knight.</param>
/// <param name="isWhite">A value indicating whether the knight is white.</param>
public class Knight(string position, bool isWhite) : Piece(position, "Knight", isWhite)
{
    /// <summary>
    /// Gets the Unicode symbol for the knight.
    /// </summary>
    public override string UnicodeSymbol => "♞";
}