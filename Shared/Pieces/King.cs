namespace Shared.Pieces;

/// <summary>
/// Represents a King piece.
/// </summary>
/// <param name="position">The initial position of the king.</param>
/// <param name="isWhite">A value indicating whether the king is white.</param>
public class King(string position, bool isWhite) : Piece(position, "King", isWhite)
{
    /// <summary>
    /// Gets the Unicode symbol for the king.
    /// </summary>
    public override string UnicodeSymbol => "♚";
    
}