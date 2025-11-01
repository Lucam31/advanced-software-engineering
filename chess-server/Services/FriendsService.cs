using chess_server.Repositories;
using Shared.Exceptions;
using Shared.InputDtos;
using chess_server.OutputDtos;
using Shared.Models;

namespace chess_server.Services;

public interface IFriendsService
{
    Task AddFriendAsync(FriendRequest dto);
    Task<List<Friend>> GetFriendsAsync(Guid userId);
    Task<List<PendingFriendRequest>> GetPendingFriendRequestsAsync(Guid userId);
    Task UpdateFriendRequestAsync(UpdateFriendship dto);
}

public class FriendsService : IFriendsService
{
    private readonly IFriendsRepository _friendsRepository;
    private readonly IUserRepository _userRepository;
    
    public FriendsService(IFriendsRepository friendsRepository, IUserRepository userRepository)
    {
        _friendsRepository = friendsRepository;
        _userRepository = userRepository;
    }

    public async Task AddFriendAsync(FriendRequest dto)
    {
        var friend = await _userRepository.GetUserByUsernameAsync(dto.FriendUsername);

        if (friend == null)
            throw new UserNotFound();
        
        await _friendsRepository.AddFriendshipAsync(dto.UserId, friend.Id);
    }
    
    public async Task<List<Friend>> GetFriendsAsync(Guid userId)
    {
        var friendships = await _friendsRepository.GetFriendsAsync(userId);

        var friends = new List<Friend>();
        
        foreach (var f in friendships)
        {
            friends.Add(new Friend
            {
                FriendshipId = f.Id,
                Name = f.UserId1 == userId ? 
                    (await _userRepository.GetUserByIdAsync(f.UserId2))!.Username : 
                    (await _userRepository.GetUserByIdAsync(f.UserId1))!.Username
            });
        }
        
        return friends;
    }

    public async Task<List<PendingFriendRequest>> GetPendingFriendRequestsAsync(Guid userId)
    {
        var pendingRequests = await _friendsRepository.GetPendingFriendRequestsAsync(userId);
        
        var result = new List<PendingFriendRequest>();
        
        foreach (var pr in pendingRequests)
        {
            var requesterId = pr.InitiatedBy == userId ? pr.UserId2 : pr.UserId1;
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
        
        return result;
    }

    public async Task UpdateFriendRequestAsync(UpdateFriendship dto)
    {
        await _friendsRepository.UpdateFriendshipStatusAsync(dto);
    }
}
