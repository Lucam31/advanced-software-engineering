using System.Text.Json.Serialization;

namespace Shared.Dtos;

/// <summary>
/// Represents a data transfer object for inserting a new game record.
/// </summary>
public class InsertGame
{
    /// <summary>
    /// Gets or sets the unique identifier for the game.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the player playing as white.
    /// </summary>
    [JsonPropertyName("whitePlayerId")]
    public Guid WhitePlayerId { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the player playing as black.
    /// </summary>
    [JsonPropertyName("blackPlayerId")]
    public Guid BlackPlayerId { get; set; }
    
    /// <summary>
    /// Gets or sets the list of moves made in the game in algebraic notation.
    /// </summary>
    [JsonPropertyName("moves")]
    public List<string> Moves { get; set; } = new();
}