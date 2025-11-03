using System.Net;
using System.Text;
using System.Text.Json;
using Shared.Logger;

namespace chess_server.Api;

/// <summary>
/// Represents an HTTP response to be sent to a client.
/// </summary>
public class Response
{
    private readonly HttpListenerContext _context;
    private string? _content;
    private string _contentType;
    private HttpStatusCode _statusCode;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Response"/> class with a default status code.
    /// </summary>
    /// <param name="context">The HTTP listener context.</param>
    public Response(HttpListenerContext context)
    {
        _context = context;
        _contentType = "text/plain";
        _statusCode = HttpStatusCode.OK; // Default Status
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Response"/> class with specified content and status code.
    /// </summary>
    /// <param name="context">The HTTP listener context.</param>
    /// <param name="content">The string content to send.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public Response(HttpListenerContext context, string content, HttpStatusCode statusCode )
    {
        _context = context;
        _contentType = "text/plain";
        _content = content;
        _statusCode = statusCode;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Response"/> class with a JSON object and status code.
    /// </summary>
    /// <param name="context">The HTTP listener context.</param>
    /// <param name="content">The object to serialize as JSON.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public Response(HttpListenerContext context, object content, HttpStatusCode statusCode )
    {
        _context = context;
        _content = JsonSerializer.Serialize(content);
        _contentType = "application/json";
        _statusCode = statusCode;
    }
    
    /// <summary>
    /// Sets the content of the response.
    /// </summary>
    /// <param name="content">The string content.</param>
    /// <param name="contentType">The content type (defaults to "text/plain").</param>
    public void SetContent(string content, string contentType = "text/plain")
    {
        _content = content;
        _contentType = contentType;
    }

    /// <summary>
    /// Sets the response content to a JSON serialized object.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    public void SetJson(object obj)
    {
        _content = JsonSerializer.Serialize(obj);
        _contentType = "application/json";
    }

    /// <summary>
    /// Sets the HTTP status code for the response.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    public void SetStatusCode(HttpStatusCode statusCode)
    {
        _statusCode = statusCode;
    }
    
    /// <summary>
    /// Sends the configured response to the client.
    /// </summary>
    public async Task Send()
    {
        if (_content != null)
        {
            await Send(_content, _contentType, _statusCode);
        }
        else
        {
            _context.Response.StatusCode = (int)_statusCode;
            _context.Response.Close();
        }
    }
    
    /// <summary>
    /// Sends a response with the specified content, content type, and status code.
    /// </summary>
    /// <param name="content">The string content to send.</param>
    /// <param name="contentType">The content type.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public async Task Send(string content, string contentType, HttpStatusCode statusCode)
    {
        try
        {
            GameLogger.Debug($"Sending response with status {statusCode} and content type {contentType}.");

            var buffer = Encoding.UTF8.GetBytes(content);

            _context.Response.ContentType = contentType;
            _context.Response.ContentLength64 = buffer.Length;
            _context.Response.StatusCode = (int)statusCode;

            await _context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }
        catch (Exception ex)
        {
            GameLogger.Error($"Error sending response: {ex.Message}", ex);
            _context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
        finally
        {
            _context.Response.Close();
        }
    }
    
    /// <summary>
    /// Sends a JSON response with the specified object, content type, and status code.
    /// </summary>
    /// <param name="obj">The object to serialize and send.</param>
    /// <param name="contentType">The content type (should typically be "application/json").</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public Task Send(object obj, string contentType, HttpStatusCode statusCode)
    {
        string json = JsonSerializer.Serialize(obj);
        return Send(json, contentType, statusCode);
    }
}