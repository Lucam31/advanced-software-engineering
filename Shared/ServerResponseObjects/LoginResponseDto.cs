using System.Text.Json.Serialization;

namespace Shared.ServerResponseObjects;

public class LoginResponseDto
{
    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }
}