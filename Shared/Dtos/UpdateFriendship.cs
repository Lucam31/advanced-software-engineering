using System.Text.Json.Serialization;
using Shared.Models;

namespace Shared.Dtos;

/// <summary>
/// Represents a data transfer object for updating the status of a friendship.
/// </summary>
public class UpdateFriendship
{
    /// <summary>
    /// Gets or sets the unique identifier of the friendship to update.
    /// </summary>
    [JsonPropertyName("friendshipId")]
    public Guid FriendshipId { get; set; }
    
    /// <summary>
    /// Gets or sets the new status for the friendship.
    /// </summary>
    [JsonPropertyName("status")]
    public FriendshipStatus Status { get; set; }
}