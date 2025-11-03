namespace Shared.InputDtos;

/// <summary>
/// Represents a data transfer object for user credentials.
/// </summary>
public class UserDto
{
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public required string Username { get; set; }
    
    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public required string Password { get; set; }
}