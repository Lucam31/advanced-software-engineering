namespace Shared;

/// <summary>
/// Holds statistical and display information for the current game.
///
/// Contains per-side move lists, player display names and a short status message
/// used by the UI to render the game HUD.
/// </summary>
public class GameStats
{
    /// <summary>
    /// Display name for the player controlling the white pieces.
    /// </summary>
    public string WhitePlayerName { get; set; } = "Player 1 (White)";

    /// <summary>
    /// Display name for the player controlling the black pieces.
    /// </summary>
    public string BlackPlayerName { get; set; } = "Player 2 (Black)";

    /// <summary>
    /// Short status message shown on the UI (for example: whose turn it is).
    /// </summary>
    public string StatusMessage { get; set; } = "Game starting...";

    /// <summary>
    /// Moves made by the White player in chronological order (e.g. "e2e4").
    /// </summary>
    public List<string> WhiteMoves { get; set; } = [];

    /// <summary>
    /// Moves made by the Black player in chronological order (e.g. "e7e5").
    /// </summary>
    public List<string> BlackMoves { get; set; } = [];

    /// <summary>
    /// Adds a formatted move to the appropriate per-side move list.
    /// </summary>
    /// <param name="from">Origin square (for example: "e2").</param>
    /// <param name="to">Destination square (for example: "e4").</param>
    /// <param name="isWhite">True when the move was performed by White; otherwise false.</param>
    public void AddMoveToHistory(string from, string to, bool isWhite)
    {
        var move = $"{from}{to}";

        if (isWhite)
            WhiteMoves.Add(move);
        else
            BlackMoves.Add(move);
    }
}