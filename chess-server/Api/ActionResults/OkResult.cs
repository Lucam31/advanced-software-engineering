using System.Net;

namespace chess_server.Api.ActionResults;

public class OkResult : IActionResult
{
    private readonly object? _value;

    public OkResult(object? value = null)
    {
        _value = value;
    }

    public async Task ExecuteResultAsync(HttpListenerContext context)
    {
        var response = new Response(context);
        response.SetStatusCode(HttpStatusCode.OK);
        
        if (_value != null)
            response.SetJson(_value);
            
        await response.Send();
    }
}