using System.Text.Json.Serialization;

namespace Shared.WebSocketMessages;

/// <summary>
/// Represents the payload for creating a new game, containing the opponent's unique identifier.
/// </summary>
public class CreateGamePayload
{
    /// <summary>
    /// The unique identifier of the opponent player.
    /// </summary>
    [JsonPropertyName("opponentId")]
    public Guid OpponentId { get; set; }
}