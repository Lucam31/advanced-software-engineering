using System.Text.Json.Serialization;

namespace Shared.WebSocketMessages;

/// <summary>
/// Represents the acknowledgment payload sent in response to a game turn message over WebSocket.
/// </summary>
public class GameTurnAckPayload
{
    /// <summary>
    /// The unique identifier of the game.
    /// </summary>
    [JsonPropertyName("gameId")]
    public Guid GameId { get; set; }
}