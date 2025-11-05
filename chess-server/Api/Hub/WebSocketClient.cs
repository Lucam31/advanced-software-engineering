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
    Task SendAsync(WebSocketMessage message);
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
    
    private readonly Channel<WebSocketMessage> _sendChan = Channel.CreateUnbounded<WebSocketMessage>();
    private readonly WebSocketHub _hub;
    private readonly JsonParser _jsonParser = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="WebSocketClient"/> class.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    /// <param name="connection">The established <see cref="WebSocket"/> connection.</param>
    /// <param name="hub">The hub instance for managing the client.</param>
    public WebSocketClient(Guid id, WebSocket connection, WebSocketHub hub)
    {
        Id = id;
        Conn = connection;
        _hub = hub;

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
        await _sendChan.Writer.WriteAsync(message);
    }
    
    private async Task ProcessSend()
    {
        await foreach (var msg in _sendChan.Reader.ReadAllAsync())
        {
            var messageBuffer = _jsonParser.SerializeToBytes(msg);
            var segment = new ArraySegment<byte>(messageBuffer);
            try
            {
                await Conn.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                GameLogger.Error($"Failed to send message to client {Id}: {ex.Message}");
            }
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
               var result = await Conn.ReceiveAsync(segment, CancellationToken.None);
               if (result.MessageType == WebSocketMessageType.Close)
               {
                   await _hub.UnregisterClient(Id);
                   
                   await Conn.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                   _sendChan.Writer.TryComplete();
                   break;
               }
               
               var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
               var message = _jsonParser.DeserializeJson<WebSocketMessage>(messageJson);
               if (message == null || string.IsNullOrEmpty(message.Type))
               {
                   GameLogger.Warning("Invalid message: missing type");
                   continue;
               }
               await DispatchToService(message.Type, message.Payload);
           }
           catch (Exception ex)
           {
               GameLogger.Error($"Failed to parse message: {ex.Message}");
           }
       }
   }
    
    /// <summary>
    /// Dispatches the received message to the appropriate service based on the message type.
    /// This is a placeholder for implementing specific dispatch logic.
    /// </summary>
    /// <param name="messageType">The type of the message to dispatch.</param>
    /// <param name="payload">The payload of the message as a JSON element.</param>
    /// <returns>A task that represents the asynchronous dispatch operation.</returns>
    private async Task DispatchToService(string messageType, JsonElement payload)
    {
        // Placeholder for dispatch logic based on message type
        GameLogger.Info($"Dispatching message of type {messageType} with payload: {payload}");
        await Task.CompletedTask;
    }
    
}