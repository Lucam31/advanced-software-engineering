using System.Text.Json.Serialization;

namespace Shared.Dtos;

/// <summary>
/// Represents a data transfer object for a chess piece.
/// </summary>
public class PieceDto
{
    /// <summary>
    /// The position of the piece on the chessboard in algebraic notation (e.g., "e4").
    /// </summary>
    [JsonPropertyName("position")]
    public string Position { get; set; } = string.Empty;
    /// <summary>
    /// Indicates whether the piece is white.
    /// </summary>
    [JsonPropertyName("isWhite")]
    public bool IsWhite { get; set; }
    /// <summary>
    /// The type of the piece (e.g., "Pawn", "Rook", "Knight", "Bishop", "Queen", "King").
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    /// <summary>
    /// The Unicode symbol representing the piece.
    /// </summary>
    [JsonPropertyName("unicodeSymbol")]
    public string UnicodeSymbol { get; set; } = string.Empty;
    /// <summary>
    /// True if the piece has been captured; otherwise, false.
    /// </summary>
    [JsonPropertyName("isCaptured")]
    public bool IsCaptured { get; set; }
    /// <summary>
    /// True if the piece has moved from its starting position; otherwise, false.
    /// </summary>
    [JsonPropertyName("moved")]
    public bool Moved { get; set; }
}