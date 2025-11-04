using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using chess_server.Api.ActionResults;
using chess_server.Api.Attributes;
using chess_server.Api.Hub;
using chess_server.Api.Middlewares;
using Shared.Exceptions;
using Shared.Logger;

namespace chess_server.Api;

/// <summary>
/// Defines the interface for a router that handles HTTP requests.
/// </summary>
public interface IRouter
{
    /// <summary>
    /// Registers a controller class, making its methods available as routes.
    /// </summary>
    /// <typeparam name="T">The type of the controller to register.</typeparam>
    void RegisterController<T>() where T : class;

    /// <summary>
    /// Registers a WebSocket hub to manage incoming websocket connections.
    /// </summary>
    /// <param name="hub">The WebSocket hub to register.</param>
    void RegisterHub(WebSocketHub hub);
    
    /// <summary>
    /// Handles an incoming HTTP request by dispatching it to the appropriate route handler.
    /// </summary>
    /// <param name="context">The HTTP listener context for the request.</param>
    Task HandleRequest(HttpListenerContext context);
}

/// <summary>
/// Represents a delegate for handling a specific route.
/// </summary>
/// <param name="context">The HTTP listener context for the request.</param>
public delegate Task RouteHandler(HttpListenerContext context);

/// <summary>
/// A router that maps HTTP requests to controller methods.
/// </summary>
public class Router : IRouter
{
    private readonly Dictionary<string, RouteHandler> _routes = new();
    private readonly IDiContainer _diContainer;
    private readonly ExceptionMiddleware _exceptionMiddleware;

    /// <summary>
    /// Initializes a new instance of the <see cref="Router"/> class.
    /// </summary>
    /// <param name="diContainer">The dependency injection container.</param>
    /// <param name="exceptionMiddleware">The exception handling middleware.</param>
    public Router(IDiContainer diContainer, ExceptionMiddleware exceptionMiddleware)
    {
        _diContainer = diContainer;
        _exceptionMiddleware = exceptionMiddleware;
        GameLogger.Debug("Router initialized with ExceptionMiddleware.");
    }

    /// <inheritdoc/>
    public void RegisterController<T>() where T : class
    {
        var controller = typeof(T);

        GameLogger.Info($"Registering controller: {controller.Name}");

        var controllerRoute = controller.GetCustomAttribute<RouteAttribute>();
        var basePath = controllerRoute?.Path ?? "";

        var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.GetCustomAttribute<RouteAttribute>() != null).ToList();

        foreach (var method in methods)
        {
            var routeAttribute = method.GetCustomAttribute<RouteAttribute>();
            var fullPath = basePath + routeAttribute.Path;

            var httpMethodAttr = method.GetCustomAttribute<HttpMethodAttribute>();
            var httpMethod = httpMethodAttr?.Method ??
                             throw new Exception($"HTTP method not defined for {method.Name}");

            GameLogger.Debug($"Registering route {httpMethod}:{fullPath} -> {controller.Name}.{method.Name}");

            RouteHandler handler = async (context) =>
            {
                GameLogger.Debug($"Handling route {httpMethod}:{fullPath}");
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
                        if (actionResult != null)
                        {
                            await actionResult.ExecuteResultAsync(context);
                        }else
                        {
                            GameLogger.Warning($"Action {controller.Name}.{method.Name} returned null IActionResult.");
                        }
                    }
                }
            };

            var key = $"{httpMethod}:{fullPath}";
            _routes.Add(key, handler);
            GameLogger.Debug($"Route registered: {key}");
        }

        GameLogger.Info($"Controller {controller.Name} registration completed with {methods.Count()} route(s).");
    }
    
    /// <inheritdoc/>
    public void RegisterHub(WebSocketHub hub)
    {
        GameLogger.Info("Registering WebSocket Hub.");

        RouteHandler handler = async (context) =>
        {
            GameLogger.Debug("Handling WebSocket connection.");
            if (context.Request.IsWebSocketRequest)
            {
                await hub.HandleConnectionRequest(context);
            }
            else
            {
                GameLogger.Warning("Received non-WebSocket request on WebSocket route.");
                throw new BadParameters("NO_WEBSOCKET");
            }
        };

        var key = $"GET:/ws";
        _routes.Add(key, handler);
        GameLogger.Info("WebSocket Hub registered at route GET:/ws.");
    }

    /// <summary>
    /// Extracts and deserializes parameters for a controller method from the HTTP request.
    /// </summary>
    /// <param name="method">The controller method to extract parameters for.</param>
    /// <param name="context">The HTTP listener context.</param>
    /// <returns>An array of objects representing the method's arguments.</returns>
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
                GameLogger.Debug($"Bound HttpListenerContext to parameter '{param.Name}'.");
            }
            else if (param.ParameterType == typeof(Response))
            {
                args[i] = new Response(context);
                GameLogger.Debug($"Created Response for parameter '{param.Name}'.");
            }
            else if (param.GetCustomAttribute<FromBodyAttribute>() != null)
            {
                using var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8);
                var json = await reader.ReadToEndAsync();
                GameLogger.Debug(
                    $"Deserializing body for parameter '{param.Name}' to type {param.ParameterType.Name}.");
                try
                {
                    args[i] = JsonSerializer.Deserialize(json, param.ParameterType);
                }
                catch (Exception ex)
                {
                    GameLogger.Error($"Failed to deserialize body for parameter '{param.Name}': {ex.Message}", ex);
                    throw;
                }
            }
            else if (param.GetCustomAttribute<FromQueryAttribute>() != null)
            {
                var queryValue = context.Request.QueryString[param.Name];
                if (queryValue != null)
                {
                    args[i] = Convert.ChangeType(queryValue, param.ParameterType);
                    GameLogger.Debug($"Extracted query parameter '{param.Name}' with value '{queryValue}'.");
                }
                else
                {
                    args[i] = null;
                    GameLogger.Warning($"Missing expected query parameter '{param.Name}'.");
                }
            }
        }

        return args;
    }

    /// <inheritdoc/>
    public async Task HandleRequest(HttpListenerContext context)
    {
        await _exceptionMiddleware.HandleWithExceptionCatch(context, async () =>
        {
            var path = context.Request.Url?.AbsolutePath;
            var method = context.Request.HttpMethod?.ToUpperInvariant() ?? "GET";

            GameLogger.Info($"Dispatching request {method} {path}");

            if (path != null && _routes.TryGetValue($"{method}:{path}", out var handler))
            {
                await handler(context);
                GameLogger.Debug($"Successfully handled {method} {path}");
            }
            else
            {
                GameLogger.Warning($"Route not found: {method}:{path}");
                throw new KeyNotFoundException($"Route '{method}:{path}' nicht gefunden");
            }
        });
    }
}