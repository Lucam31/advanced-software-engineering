using chess_server.Repositories;
using Shared.Exceptions;
using Shared.InputDtos;
using chess_server.OutputDtos;
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
    /// Retrieves a list of pending friend requests for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of <see cref="PendingFriendRequest"/> objects.</returns>
    Task<List<PendingFriendRequest>> GetPendingFriendRequestsAsync(Guid userId);
    
    /// <summary>
    /// Updates the status of a friend request.
    /// </summary>
    /// <param name="dto">The data for updating the friendship status.</param>
    Task UpdateFriendRequestAsync(UpdateFriendship dto);
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
    /// <param name="friendsRepository">The friends repository.</param>
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
            Type = MessageType.FetchFriendRequest
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
    public async Task<List<PendingFriendRequest>> GetPendingFriendRequestsAsync(Guid userId)
    {
        GameLogger.Info($"Fetching pending friend requests for user {userId}");
        var pendingRequests = await _friendsRepository.GetPendingFriendRequestsAsync(userId);
        
        var result = new List<PendingFriendRequest>();
        
        foreach (var pr in pendingRequests)
        {
            var requesterId = pr.InitiatedBy;
            var requester = await _userRepository.GetUserByIdAsync(requesterId);
            
            if (requester != null)
            {
                result.Add(new PendingFriendRequest
                {
                    RequestId = pr.Id,
                    FromUsername = requester.Username,
                    Status = pr.Status
                });
            }
        }
        
        GameLogger.Info($"Found {result.Count} pending friend requests for user {userId}");
        return result;
    }

    /// <inheritdoc/>
    public async Task UpdateFriendRequestAsync(UpdateFriendship dto)
    {
        GameLogger.Info($"Updating friend request {dto.FriendshipId} to status {dto.Status}");
        await _friendsRepository.UpdateFriendshipStatusAsync(dto);
        GameLogger.Info($"Friend request {dto.FriendshipId} updated successfully.");
    }
}
