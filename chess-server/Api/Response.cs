using System.Net;
using System.Text;
using System.Text.Json;

namespace chess_server.Api;

public class Response
{
    private readonly HttpListenerContext _context;
    private string? _content;
    private string _contentType;
    private HttpStatusCode _statusCode;
    
    public Response(HttpListenerContext context)
    {
        _context = context;
        _contentType = "text/plain";
        _statusCode = HttpStatusCode.OK; // Default Status
    }

    public Response(HttpListenerContext context, string content, HttpStatusCode statusCode )
    {
        _context = context;
        _contentType = "text/plain";
        _content = content;
        _statusCode = statusCode;
    }
    
    public Response(HttpListenerContext context, object content, HttpStatusCode statusCode )
    {
        _context = context;
        _content = JsonSerializer.Serialize(content);
        _contentType = "application/json";
        _statusCode = statusCode;
    }
    
    public void SetContent(string content, string contentType = "text/plain")
    {
        _content = content;
        _contentType = contentType;
    }

    public void SetJson(object obj)
    {
        _content = JsonSerializer.Serialize(obj);
        _contentType = "application/json";
    }

    public void SetStatusCode(HttpStatusCode statusCode)
    {
        _statusCode = statusCode;
    }
    
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
    
    public async Task Send(string content, string contentType, HttpStatusCode statusCode)
    {
        try
        {
            Console.WriteLine("Send method called");

            var buffer = Encoding.UTF8.GetBytes(content);

            _context.Response.ContentType = contentType;
            _context.Response.ContentLength64 = buffer.Length;
            _context.Response.StatusCode = (int)statusCode;

            await _context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Hello: {ex.Message}");
            _context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
        finally
        {
            _context.Response.Close();
        }
    }
    
    public Task Send(object obj, string contentType, HttpStatusCode statusCode)
    {
        string json = JsonSerializer.Serialize(obj);
        return Send(json, contentType, statusCode);
    }
}