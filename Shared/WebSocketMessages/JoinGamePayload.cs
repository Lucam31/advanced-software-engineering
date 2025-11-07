using System.Text.Json.Serialization;

namespace Shared.WebSocketMessages;

public class JoinGamePayload
{
    [JsonPropertyName("gameId")]
    public Guid GameId { get; set; }
}