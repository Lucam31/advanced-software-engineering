using System.Net;

namespace chess_server.Api.ActionResults;

public interface IActionResult
{
    Task ExecuteResultAsync(HttpListenerContext context);
}