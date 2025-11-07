using System.Text.Json.Serialization;

namespace Shared.WebSocketMessages;

/// <summary>
/// Represents the payload for making a move in a game.
/// </summary>
public class MakeMovePayload
{
    /// <summary>
    /// The unique identifier of the game.
    /// </summary>
    [JsonPropertyName("gameId")]
    public Guid GameId { get; set; }
    /// <summary>
    /// The move to be made, e.g. e2e3.
    /// </summary>
    [JsonPropertyName("move")]
    public required string Move { get; set; }
}