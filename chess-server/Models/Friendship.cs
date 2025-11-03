namespace chess_server.Models;

/// <summary>
/// Represents a friendship between two users.
/// </summary>
public class Friendship
{
    /// <summary>
    /// Gets or sets the unique identifier for the friendship.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the first user in the friendship.
    /// </summary>
    public Guid UserId1 { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the second user in the friendship.
    /// </summary>
    public Guid UserId2 { get; set; }
    
    /// <summary>
    /// Gets or sets the status of the friendship (e.g., "pending", "accepted").
    /// </summary>
    public string Status { get; set; } = "pending";
    
    /// <summary>
    /// Gets or sets the ID of the user who initiated the friendship.
    /// </summary>
    public Guid InitiatedBy { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp when the friendship was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
