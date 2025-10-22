using System.Net;
using System.Text;
using chess_server.Services;

namespace chess_server.API.Controller;

public class UserController
{
    private readonly IUserService _userService;
    
    public UserController(UserService userService)
    {
        _userService = userService;
    }
    
    public async Task Hello(HttpListenerContext context)
    {
        try
        {
            Console.WriteLine("Hello endpoint called");
        
            var response = "Hello World!";
            var buffer = Encoding.UTF8.GetBytes(response);

            context.Response.ContentType = "text/plain";
            context.Response.ContentLength64 = buffer.Length;
            context.Response.StatusCode = 200;

            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.Close();
        
            Console.WriteLine("Response sent successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Hello: {ex.Message}");
            context.Response.StatusCode = 500;
            context.Response.Close();
        }
    }
}