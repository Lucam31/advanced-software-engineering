using System.Net;
using Shared.Logger;

namespace chess_server.Api.ActionResults;

/// <summary>
/// Represents an <see cref="IActionResult"/> that returns an HTTP 400 Bad Request status code.
/// </summary>
public class BadRequestResult : IActionResult
{
    private readonly string _errorMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestResult"/> class.
    /// </summary>
    /// <param name="errorMessage">The error message to return to the client.</param>
    public BadRequestResult(string errorMessage)
    {
        _errorMessage = errorMessage;
    }

    /// <inheritdoc/>
    public async Task ExecuteResultAsync(HttpListenerContext context)
    {
        GameLogger.Debug($"Executing BadRequestResult (ErrorMessage='{_errorMessage}')");
        
        var response = new Response(context);
        
        // Wir setzen den Statuscode auf 400 (Bad Request)
        response.SetStatusCode(HttpStatusCode.BadRequest);

        // Da dein Response-Objekt eine SetJson-Methode hat, nutzen wir diese, 
        // um den Fehler als einfaches JSON-Objekt an den Client zu schicken.
        response.SetJson(new { error = _errorMessage });

        await response.Send();
    }
}