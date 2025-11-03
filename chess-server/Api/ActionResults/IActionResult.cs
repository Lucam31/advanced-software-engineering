using System.Net;

namespace chess_server.Api.ActionResults;

/// <summary>
/// Defines a contract for a result of an action method.
/// </summary>
public interface IActionResult
{
    /// <summary>
    /// Executes the result operation of the action method asynchronously.
    /// </summary>
    /// <param name="context">The context in which the result is executed.</param>
    Task ExecuteResultAsync(HttpListenerContext context);
}