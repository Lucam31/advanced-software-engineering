using chess_server.Models;
using chess_server.Repositories;
using Shared.Dtos;
using Shared.Exceptions;
using Shared.Logger;

namespace chess_server.Services;

/// <summary>
/// Defines the interface for user-related business logic.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="dto">The user data transfer object.</param>
    Task RegisterAsync(UserDto dto);
    
    /// <summary>
    /// Logs in a user and returns their ID.
    /// </summary>
    /// <param name="dto">The user data transfer object.</param>
    /// <returns>The user's unique identifier.</returns>
    Task<Guid> LoginAsync(UserDto dto);
    
    /// <summary>
    /// Searches for users by a username query.
    /// </summary>
    /// <param name="query">The username query.</param>
    /// <returns>A list of matching usernames.</returns>
    Task<List<string>> SearchUsersAsync(string query);
}

/// <summary>
/// Implements the business logic for user-related operations.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    /// <param name="repository">The user repository.</param>
    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc/>
    public async Task RegisterAsync(UserDto dto)
    {
        GameLogger.Info($"Registration attempt for user: {dto.Username}");
        var existingUser = await _repository.GetUserByUsernameAsync(dto.Username);
        if (existingUser != null)
        {
            GameLogger.Warning($"Registration failed: User '{dto.Username}' already exists.");
            throw new UserAlreadyExists();
        }
        
        GameLogger.Debug($"User '{dto.Username}' does not exist. Proceeding with registration.");
        CreatePasswordHash(dto.Password, out var hash, out var salt);
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = dto.Username,
            PasswordHash = hash,
            PasswordSalt = salt,
            Rating = 1200
        };
        
        await _repository.InsertUserAsync(user);
        GameLogger.Info($"User '{dto.Username}' registered successfully.");
    }
    
    /// <inheritdoc/>
    public async Task<Guid> LoginAsync(UserDto dto)
    {
        GameLogger.Info($"Login attempt for user: {dto.Username}");
        var user = await _repository.GetUserByUsernameAsync(dto.Username);
        if (user == null)
        {
            GameLogger.Warning($"Login failed: User '{dto.Username}' not found.");
            throw new UserNotFound();
        }

        if (!VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
        {
            GameLogger.Warning($"Login failed: Invalid password for user '{dto.Username}'.");
            throw new InvalidCredentials();
        }
        
        GameLogger.Info($"User '{dto.Username}' logged in successfully.");
        return user.Id;
    }
    
    /// <inheritdoc/>
    public async Task<List<string>> SearchUsersAsync(string query)
    {
        GameLogger.Info($"Searching for users with query: '{query}'");
        var users = await _repository.SearchUsersByUsernameAsync(query);
        var usernames = users.Select(u => u.Username).ToList();
        GameLogger.Info($"Found {usernames.Count} users for query: '{query}'");
        return usernames;
    }
    
    /// <summary>
    /// Creates a password hash and salt from a given password.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <param name="hash">The generated password hash.</param>
    /// <param name="salt">The generated password salt.</param>
    private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
    {
        GameLogger.Debug("Creating password hash and salt.");
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        salt = hmac.Key;
        hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }

    /// <summary>
    /// Verifies that a password matches a given hash and salt.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="hash">The stored password hash.</param>
    /// <param name="salt">The stored password salt.</param>
    /// <returns>True if the password is valid; otherwise, false.</returns>
    private bool VerifyPasswordHash(string password, byte[] hash, byte[] salt)
    {
        GameLogger.Debug("Verifying password hash.");
        using var hmac = new System.Security.Cryptography.HMACSHA512(salt);
        var generatedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return generatedHash.SequenceEqual(hash);
    }
}