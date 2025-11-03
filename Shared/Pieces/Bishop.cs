namespace Shared.Pieces;

/// <summary>
/// Represents a Bishop piece.
/// </summary>
/// <param name="position">The initial position of the bishop.</param>
/// <param name="isWhite">A value indicating whether the bishop is white.</param>
public class Bishop(string position, bool isWhite) : Piece(position, "Bishop", isWhite)
{
    /// <summary>
    /// Gets the Unicode symbol for the bishop.
    /// </summary>
    public override string UnicodeSymbol => "♝";
}