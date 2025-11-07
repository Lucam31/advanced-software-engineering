using System.Text.Json.Serialization;

namespace Shared.WebSocketMessages;

public class GameInvitationPayload
{
    [JsonPropertyName("gameId")]
    public Guid GameId { get; set; }
    [JsonPropertyName("inviterId")]
    public Guid InviterId { get; set; }
}