namespace Shared.Pieces;

/// <summary>
/// Represents a Pawn piece.
/// </summary>
/// <param name="position">The initial position of the pawn.</param>
/// <param name="isWhite">A value indicating whether the pawn is white.</param>
public class Pawn(string position, bool isWhite) : Piece(position, "Pawn", isWhite)
{
    /// <summary>
    /// Gets the Unicode symbol for the pawn.
    /// </summary>
    public override string UnicodeSymbol => "♟";
}