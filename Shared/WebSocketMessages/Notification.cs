namespace Shared.WebSocketMessages;

/// <summary>
/// Represents a notification to be sent to a user.
/// </summary>
public class Notification
{
    /// <summary>
    /// The ID of the user to notify.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// The WebSocket message to send.
    /// </summary>
    public required WebSocketMessage Message { get; set; }
}
