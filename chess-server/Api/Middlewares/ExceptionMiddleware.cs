using System.Net;
using Shared.Exceptions;
using Shared.Logger;

namespace chess_server.Api.Middlewares;

/// <summary>
/// Middleware for handling exceptions during request processing.
/// </summary>
public class ExceptionMiddleware
{
    /// <summary>
    /// Wraps an action with a try-catch block to handle exceptions.
    /// </summary>
    /// <param name="context">The HTTP listener context.</param>
    /// <param name="action">The action to execute.</param>
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

    /// <summary>
    /// Handles an exception by sending an appropriate HTTP error response.
    /// </summary>
    /// <param name="context">The HTTP listener context.</param>
    /// <param name="e">The exception that occurred.</param>
    private async Task HandleException(HttpListenerContext context, Exception e)
    {
        var statusCode = e switch
        {
            UserNotFound => HttpStatusCode.NotFound,

            InvalidCredentials => HttpStatusCode.Unauthorized,

            UserAlreadyExists => HttpStatusCode.Conflict,
            
            BadParameters => HttpStatusCode.BadRequest,

            _ => HttpStatusCode.InternalServerError
        };

        var response = new Response(context);
        response.SetStatusCode(statusCode);
        response.SetJson(new { error = e.Message });

        await response.Send();

        GameLogger.Error($"Exception handled: {e.Message}", e);
    }
}