using Shared.Pieces;

namespace Shared;

/// <summary>
/// Represents a single move in a chess game, from a starting position to an ending position
/// </summary>
/// <param name="from">The starting position of the move in algebraic notation (e.g., "e2")</param>
/// <param name="to">The ending position of the move in algebraic notation (e.g., "e4")</param>
/// <param name="piece">The Piece that was captured during the move</param>
/// <param name="capturedPosition">The position of the captured Piece</param>
public readonly struct Move(string from, string to, Piece? piece = null, string? capturedPosition = null)
{
    /// <summary>
    /// Gets the starting position of the move
    /// </summary>
    public readonly string From = from;
    
    /// <summary>
    /// Gets the ending position of the move
    /// </summary>
    public readonly string To = to;
    
    /// <summary>
    /// Gets the piece that was captured during the move, can be null
    /// </summary>
    public readonly Piece? CapturedPiece = piece;
    
    /// <summary>
    /// Gets the position of the captured piece, can be null
    /// </summary>
    public readonly string? CapturedPosition = capturedPosition;
    
    /// <summary>
    /// Overrides the ToString method to provide a string representation of the move
    /// </summary>
    /// <returns>string</returns>
    public override string ToString() => $"{From}->{To}";
}