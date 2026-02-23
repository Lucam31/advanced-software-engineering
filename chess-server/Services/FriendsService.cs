using chess_server.Repositories;
using Shared.Exceptions;
using chess_server.OutputDtos;
using Shared.Dtos;
using Shared.Logger;
using Shared.Models;
using Shared.WebSocketMessages;

namespace chess_server.Services;

/// <summary>
/// Defines the interface for friend-related business logic.
/// </summary>
public interface IFriendsService
{
    /// <summary>
    /// Sends a friend request.
    /// </summary>
    /// <param name="dto">The friend request data.</param>
    Task AddFriendAsync(FriendRequest dto);
    
    /// <summary>
    /// Retrieves a list of a user's friends.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of <see cref="Friend"/> objects.</returns>
    Task<List<Friend>> GetFriendsAsync(Guid userId);
    
    /// <summary>
    /// Removes a friend from the user's friend list.
    /// </summary>
    /// <param name="friendshipId">The ID of the user.</param>
    Task RemoveFriendAsync(Guid friendshipId);
}

/// <summary>
/// Implements the business logic for friend-related operations.
/// </summary>
public class FriendsService : IFriendsService
{
    private readonly IFriendsRepository _friendsRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationSender _notificationSender;

    /// <summary>
    /// Initializes a new instance of the <see cref="FriendsService"/> class.
    /// </summary>
    /// <param name="friendsRepository">The friends' repository.</param>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="notificationSender">The notification sender.</param>
    public FriendsService(IFriendsRepository friendsRepository, IUserRepository userRepository, INotificationSender notificationSender)
    {
        _friendsRepository = friendsRepository;
        _userRepository = userRepository;
        _notificationSender = notificationSender;
    }

    /// <inheritdoc/>
    public async Task AddFriendAsync(FriendRequest dto)
    {
        GameLogger.Info($"User {dto.UserId} is attempting to add friend {dto.FriendUsername}");
        var friend = await _userRepository.GetUserByUsernameAsync(dto.FriendUsername);

        if (friend == null)
        {
            GameLogger.Warning($"Add friend failed: User '{dto.FriendUsername}' not found.");
            throw new UserNotFound();
        }

        await _friendsRepository.AddFriendshipAsync(dto.UserId, friend.Id);
        GameLogger.Info($"Friend request sent from {dto.UserId} to {friend.Id}");

        // Send notification to the friend if online
        await _notificationSender.SendNotificationAsync(friend.Id, new WebSocketMessage
        {
            Type = MessageType.FetchFriends
        });
    }

    /// <inheritdoc/>
    public async Task<List<Friend>> GetFriendsAsync(Guid userId)
    {
        GameLogger.Info($"Fetching friends for user {userId}");
        var friendships = await _friendsRepository.GetFriendsAsync(userId);

        var friends = new List<Friend>();

        foreach (var f in friendships)
        {
            var friendId = f.UserId1 == userId ? f.UserId2 : f.UserId1;
            var friendUser = await _userRepository.GetUserByIdAsync(friendId);
            if (friendUser != null)
            {
                friends.Add(new Friend
                {
                    FriendshipId = f.Id,
                    Name = friendUser.Username
                });
            }
        }

        GameLogger.Info($"Found {friends.Count} friends for user {userId}");
        return friends;
    }

    /// <inheritdoc/>
    public async Task RemoveFriendAsync(Guid friendshipId)
    {
        GameLogger.Info($"Removing friendship with ID {friendshipId}");
        await _friendsRepository.RemoveFriendshipAsync(friendshipId);
        GameLogger.Info($"Friendship with ID {friendshipId} removed");
    }
}