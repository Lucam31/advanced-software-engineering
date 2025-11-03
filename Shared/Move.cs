namespace Shared;

/// <summary>
/// Represents a single move in a chess game, from a starting position to an ending position.
/// </summary>
/// <param name="from">The starting position of the move in algebraic notation (e.g., "e2").</param>
/// <param name="to">The ending position of the move in algebraic notation (e.g., "e4").</param>
public readonly struct Move(string from, string to)
{
    /// <summary>
    /// Gets the starting position of the move.
    /// </summary>
    public readonly string From = from;
    
    /// <summary>
    /// Gets the ending position of the move.
    /// </summary>
    public readonly string To = to;
}