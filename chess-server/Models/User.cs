namespace chess_server.Models;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class User
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public required string Username { get; init; }
    
    /// <summary>
    /// Gets or sets the hashed password.
    /// </summary>
    public required byte[] PasswordHash { get; init; }
    
    /// <summary>
    /// Gets or sets the salt used for password hashing.
    /// </summary>
    public required byte[] PasswordSalt { get; init; }
    
    /// <summary>
    /// Gets or sets the user's rating.
    /// </summary>
    public required int Rating { get; set; }
}