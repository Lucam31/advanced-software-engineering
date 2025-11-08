using System.Text.Json.Serialization;

namespace Shared.WebSocketMessages;

/// <summary>
/// Represents the payload for a request to join a game.
/// </summary>
public class JoinGamePayload
{
    /// <summary>
    /// The unique identifier of the game to join.
    /// </summary>
    [JsonPropertyName("gameId")]
    public Guid GameId { get; set; }
}