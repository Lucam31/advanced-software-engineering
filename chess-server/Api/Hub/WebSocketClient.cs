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
    
    public event Func<Guid, Task>? ClientDisconnected;
    public event Func<string, JsonElement,Guid, Task>? MessageReceived;

    
    private readonly Channel<WebSocketMessage> _sendChan = Channel.CreateUnbounded<WebSocketMessage>();
    private readonly JsonParser _jsonParser = new();

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
                   if (ClientDisconnected != null) 
                       await ClientDisconnected.Invoke(Id);

                   await Conn.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                   _sendChan.Writer.TryComplete();
                   break;
               }
               
               var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
               var message = _jsonParser.DeserializeJson<WebSocketMessage>(messageJson);
                
               if (message?.Type != null)
               {
                   if (MessageReceived != null)
                       await MessageReceived.Invoke(message.Type, message.Payload,Id);
               }
           }
           catch (Exception ex)
           {
               GameLogger.Error($"Failed to parse message: {ex.Message}");
           }
       }
   }
}