namespace Shared.InputDtos;

/// <summary>
/// Represents a data transfer object for sending a friend request.
/// </summary>
public class FriendRequest
{
    /// <summary>
    /// Gets or sets the ID of the user sending the request.
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Gets or sets the username of the user to whom the friend request is sent.
    /// </summary>
    public string FriendUsername { get; set; } = string.Empty;
}