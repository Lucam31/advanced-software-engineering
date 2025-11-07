using System.Text.Json.Serialization;

namespace Shared.Dtos;

/// <summary>
/// Data Transfer Object (DTO) representing the state of a chess gameboard.
/// </summary>
public class GameboardDto
{
    /// <summary>
    /// Two-dimensional array representing the tiles on the chessboard.
    /// </summary>
    [JsonPropertyName("tiles")]
    public TileDto[,] Tiles { get; set; } = new TileDto[8, 8];
    /// <summary>
    /// List of captured white pieces.
    /// </summary>
    [JsonPropertyName("capturedWhitePieces")]
    public List<PieceDto> CapturedWhitePieces { get; set; } = [];
    /// <summary>
    /// List of captured black pieces.
    /// </summary>
    [JsonPropertyName("capturedBlackPieces")]
    public List<PieceDto> CapturedBlackPieces { get; set; } = [];
}