using System.Collections.Concurrent;
using System.Net;
using System.Threading.Channels;
using Shared.Exceptions;
using Shared.Logger;

namespace chess_server.Api.Hub;

/// <summary>
/// Defines the interface for a websocket hub that manages connected clients.
/// </summary>
public interface IWebSocketHub
{
    /// <summary>
    /// Processes messages coming from connected websocket clients and dispatches them as needed.
    /// </summary>
    /// <returns>A task that completes when processing stops.</returns>
    Task ProcessMessages();

    /// <summary>
    /// Accepts and handles a new WebSocket connection request from an HTTP listener context.
    /// </summary>
    /// <param name="context">The HTTP listener context containing the WebSocket upgrade request and querystring.</param>
    /// <returns>A task that completes when the connection has been handled.</returns>
    Task HandleConnectionRequest(HttpListenerContext context);
}

/// <summary>
/// Concrete implementation of <see cref="IWebSocketHub"/> that manages connected clients and a central input channel.
/// </summary>
public class WebSocketHub : IWebSocketHub
{
    private readonly ConcurrentDictionary<Guid, WebSocketClient> _clients = new();
    private readonly Channel<string> _hubInputChan = Channel.CreateUnbounded<string>();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="WebSocketHub"/> class and starts the background message-processing task.
    /// </summary>
    public WebSocketHub()
    {
        _ = Task.Run(ProcessMessages);
    }

    /// <summary>
    /// Handles an incoming WebSocket connection request. Validates the query parameters, accepts the WebSocket
    /// upgrade and registers a new <see cref="WebSocketClient"/> for the given user id.
    /// </summary>
    /// <param name="context">The <see cref="HttpListenerContext"/> representing the incoming request.</param>
    /// <returns>A task that completes after the connection has been accepted and the client registered.</returns>
    /// <exception cref="Shared.Exceptions.BadParameters">Thrown when the required query parameter is missing or invalid.</exception>
    public async Task HandleConnectionRequest(HttpListenerContext context)
    {
        GameLogger.Info("Parsing the userId.");
        
        var userIdStr = context.Request.QueryString["userId"];
        if (string.IsNullOrWhiteSpace(userIdStr))
            throw new BadParameters("NO_USERID");

        if (!Guid.TryParse(userIdStr, out var userId))
            throw new BadParameters("INVALID_USERID");
        
        var wsContext = await context.AcceptWebSocketAsync(subProtocol: null);
        var socket = wsContext.WebSocket;
        
        var client = new WebSocketClient(userId, socket,_hubInputChan);
        
        _clients.TryAdd(userId, client);
        
        GameLogger.Info($"New client connected with {userIdStr}");
    }


    /// <summary>
    /// Continuously processes messages from the hub input channel and dispatches them to clients or other handlers.
    /// </summary>
    /// <returns>A long-running task that processes messages until the application shuts down.</returns>
    public async Task ProcessMessages()
    {
        throw new NotImplementedException();
    }
}