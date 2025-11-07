using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.WebSocketMessages;

/// <summary>
/// Represents a generic WebSocket message with a type and payload.
/// </summary>
public class WebSocketMessage
{
    /// <summary>
    /// The type of the WebSocket message.
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; set; }
    /// <summary>
    /// The payload of the WebSocket message.
    /// </summary>
    [JsonPropertyName("payload")]
    public JsonElement Payload { get; set; }
}