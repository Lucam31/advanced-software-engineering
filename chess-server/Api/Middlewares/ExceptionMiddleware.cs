using System.Net;
using Shared.Exceptions;

namespace chess_server.Api.Middlewares;

public class ExceptionMiddleware
{
    public async Task HandleWithExceptionCatch(HttpListenerContext context, Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private async Task HandleException(HttpListenerContext context, Exception e)
    {
        var statusCode = e switch
        {
            UserNotFound => HttpStatusCode.NotFound,
            
            InvalidCredentials => HttpStatusCode.Unauthorized,
            
            UserAlreadyExists => HttpStatusCode.Conflict,
            
            _ => HttpStatusCode.InternalServerError
        };
        
        var response = new Response(context);
        response.SetStatusCode(statusCode);
        response.SetJson(new { error = e.Message});
        
        await response.Send();

        Console.WriteLine($"Exception handled: {e.Message}");
    }
}
