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
        var friends = await _friendsRepository.GetFriendsAsync(userId);
        
        return friends;
    }

    public async Task<List<PendingFriendRequest>> GetPendingFriendRequestsAsync(Guid userId)
    {
        return await _friendsRepository.GetPendingFriendRequestsAsync(userId);
    }

    public async Task UpdateFriendRequestAsync(UpdateFriendship dto)
    {
        await _friendsRepository.UpdateFriendshipStatusAsync(dto);
    }
}
