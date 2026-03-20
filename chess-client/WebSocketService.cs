namespace chess_client;

using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Shared.Logger;
using Shared.WebSocketMessages;
using States;

public class WebSocketService : IAsyncDisposable
{
    private readonly ClientWebSocket _ws = new();
    private IGameState? _currentState;
    private CancellationTokenSource _cts = new();

    public bool IsConnected => _ws.State == WebSocketState.Open;

    public async Task<bool> ConnectAsync(string uri)
    {
        try
        {
            await _ws.ConnectAsync(new Uri(uri), _cts.Token);
            GameLogger.Info($"WebSocket connected to {uri}");

            _ = Task.Run(ReceiveLoopAsync);
            return true;
        }
        catch (Exception ex)
        {
            GameLogger.Error($"WebSocket connection failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Wechselt den aktuellen Zustand (z.B. von MainMenu zu Gameplay).
    /// </summary>
    public void TransitionTo(IGameState newState)
    {
        GameLogger.Debug($"State transition: {_currentState?.GetType().Name} → {newState.GetType().Name}");
        _currentState?.OnExit();
        _currentState = newState;
        _currentState.OnEnter();
    }

    public async Task SendAsync(WebSocketMessage message)
    {
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

        try
        {
            await _ws.SendAsync(bytes, WebSocketMessageType.Text, true, _cts.Token);
        }
        catch (Exception e)
        {
            GameLogger.Error($"SendAsync error: {e.Message}");
            throw;
        }

        GameLogger.Debug($"Sent message: {message.Type}");
    }

    /// <summary>
    /// Läuft dauerhaft im Hintergrund und empfängt Nachrichten vom Server.
    /// </summary>
    private async Task ReceiveLoopAsync()
    {
        var buffer = new byte[4096];
        GameLogger.Debug("ReceiveLoop started.");

        try
        {
            while (!_cts.Token.IsCancellationRequested && _ws.State == WebSocketState.Open)
            {
                using var ms = new System.IO.MemoryStream();
                WebSocketReceiveResult result;

                do
                {
                    result = await _ws.ReceiveAsync(buffer, _cts.Token);
                    ms.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                GameLogger.Debug($"Received: {ms.Length} bytes");

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    GameLogger.Warning("Server closed connection.");
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
    }

    public async ValueTask DisposeAsync()
    {
        await _cts.CancelAsync();
        await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnecting", CancellationToken.None);
        _ws.Dispose();
    }
}