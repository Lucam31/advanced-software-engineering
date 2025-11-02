using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using chess_server.Api.ActionResults;
using chess_server.Api.Attributes;
using chess_server.Api.Middlewares;

namespace chess_server.Api;

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
    private readonly ExceptionMiddleware _exceptionMiddleware;

    public Router(IDiContainer diContainer)
    {
        _diContainer = diContainer;
        _exceptionMiddleware = new ExceptionMiddleware();
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

            var httpMethodAttr = method.GetCustomAttribute<HttpMethodAttribute>();
            var httpMethod = httpMethodAttr?.Method ?? throw new Exception($"HTTP method not defined for {method.Name}");

            RouteHandler handler = async (context) =>
            {
                var instance = _diContainer.Resolve<T>();
                var parameters = await GetMethodParameters(method, context);
                var result = method.Invoke(instance, parameters);

                if (result is Task task)
                {
                    await task;

                    if (task.GetType().IsGenericType &&
                        task.GetType().GetGenericArguments()[0].IsAssignableTo(typeof(IActionResult)))
                    {
                        var actionResult = (IActionResult)task.GetType().GetProperty("Result")?.GetValue(task);
                        await actionResult?.ExecuteResultAsync(context);
                    }
                }
            };
            
            var key = $"{httpMethod}:{fullPath}";
            _routes.Add(key, handler);
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
            else if (param.ParameterType == typeof(Response))
            {
                args[i] = new Response(context);
            }
            else if (param.GetCustomAttribute<FromBodyAttribute>() != null)
            {
                using var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8);
                var json = await reader.ReadToEndAsync();
                args[i] = JsonSerializer.Deserialize(json, param.ParameterType);
            }
            else if (param.GetCustomAttribute<FromQueryAttribute>() != null)
            {
                var queryValue = context.Request.QueryString[param.Name];
                if (queryValue != null)
                {
                    args[i] = Convert.ChangeType(queryValue, param.ParameterType);
                }
                else
                {
                    args[i] = null; 
                }
            }
        }

        return args;
    }

    public async Task HandleRequest(HttpListenerContext context)
    {
        await _exceptionMiddleware.HandleWithExceptionCatch(context, async () =>
        {
            var path = context.Request.Url?.AbsolutePath;
            var method = context.Request.HttpMethod?.ToUpperInvariant() ?? "GET";

            if (path != null && _routes.TryGetValue($"{method}:{path}", out var handler))
            {
                await handler(context);
            }
            else
            {
                throw new KeyNotFoundException($"Route '{method}:{path}' nicht gefunden");
            }
        });
    }
}