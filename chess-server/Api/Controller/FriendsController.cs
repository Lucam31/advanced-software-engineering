using chess_server.Api.ActionResults;
using chess_server.Api.Attributes;
using chess_server.Services;
using Shared.Dtos;
using Shared.Logger;

namespace chess_server.Api.Controller;

/// <summary>
/// Defines the interface for the friends' controller.
/// </summary>
public interface IFriendsController
{
    /// <summary>
    /// Adds a new friend.
    /// </summary>
    /// <param name="dto">The friend request data.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
    Task<IActionResult> AddFriend(FriendRequest dto);
    
    /// <summary>
    /// Gets the list of friends for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>An <see cref="IActionResult"/> containing the list of friends.</returns>
    Task<IActionResult> GetFriends(Guid userId);
    
    /// <summary>
    /// Removes a friend from the user's friend list.
    /// </summary>
    /// <param name="friendshipId">The ID of the friendship to remove.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
    Task<IActionResult> RemoveFriend(Guid friendshipId);
}

/// <summary>
/// API controller for friend-related actions.
/// </summary>
[Route("/api/friends")]
public class FriendsController : IFriendsController
{
    private readonly IFriendsService _friendsService;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="FriendsController"/> class.
    /// </summary>
    /// <param name="friendsService">The friends service.</param>
    public FriendsController(IFriendsService friendsService)
    {
        _friendsService = friendsService;
    }
    
    /// <inheritdoc/>
    [HttpMethod("POST")]
    [Route("/add")]
    public async Task<IActionResult> AddFriend([FromBody] FriendRequest dto)
    {
        GameLogger.Info($"HTTP POST /api/friends/add by {dto.UserId} for '{dto.FriendUsername}'");
        await _friendsService.AddFriendAsync(dto);
        GameLogger.Info($"Friend request created by {dto.UserId} for '{dto.FriendUsername}'");
        return Results.Ok();
    }

    /// <inheritdoc/>
    [HttpMethod("GET")]
    [Route("/list")]
    public async Task<IActionResult> GetFriends([FromQuery] Guid userId)
    {
        GameLogger.Info($"HTTP GET /api/friends/list?userId={userId}");
        var friends = await _friendsService.GetFriendsAsync(userId);
        GameLogger.Info($"Returning {friends.Count} friends for user {userId}");
        return Results.Ok(friends);
    }

    /// <inheritdoc/>
    [HttpMethod("DELETE")]
    [Route("/remove")]
    public async Task<IActionResult> RemoveFriend([FromQuery] Guid friendshipId)
    {
        GameLogger.Info($"HTTP DELETE /api/friends/remove?friendshipId={friendshipId}");
        await _friendsService.RemoveFriendAsync(friendshipId);
        GameLogger.Info($"Friendship {friendshipId} removed");
        return Results.Ok();
    }
}