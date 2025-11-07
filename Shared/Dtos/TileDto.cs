using System.Text.Json.Serialization;

namespace Shared.Dtos;

/// <summary>
/// Represents a data transfer object for a tile on a chessboard.
/// </summary>
public class TileDto
{
    /// <summary>
    /// The row index of the tile on the chessboard.
    /// </summary>
    [JsonPropertyName("row")]
    public int Row { get; set; }
    /// <summary>
    /// The column index of the tile on the chessboard.
    /// </summary>
    [JsonPropertyName("col")]
    public int Col { get; set; }
    /// <summary>
    /// True if the tile is white; otherwise, false.
    /// </summary>
    [JsonPropertyName("isWhite")]
    public bool IsWhite { get; set; }
    /// <summary>
    /// The current piece on the tile, if any.
    /// </summary>
    [JsonPropertyName("currentPiece")]
    public PieceDto? CurrentPiece { get; set; }
}