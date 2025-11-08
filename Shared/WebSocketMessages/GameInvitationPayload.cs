using System.Text.Json.Serialization;

namespace Shared.WebSocketMessages;

/// <summary>
/// Represents the payload for a game invitation message.
/// </summary>
public class GameInvitationPayload
{
    /// <summary>
    /// The unique identifier of the game.
    /// </summary>
    [JsonPropertyName("gameId")]
    public Guid GameId { get; set; }
    /// <summary>
    /// The unique identifier of the inviter.
    /// </summary>
    [JsonPropertyName("inviterId")]
    public Guid InviterId { get; set; }
}