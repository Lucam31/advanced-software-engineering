namespace chess_server.OutputDtos;

/// <summary>
/// Represents a data transfer object for a played game.
/// </summary>
public class PlayedGame
{
    /// <summary>
    /// Gets or sets the unique identifier for the game.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the username of the player who played as white.
    /// </summary>
    public string WhitePlayerUsername { get; set; } = "";
    
    /// <summary>
    /// Gets or sets the username of the player who played as black.
    /// </summary>
    public string BlackPlayerUsername { get; set; } = "";
    
    /// <summary>
    /// Gets or sets the list of moves made during the game.
    /// </summary>
    public List<string> Moves { get; set; } = new();
}