using chess_server.Repositories;
using chess_server.OutputDtos;
using Shared.InputDtos;

namespace chess_server_tests.Mocks;

public class MockFriendsRepository : IFriendsRepository
{
    public Guid FriendshipIdToReturn { get; set; } = Guid.NewGuid();
    public List<Friend> FriendsToReturn { get; set; } = new List<Friend>();
    public List<PendingFriendRequest> PendingRequestsToReturn { get; set; } = new List<PendingFriendRequest>();
    public UpdateFriendship LastUpdatedFriendship { get; private set; }
    public Guid LastFromUserId { get; private set; }
    public Guid LastToUserId { get; private set; }

    public Task<Guid> AddFriendshipAsync(Guid fromUserId, Guid toUserId)
    {
        LastFromUserId = fromUserId;
        LastToUserId = toUserId;
        return Task.FromResult(FriendshipIdToReturn);
    }

    public Task<List<Friend>> GetFriendsAsync(Guid userId)
    {
        return Task.FromResult(FriendsToReturn);
    }

    public Task<List<PendingFriendRequest>> GetPendingFriendRequestsAsync(Guid userId)
    {
        return Task.FromResult(PendingRequestsToReturn);
    }

    public Task UpdateFriendshipStatusAsync(UpdateFriendship dto)
    {
        LastUpdatedFriendship = dto;
        return Task.CompletedTask;
    }
}