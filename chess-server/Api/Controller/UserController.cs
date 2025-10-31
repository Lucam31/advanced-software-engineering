using chess_server.Api.ActionResults;
using chess_server.Api.Attributes;
using chess_server.Services;
using Shared.InputDto;

namespace chess_server.Api.Controller;

public interface IUserController
{
    Task<IActionResult> Register(UserDto input);
    Task<IActionResult> Login(UserDto input);
}

[Route("/api/user")]
public class UserController : IUserController
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpMethod("POST")]
    [Route("/register")]
    public async Task<IActionResult> Register([FromBody] UserDto dto)
    {
        await _userService.RegisterAsync(dto);
        
        return Results.Ok();
    }
    
    [HttpMethod("POST")]
    [Route("/login")]
    public async Task<IActionResult> Login([FromBody] UserDto dto)
    {
        var userId = await _userService.LoginAsync(dto);
        
        return Results.Ok(new { userId });
    }

    [HttpMethod("GET")]
    [Route("/search")]
    public async Task<IActionResult> SearchUsers([FromQuery] string query)
    {
        var usernames = await _userService.SearchUsersAsync(query);
        
        return Results.Ok(new { usernames });
    }
}