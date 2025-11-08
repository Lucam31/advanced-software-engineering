using System.Text.Json.Serialization;
using Shared.Dtos;

namespace Shared.WebSocketMessages;

/// <summary>
/// Represents the payload for starting a new game, including the game ID and the initial game board state.
/// </summary>
public class StartGamePayload
{
    /// <summary>
    /// The unique identifier for the game.
    /// </summary>
    [JsonPropertyName("gameId")]
    public Guid GameId { get; set; }
    /// <summary>
    /// The starting state of the game board.
    /// </summary>
    [JsonPropertyName("startingBoard")]
    public required GameboardDto StartingBoard { get; set; }
}