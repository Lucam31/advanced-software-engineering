using chess_server.Api.ActionResults;
using chess_server.Api.Attributes;
using chess_server.Services;
using Shared.InputDtos;

namespace chess_server.Api.Controller;

/// <summary>
/// Defines the interface for the user controller.
/// </summary>
public interface IUserController
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="input">The user registration data.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
    Task<IActionResult> Register(UserDto input);
    
    /// <summary>
    /// Logs in a user.
    /// </summary>
    /// <param name="input">The user login data.</param>
    /// <returns>An <see cref="IActionResult"/> containing the user ID on success.</returns>
    Task<IActionResult> Login(UserDto input);
    
    /// <summary>
    /// Searches for users by username.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <returns>An <see cref="IActionResult"/> containing a list of matching usernames.</returns>
    Task<IActionResult> SearchUsers(string query);

}

/// <summary>
/// API controller for user-related actions.
/// </summary>
[Route("/api/user")]
public class UserController : IUserController
{
    private readonly IUserService _userService;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="userService">The user service.</param>
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    /// <inheritdoc/>
    [HttpMethod("POST")]
    [Route("/register")]
    public async Task<IActionResult> Register([FromBody] UserDto dto)
    {
        await _userService.RegisterAsync(dto);
        
        return Results.Ok();
    }
    
    /// <inheritdoc/>
    [HttpMethod("POST")]
    [Route("/login")]
    public async Task<IActionResult> Login([FromBody] UserDto dto)
    {
        var userId = await _userService.LoginAsync(dto);
        
        return Results.Ok(new { userId });
    }

    /// <inheritdoc/>
    [HttpMethod("GET")]
    [Route("/search")]
    public async Task<IActionResult> SearchUsers([FromQuery] string query)
    {
        var usernames = await _userService.SearchUsersAsync(query);
        
        return Results.Ok(new { usernames });
    }
}