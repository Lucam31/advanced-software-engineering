using chess_server.Api.ActionResults;
using chess_server.Api.Attributes;
using chess_server.Services;
using Shared.InputDtos;

namespace chess_server.Api.Controller;

public interface IFriendsController
{
    Task<IActionResult> AddFriend(FriendRequest dto);
    Task<IActionResult> GetFriends(Guid userId);
    Task<IActionResult> GetPendingRequests(Guid userId);
    Task<IActionResult> UpdateFriendRequest(UpdateFriendship dto);
}

[Route("/api/friends")]
public class FriendsController : IFriendsController
{
    private readonly IFriendsService _friendsService;
    
    public FriendsController(IFriendsService friendsService)
    {
        _friendsService = friendsService;
    }
    
    [HttpMethod("POST")]
    [Route("/add")]
    public async Task<IActionResult> AddFriend([FromBody] FriendRequest dto)
    {
        await _friendsService.AddFriendAsync(dto);
        
        return Results.Ok();
    }

    [HttpMethod("GET")]
    [Route("/list")]
    public async Task<IActionResult> GetFriends([FromQuery] Guid userId)
    {
        var friends = await _friendsService.GetFriendsAsync(userId);
        
        return Results.Ok(friends);
    }

    [HttpMethod("GET")]
    [Route("/pendingRequests")]
    public async Task<IActionResult> GetPendingRequests([FromQuery] Guid userId)
    {
        return Results.Ok(await _friendsService.GetPendingFriendRequestsAsync(userId));
    }

    [HttpMethod("PATCH")]
    [Route("/update")]
    public async Task<IActionResult> UpdateFriendRequest([FromBody] UpdateFriendship dto)
    {
        await _friendsService.UpdateFriendRequestAsync(dto);
        
        return Results.Ok();
    }
}