using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.WebSocketMessages;

public class WebSocketMessage
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }
    [JsonPropertyName("payload")]
    public JsonElement Payload { get; set; }
}