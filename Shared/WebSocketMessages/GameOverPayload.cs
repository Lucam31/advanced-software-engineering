namespace Shared.WebSocketMessages;

/// <summary>
/// GameOverPayload represents the payload sent when a game is over, containing the game ID and the winner information.
/// </summary>
public class GameOverPayload
{
    /// <summary>
    /// The unique identifier of the game that has ended.
    /// </summary>
    public Guid GameId { get; set; }
    /// <summary>
    /// The username of the player who won the game. This can be null if the game ended in a draw or if the winner is not determined.
    /// </summary>
    public string? Winner { get; set; } 
}
