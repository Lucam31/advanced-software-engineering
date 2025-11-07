using System.Text.Json.Serialization;

namespace Shared.Dtos;

/// <summary>
/// Represents a data transfer object for user credentials.
/// </summary>
public class UserDto
{
    /// <summary>
    /// The username of the user.
    /// </summary>
    [JsonPropertyName("username")]
    public required string Username { get; set; }
    
    /// <summary>
    /// The password of the user.
    /// </summary>
    [JsonPropertyName("password")]
    public required string Password { get; set; }
}