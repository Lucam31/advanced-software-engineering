using System.Text.Json.Serialization;
using Shared.Dtos;

namespace Shared.WebSocketMessages;

/// <summary>
/// Represents the payload sent during a game turn over WebSocket.
/// </summary>
public class GameTurnPayload
{
    /// <summary>
    /// The unique identifier of the game.
    /// </summary>
    [JsonPropertyName("gameId")]
    public Guid GameId { get; set; }
    /// <summary>
    /// The current state of the game board.
    /// </summary>
    [JsonPropertyName("currentBoard")]
    public required GameboardDto CurrentBoard { get; set; }
    /// <summary>
    /// The move that was just made.
    /// </summary>
    [JsonPropertyName("lastMove")]
    public required String LastMove { get; set; }
}