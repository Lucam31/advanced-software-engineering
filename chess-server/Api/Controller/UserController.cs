using System.Net;
using chess_server.Services;
using Shared.InputDto;

namespace chess_server.API.Controller;

public interface IUserController
{
    Task Hello(HttpListenerContext context, Test input);
}

[Route("/api/user")]
public class UserController : IUserController
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [Route("/hello")]
    public async Task Hello(HttpListenerContext context, [FromBody] Test input)
    {
        Console.WriteLine($"Received request: {context.Request.Url}");
        var response = new Response(context);
        await response.Send("Hello " + input.Name + "!");
    }
}