namespace Shared.Pieces;

/// <summary>
/// Represents a Rook piece.
/// </summary>
/// <param name="position">The initial position of the rook.</param>
/// <param name="isWhite">A value indicating whether the rook is white.</param>
public class Rook(string position, bool isWhite) : Piece(position, "Rook", isWhite)
{
    /// <summary>
    /// Gets the Unicode symbol for the rook.
    /// </summary>
    public override string UnicodeSymbol => "♜";
    
}