using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Shared;
using Shared.Logger;
using Shared.WebSocketMessages;

namespace chess_server.Api.Hub;

public interface IWebSocketClient
{
    Guid Id { get; }
    bool IsActive { get; }
    DateTime LastPongUtc { get; }
    Task SendAsync(WebSocketMessage message);
    void MarkPongReceived();
    void MarkInactive();
    Task CloseAsync(string reason);
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
    /// Indicates whether the client is currently considered active by heartbeat checks.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Stores the UTC timestamp of the last received Pong.
    /// </summary>
    public DateTime LastPongUtc => new(Interlocked.Read(ref _lastPongTicks), DateTimeKind.Utc);

    /// <summary>
    /// The underlying <see cref="WebSocket"/> connection instance.
    /// </summary>
    private WebSocket Conn { get; set; }
    
    public event Func<Guid, Task>? ClientDisconnected;
    public event Func<string, JsonElement?,Guid, Task>? MessageReceived;

    
    private readonly Channel<WebSocketMessage> _sendChan = Channel.CreateUnbounded<WebSocketMessage>();
    private readonly JsonParser _jsonParser = new();
    private readonly CancellationTokenSource _lifecycleCts = new();
    private long _lastPongTicks = DateTime.UtcNow.Ticks;
    private int _closed;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebSocketClient"/> class.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    /// <param name="connection">The established <see cref="WebSocket"/> connection.</param>
    public WebSocketClient(Guid id, WebSocket connection)
    {
        Id = id;
        Conn = connection;
        _ = Task.Run(ProcessRead);
        _ = Task.Run(ProcessSend);
    }
    
    /// <summary>
    /// Sends a message to the client by writing it to the send channel.
    /// </summary>
    /// <param name="message">The <see cref="WebSocketMessage"/> to send.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    public async Task SendAsync(WebSocketMessage message)
    {
        await _sendChan.Writer.WriteAsync(message, _lifecycleCts.Token);
    }

    /// <summary>
    /// Updates heartbeat state after a Pong was received.
    /// </summary>
    public void MarkPongReceived()
    {
        Interlocked.Exchange(ref _lastPongTicks, DateTime.UtcNow.Ticks);
        IsActive = true;
    }

    /// <summary>
    /// Marks the client as inactive without forcefully closing the socket.
    /// </summary>
    public void MarkInactive()
    {
        IsActive = false;
    }

    /// <summary>
    /// Closes the client connection and completes background loops.
    /// </summary>
    /// <param name="reason">Close reason for the websocket close frame.</param>
    public async Task CloseAsync(string reason)
    {
        if (Interlocked.Exchange(ref _closed, 1) == 1)
        {
            return;
        }

        _sendChan.Writer.TryComplete();

        if (!_lifecycleCts.IsCancellationRequested)
        {
            await _lifecycleCts.CancelAsync();
        }

        try
        {
            if (Conn.State is WebSocketState.Open or WebSocketState.CloseReceived)
            {
                await Conn.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, reason, CancellationToken.None);
            }
        }
        catch (Exception ex) when (IsExpectedCloseException(ex))
        {
            GameLogger.Debug($"Ignored expected close race for client {Id}: {ex.Message}");
        }
        finally
        {
            if (Conn.State is not WebSocketState.Closed and not WebSocketState.None and not WebSocketState.Aborted)
            {
                try
                {
                    Conn.Abort();
                }
                catch (Exception ex) when (IsExpectedCloseException(ex))
                {
                    GameLogger.Debug($"Ignored expected abort race for client {Id}: {ex.Message}");
                }
            }
        }
    }
    
    private async Task ProcessSend()
    {
        try
        {
            await foreach (var msg in _sendChan.Reader.ReadAllAsync(_lifecycleCts.Token))
            {
                var messageBuffer = _jsonParser.SerializeToBytes(msg);
                var segment = new ArraySegment<byte>(messageBuffer);
                try
                {
                    await Conn.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch (Exception ex) when (IsExpectedCloseException(ex))
                {
                    GameLogger.Debug($"Stopped sending for closing client {Id}: {ex.Message}");
                    break;
                }
                catch (Exception ex)
                {
                    GameLogger.Error($"Failed to send message to client {Id}: {ex.Message}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown.
        }
    }
    
   /// <summary>
    /// Continuously reads incoming messages from the WebSocket connection, handles close messages,
    /// and dispatches valid messages to the service for further processing.
    /// </summary>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    private async Task ProcessRead()
    {
        var buffer = new byte[1024 * 4];
        var segment = new ArraySegment<byte>(buffer);

        while (Conn.State == WebSocketState.Open)
        {
            try
            {
                using var ms = new MemoryStream();
                WebSocketReceiveResult result;

                do
                {
                    result = await Conn.ReceiveAsync(segment, _lifecycleCts.Token);
                    ms.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    if (ClientDisconnected != null)
                        await ClientDisconnected.Invoke(Id);

                    await CloseAsync(string.Empty);
                    break;
                }

                var messageJson = Encoding.UTF8.GetString(ms.ToArray());
                var message = _jsonParser.DeserializeJson<WebSocketMessage>(messageJson);

                GameLogger.Debug("Received message from client " + Id + ": " + messageJson);
                if (message?.Type != null)
                {
                    if (MessageReceived != null)
                        await MessageReceived.Invoke(message.Type, message.Payload, Id);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                GameLogger.Error($"Failed to parse message: {ex.Message}");
            }
        }
    }

    private static bool IsExpectedCloseException(Exception ex)
    {
        if (ex is OperationCanceledException or ObjectDisposedException)
        {
            return true;
        }

        if (ex is WebSocketException wsEx)
        {
            return wsEx.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely
                   || wsEx.WebSocketErrorCode == WebSocketError.InvalidState;
        }

        return false;
    }

}