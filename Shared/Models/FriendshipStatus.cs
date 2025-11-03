namespace Shared.Models;

/// <summary>
/// Defines the possible statuses of a friendship.
/// </summary>
public enum FriendshipStatus
{
    /// <summary>
    /// The friendship request is pending and has not been accepted or declined.
    /// </summary>
    Pending,
    
    /// <summary>
    /// The friendship request has been accepted.
    /// </summary>
    Accepted,
    
    /// <summary>
    /// The friendship request has been declined.
    /// </summary>
    Declined,
    
    /// <summary>
    /// The friendship has been removed by one of the users.
    /// </summary>
    Removed
}
