using System.Text.Json.Serialization;

namespace Shared.WebSocketMessages;

public class CreateGamePayload
{
    [JsonPropertyName("opponentId")]
    public Guid OpponentId { get; set; }
}