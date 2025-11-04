using System.Net.WebSockets;
using System.Threading.Channels;

namespace chess_server.Api.Hub;

/// <summary>
/// Defines the interface for a websocket client that represents the client on the server side.
/// </summary>
public interface IWebSocketClient
{
    
}

/// <summary>
/// Represents a connected WebSocket client with an identifier, the underlying WebSocket connection
/// and channels for sending and hub dispatch.
/// </summary>
public class WebSocketClient : IWebSocketClient
{
    /// <summary>
    /// The unique identifier of the connected user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The underlying <see cref="WebSocket"/> connection instance.
    /// </summary>
    public WebSocket Conn { get; set; }
    
    private readonly Channel<string> _sendChan = Channel.CreateUnbounded<string>();
    private readonly Channel<string> _hubDispatchChan;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebSocketClient"/> class.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    /// <param name="connection">The established <see cref="WebSocket"/> connection.</param>
    /// <param name="hubChan">The hub's input channel used to dispatch messages to the hub.</param>
    public WebSocketClient(Guid id, WebSocket connection, Channel<string> hubChan)
    {
        Id = id;
        Conn = connection;
        _hubDispatchChan = hubChan;   
    }
}