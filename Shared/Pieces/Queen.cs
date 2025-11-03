namespace Shared.Pieces;

/// <summary>
/// Represents a Queen piece.
/// </summary>
/// <param name="position">The initial position of the queen.</param>
/// <param name="isWhite">A value indicating whether the queen is white.</param>
public class Queen(string position, bool isWhite) : Piece(position, "Queen", isWhite)
{
    /// <summary>
    /// Gets the Unicode symbol for the queen.
    /// </summary>
    public override string UnicodeSymbol => "♛";
}