using System.Text.Json.Serialization;
using Shared.Dtos;

namespace Shared.WebSocketMessages;

/// <summary>
/// GameOverPayload represents the payload sent when a game is over, containing the game ID and the winner information.
/// </summary>
public class GameOverPayload
{
    /// <summary>
    /// The unique identifier of the game that has ended.
    /// </summary>
    [JsonPropertyName("gameId")]
    public Guid GameId { get; set; }
    /// <summary>
    /// The username of the player who won the game. This can be null if the game ended in a draw or if the winner is not determined.
    /// </summary>
    [JsonPropertyName("winner")]
    public string? Winner { get; set; } 
    /// <summary>
    /// The current state of the game board.
    /// </summary>
    [JsonPropertyName("currentBoard")]
    public required GameboardDto CurrentBoard { get; set; }
    /// <summary>
    /// The move that was just made.
    /// </summary>
    [JsonPropertyName("lastMove")]
    public required string LastMove { get; set; }
    
}
