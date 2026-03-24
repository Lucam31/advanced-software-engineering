namespace chess_client;

using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using Shared.Logger;
using Shared.WebSocketMessages;
using States;

/// <summary>
/// Manages the client's WebSocket connection, including sending, receiving,
/// state delegation, and graceful shutdown.
/// </summary>
public class WebSocketService : IAsyncDisposable
{
    private ClientWebSocket _ws = new();
    private readonly SemaphoreSlim _sendSemaphore = new(1, 1);
    private IGameState? _currentState;
    private CancellationTokenSource _cts = new();
    private Task? _receiveLoopTask;
    private int _disconnecting;
    private int _disposed;
    private volatile bool _isConnected;

    /// <summary>
    /// Indicates whether the WebSocket is currently open.
    /// </summary>
    public bool IsConnected => _isConnected;

    /// <summary>
    /// Establishes the WebSocket connection and starts the receive loop.
    /// </summary>
    /// <param name="uri">Target URI of the WebSocket server.</param>
    /// <returns><c>true</c> if the connection was established successfully; otherwise, <c>false</c>.</returns>
    public async Task<bool> ConnectAsync(string uri)
    {
        ThrowIfDisposed();

        // If CTS was cancelled (from previous disconnect), create a new one
        if (_cts.IsCancellationRequested)
        {
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            
            // Also create a new WebSocket; the old one is in a bad state after disconnect
            _ws.Dispose();
            _ws = new ClientWebSocket();
        }

        try
        {
            await _ws.ConnectAsync(new Uri(uri), _cts.Token);
            _isConnected = true;
            GameLogger.Info($"WebSocket connected to {uri}");

            _receiveLoopTask = Task.Run(ReceiveLoopAsync);
            return true;
        }
        catch (Exception ex)
        {
            _isConnected = false;
            GameLogger.Error($"WebSocket connection failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Closes the connection in a controlled way and waits for the receive loop to finish.
    /// </summary>
    /// <param name="reason">Reason used when closing the connection.</param>
    public async Task DisconnectAsync(string reason = "Client disconnecting")
    {
        if (Interlocked.Exchange(ref _disconnecting, 1) == 1)
        {
            return;
        }

        try
        {
            await _cts.CancelAsync();

            if (_ws.State == WebSocketState.Open)
            {
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                await _ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, reason, timeoutCts.Token);
            }

            if (_receiveLoopTask is not null)
            {
                try
                {
                    await _receiveLoopTask;
                }
                catch (Exception e)
                {
                    GameLogger.Error($"Exception while awaiting closing: {e.Message}");
                    throw;
                }
            }
        }
        finally
        {
            _isConnected = false;
            Volatile.Write(ref _disconnecting, 0);
        }
    }

    /// <summary>
    /// Switches the active game state (for example, from MainMenu to Gameplay).
    /// </summary>
    /// <param name="newState">The new state that will handle incoming messages.</param>
    public void TransitionTo(IGameState newState)
    {
        ThrowIfDisposed();

        GameLogger.Debug($"State transition: {_currentState?.GetType().Name} → {newState.GetType().Name}");
        _currentState?.OnExit();
        _currentState = newState;
        _currentState.OnEnter();
    }

    /// <summary>
    /// Serializes and sends a message through the WebSocket.
    /// Send operations are executed strictly sequentially.
    /// </summary>
    /// <param name="message">Message to send.</param>
    public async Task SendAsync(WebSocketMessage message)
    {
        ThrowIfDisposed();

        if (!IsConnected)
        {
            throw new InvalidOperationException("WebSocket is not connected.");
        }

        string json;
        try
        {
            json = JsonSerializer.Serialize(message);
        }
        catch (Exception e)
        {
            GameLogger.Error($"SendAsync error: {e.Message}");
            throw;
        }

        var bytes = Encoding.UTF8.GetBytes(json);

        await _sendSemaphore.WaitAsync(_cts.Token);
        try
        {
            await _ws.SendAsync(bytes, WebSocketMessageType.Text, true, _cts.Token);
            GameLogger.Debug($"Sent message: {message.Type}");
        }
        catch (Exception e)
        {
            GameLogger.Error($"SendAsync error: {e.Message}");
            throw;
        }
        finally
        {
            _sendSemaphore.Release();
        }
    }

    /// <summary>
    /// Runs continuously in the background and receives messages from the server.
    /// </summary>
    private async Task ReceiveLoopAsync()
    {
        var buffer = new byte[4096];
        GameLogger.Debug("ReceiveLoop started.");

        try
        {
            while (!_cts.Token.IsCancellationRequested && _ws.State == WebSocketState.Open)
            {
                using var ms = new MemoryStream();
                WebSocketReceiveResult result;

                do
                {
                    result = await _ws.ReceiveAsync(buffer, _cts.Token);
                    ms.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                GameLogger.Debug($"Received: {ms.Length} bytes");

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    GameLogger.Warning("Server initiated close.");

                    if (_ws.State == WebSocketState.CloseReceived)
                    {
                        await _ws.CloseOutputAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Ack close",
                            CancellationToken.None);
                    }

                    break;
                }

                var json = Encoding.UTF8.GetString(ms.ToArray());
                GameLogger.Debug($"Raw message: {json}");

                var message = JsonSerializer.Deserialize<WebSocketMessage>(json);

                if (message is null)
                {
                    GameLogger.Warning($"Deserialized message was null. Raw: {json}");
                    continue;
                }

                if (message.Type == MessageType.Ping)
                {
                    var pong = new WebSocketMessage
                    {
                        Type = MessageType.Pong,
                        Payload = null,
                    };

                    await SendAsync(pong);
                    continue;
                }

                if (_currentState is null)
                {
                    GameLogger.Warning($"Received '{message.Type}' but no state is active — message dropped!");
                    continue;
                }

                GameLogger.Debug($"Received: {message.Type} — delegating to {_currentState.GetType().Name}");
                await _currentState.HandleMessageAsync(message);
            }
        }
        catch (OperationCanceledException)
        {
            GameLogger.Debug("ReceiveLoop cancelled.");
        }
        catch (Exception ex)
        {
            GameLogger.Error($"ReceiveLoop error: {ex.Message}");
        }
        finally
        {
            _isConnected = false;
        }
    }

    /// <summary>
    /// Asynchronously disposes the service and closes open resources in an idempotent way.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1)
        {
            return;
        }

        try
        {
            await DisconnectAsync("Client disposing");
        }
        catch (Exception ex)
        {
            // During disposal, only log errors: resources must still be released.
            GameLogger.Warning($"DisposeAsync disconnect warning: {ex.Message}");
        }
        finally
        {
            _sendSemaphore.Dispose();
            _cts.Dispose();
            _ws.Dispose();
        }
    }

    /// <summary>
    /// Throws an exception if the service has already been disposed.
    /// </summary>
    private void ThrowIfDisposed()
    {
        if (Volatile.Read(ref _disposed) == 1)
        {
            throw new ObjectDisposedException(nameof(WebSocketService));
        }
    }
}

