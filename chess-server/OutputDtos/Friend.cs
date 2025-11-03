namespace chess_server.OutputDtos;

/// <summary>
/// Represents a data transfer object for a friend.
/// </summary>
public class Friend
{
    /// <summary>
    /// Gets or sets the ID of the friendship.
    /// </summary>
    public Guid FriendshipId { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the friend.
    /// </summary>
    public string Name { get; set; } = "";
}