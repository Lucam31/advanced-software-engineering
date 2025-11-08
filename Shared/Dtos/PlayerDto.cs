using System.Text.Json.Serialization;

namespace Shared.Dtos;

/// <summary>
/// Represents a data transfer object for a player in a chess game.
/// </summary>
public class PlayerDto
{
    /// <summary>
    /// The name of the player.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    /// <summary>
    /// Indicates whether the player is playing as white.
    /// </summary>
    [JsonPropertyName("isWhite")]
    public bool IsWhite { get; set; }
    /// <summary>
    /// The rating of the player.
    /// </summary>
    [JsonPropertyName("rating")]
    public int Rating { get; set; }
}