namespace Shared.Dtos;
using System.Text.Json.Serialization;

/// <summary>
/// DTO for handling server error responses.
/// </summary>
public class ErrorResponseDto
{
    [JsonPropertyName("error")]
    public string? Error { get; set; }
}