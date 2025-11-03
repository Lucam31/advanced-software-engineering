using System.Net;

namespace chess_server.Api.ActionResults;

/// <summary>
/// Represents an <see cref="IActionResult"/> that returns an HTTP 200 OK status code.
/// </summary>
public class OkResult : IActionResult
{
    private readonly object? _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="OkResult"/> class.
    /// </summary>
    /// <param name="value">The content to format in the entity body.</param>
    public OkResult(object? value = null)
    {
        _value = value;
    }

    /// <inheritdoc/>
    public async Task ExecuteResultAsync(HttpListenerContext context)
    {
        var response = new Response(context);
        response.SetStatusCode(HttpStatusCode.OK);

        if (_value != null)
            response.SetJson(_value);

        await response.Send();
    }
}