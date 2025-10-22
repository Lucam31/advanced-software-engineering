using System.Net;

namespace chess_server.API;

public interface IRouter
{
    void Register(string path, RouteHandler handler);
    Task HandleRequest(HttpListenerContext context);
}

public delegate Task RouteHandler(HttpListenerContext context);

public class Router : IRouter
{
    private readonly Dictionary<string, RouteHandler> _routes = new();

    public void Register(string path, RouteHandler handler)
    {
        _routes.Add(path, handler);
    }
    
    public async Task HandleRequest(HttpListenerContext context)
    {
        var path = context.Request.Url?.AbsolutePath;
        if (path != null && _routes.TryGetValue(path, out var handler))
        {
            await handler(context);
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.Close();
        }
    }
}