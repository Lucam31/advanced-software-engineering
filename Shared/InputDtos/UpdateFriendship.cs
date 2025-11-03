using Shared.Models;

namespace Shared.InputDtos;

/// <summary>
/// Represents a data transfer object for updating the status of a friendship.
/// </summary>
public class UpdateFriendship
{
    /// <summary>
    /// Gets or sets the unique identifier of the friendship to update.
    /// </summary>
    public Guid FriendshipId { get; set; }
    
    /// <summary>
    /// Gets or sets the new status for the friendship.
    /// </summary>
    public FriendshipStatus Status { get; set; }
}