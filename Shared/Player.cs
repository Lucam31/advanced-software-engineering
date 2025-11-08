namespace Shared;

/// <summary>
/// Represents a player in an active chess game.
/// </summary>
public class Player(Guid id, string name, bool isWhite, int rating)
{
    /// <summary>
    /// The unique identifier of the player.
    /// </summary>
    public Guid Id { get; } = id;
    /// <summary>
    /// The name of the player.
    /// </summary>
    public string Name { get; init; } = name ?? throw new ArgumentNullException(nameof(name));
    /// <summary>
    /// True if the player is playing as white; otherwise, false.
    /// </summary>
    public bool IsWhite { get; init; } = isWhite;
    /// <summary>
    /// The rating of the player.
    /// </summary>
    public int Rating { get; init; } = rating;
}