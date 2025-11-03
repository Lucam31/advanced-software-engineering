namespace Shared.InputDtos;

/// <summary>
/// Represents a data transfer object for inserting a new game record.
/// </summary>
public class InsertGame
{
    /// <summary>
    /// Gets or sets the unique identifier for the game.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the player playing as white.
    /// </summary>
    public Guid WhitePlayerId { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the player playing as black.
    /// </summary>
    public Guid BlackPlayerId { get; set; }
    
    /// <summary>
    /// Gets or sets the list of moves made in the game in algebraic notation.
    /// </summary>
    public List<string> Moves { get; set; } = new();
}