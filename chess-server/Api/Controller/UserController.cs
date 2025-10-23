using System.Net;
using System.Text;
using chess_server.Services;
using Shared.InputDto;

namespace chess_server.API.Controller;

[Route("/api/user")]
public class UserController
{
    private readonly IUserService _userService;
    
    public UserController(UserService userService)
    {
        _userService = userService;
    }
    
    [Route("/hello")]
    public async Task Hello(HttpListenerContext context, [FromBody] Test input)
    {
        Console.WriteLine($"Received request: {context.Request.Url}");
        var response = new Response(context);
        await response.Send("Hello" + input.Name + "!");
    }
}