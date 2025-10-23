using System.Net;
using System.Text;
using System.Text.Json;

namespace chess_server.API;

public class Response
{
    private readonly HttpListenerContext _ctx;

    public Response(HttpListenerContext context)
    {
        _ctx = context;
    }
    
    public async Task Send(string content, string contentType = "text/plain")
    {
        try
        {
            Console.WriteLine("Send method called");

            var response = content;
            var buffer = Encoding.UTF8.GetBytes(response);

            _ctx.Response.ContentType = contentType;
            _ctx.Response.ContentLength64 = buffer.Length;
            _ctx.Response.StatusCode = 200;

            await _ctx.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Hello: {ex.Message}");
            _ctx.Response.StatusCode = 500;
        }
        finally
        {
            _ctx.Response.Close();
        }
    }
    
    public Task Send(object obj, string contentType = "application/json")
    {
        string json = JsonSerializer.Serialize(obj);
        return Send(json, contentType);
    }
}