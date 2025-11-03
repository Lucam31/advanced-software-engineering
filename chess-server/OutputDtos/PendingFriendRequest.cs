namespace chess_server.OutputDtos;

/// <summary>
/// Represents a data transfer object for a pending friend request.
/// </summary>
public class PendingFriendRequest
{
    /// <summary>
    /// Gets or sets the ID of the friend request.
    /// </summary>
    public Guid RequestId { get; set; }

    /// <summary>
    /// Gets or sets the username of the user who sent the request.
    /// </summary>
    public string FromUsername { get; set; } = "";

    /// <summary>
    /// Gets or sets the status of the request.
    /// </summary>
    public string Status { get; set; } = " ";
}