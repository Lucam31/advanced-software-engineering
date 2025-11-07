using System.Threading.Channels;
using Shared.WebSocketMessages;

namespace chess_server.Services;

/// <summary>
/// Defines the interface for sending notifications to users via WebSocket.
/// </summary>
public interface INotificationSender
{
    /// <summary>
    /// Sends a notification message to a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user to notify.</param>
    /// <param name="message">The WebSocket message to send.</param>
    Task SendNotificationAsync(Guid userId, WebSocketMessage message);
}

/// <summary>
/// Implements notification sending by writing to a channel.
/// </summary>
public class NotificationSender : INotificationSender
{
    private readonly ChannelWriter<Notification> _notificationWriter;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationSender"/> class.
    /// </summary>
    /// <param name="notificationWriter">The channel writer for notifications.</param>
    public NotificationSender(ChannelWriter<Notification> notificationWriter)
    {
        _notificationWriter = notificationWriter;
    }

    /// <inheritdoc/>
    public async Task SendNotificationAsync(Guid userId, WebSocketMessage message)
    {
        var notification = new Notification { UserId = userId, Message = message };
        await _notificationWriter.WriteAsync(notification);
    }
}


