using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using chess_server.API.Controller;

namespace chess_server.API;

public interface IRouter
{
    void RegisterController<T>() where T : class;
    Task HandleRequest(HttpListenerContext context);
}

public delegate Task RouteHandler(HttpListenerContext context);

public class Router : IRouter
{
    private readonly Dictionary<string, RouteHandler> _routes = new();
    private readonly IDiContainer _diContainer;
    
    public Router(IDiContainer diContainer)
    {
        _diContainer = diContainer;
    }

    public void RegisterController<T>() where T : class
    {
        var controller = typeof(T);
        
        Console.WriteLine($"Registering controller: {controller}");
    
        var controllerRoute = controller.GetCustomAttribute<RouteAttribute>();
        var basePath = controllerRoute?.Path ?? "";
    
        var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.GetCustomAttribute<RouteAttribute>() != null);

        foreach (var method in methods)
        {
            var routeAttribute = method.GetCustomAttribute<RouteAttribute>();
            var fullPath = basePath + routeAttribute.Path;

            RouteHandler handler = async (context) =>
            {
                var instance = _diContainer.Resolve<T>();
                var parameters = await GetMethodParameters(method, context);
                var result = method.Invoke(instance, parameters);
            
                if (result is Task task)
                    await task;
            };

            _routes.Add(fullPath, handler);
        }
    }

    private async Task<object[]> GetMethodParameters(MethodInfo method, HttpListenerContext context)
    {
        var parameters = method.GetParameters();
        var args = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var param = parameters[i];

            if (parameters[i].ParameterType == typeof(HttpListenerContext))
            {
                args[i] = context;
            }
            else if (param.GetCustomAttribute<FromBodyAttribute>() != null)
            {
                using var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8);
                var json = await reader.ReadToEndAsync();
                args[i] = JsonSerializer.Deserialize(json, param.ParameterType);
            }
        }

        return args;
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