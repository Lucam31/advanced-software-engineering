using chess_server.Services;
using chess_server_tests.Mocks;
using Shared.Exceptions;
using Shared.InputDtos;
using chess_server.Models;
using chess_server.OutputDtos;

namespace chess_server_tests.UnitTests;

[TestClass]
public sealed class FriendsServiceTests
{
    private MockFriendsRepository _mockFriendsRepository;
    private MockUserRepository _mockUserRepository;
    private FriendsService _service;

    [TestInitialize]
    public void Setup()
    {
        _mockFriendsRepository = new MockFriendsRepository();
        _mockUserRepository = new MockUserRepository();
        _service = new FriendsService(_mockFriendsRepository, _mockUserRepository);
    }

    [TestMethod]
    public async Task AddFriendAsync_UserExists_AddsFriendship()
    {
        var dto = new FriendRequest { UserId = Guid.NewGuid(), FriendUsername = "frienduser" };
        _mockUserRepository.UserToReturn = new User { Id = Guid.NewGuid(), Username = "frienduser", PasswordHash = new byte[0], PasswordSalt = new byte[0], Rating = 1200 };

        await _service.AddFriendAsync(dto);

        Assert.AreEqual(dto.UserId, _mockFriendsRepository.LastFromUserId);
        Assert.AreEqual(_mockUserRepository.UserToReturn.Id, _mockFriendsRepository.LastToUserId);
    }

    [TestMethod]
    [ExpectedException(typeof(UserNotFound))]
    public async Task AddFriendAsync_UserNotFound_ThrowsException()
    {
        var dto = new FriendRequest { UserId = Guid.NewGuid(), FriendUsername = "nonexistent" };
        _mockUserRepository.UserToReturn = null;

        await _service.AddFriendAsync(dto);
    }

    [TestMethod]
    public async Task GetFriendsAsync_ReturnsFriendsList()
    {
        var userId = Guid.NewGuid();
        var expectedFriends = new List<Friend>
        {
            new() { Name = "friend1", FriendshipId = Guid.NewGuid() }
        };
        _mockFriendsRepository.FriendsToReturn = expectedFriends;

        var result = await _service.GetFriendsAsync(userId);

        CollectionAssert.AreEqual(expectedFriends, result);
    }

    [TestMethod]
    public async Task GetPendingFriendRequestsAsync_ReturnsPendingRequests()
    {
        var userId = Guid.NewGuid();
        var expectedRequests = new List<PendingFriendRequest>
        {
            new() { RequestId = Guid.NewGuid(), FromUsername = "requester", CreatedAt = DateTime.Now }
        };
        _mockFriendsRepository.PendingRequestsToReturn = expectedRequests;

        var result = await _service.GetPendingFriendRequestsAsync(userId);

        CollectionAssert.AreEqual(expectedRequests, result);
    }

    [TestMethod]
    public async Task UpdateFriendRequestAsync_UpdatesStatus()
    {
        var dto = new UpdateFriendship { FriendshipId = Guid.NewGuid(), Status = Shared.Models.FriendshipStatus.Accepted };

        await _service.UpdateFriendRequestAsync(dto);

        Assert.AreEqual(dto, _mockFriendsRepository.LastUpdatedFriendship);
    }
}