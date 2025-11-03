namespace chess_server.Models;

/// <summary>
/// Represents a recorded chess game.
/// </summary>
public class Game
{
    /// <summary>
    /// Gets or sets the unique identifier for the game.
    /// </summary>
    public Guid Guid { get; init; }
    
    /// <summary>
    /// Gets or sets the ID of the player who played as white.
    /// </summary>
    public required Guid WhitePlayerId { get; init; }
    
    /// <summary>
    /// Gets or sets the ID of the player who played as black.
    /// </summary>
    public required Guid BlackPlayerId { get; init; }
    
    /// <summary>
    /// Gets or sets the list of moves made during the game in algebraic notation.
    /// </summary>
    public required List<string> Moves { get; set; }
}