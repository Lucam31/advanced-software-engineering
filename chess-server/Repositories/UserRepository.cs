using System.Data;
using chess_server.Data;
using chess_server.Models;
using Shared.Logger;

namespace chess_server.Repositories;

/// <summary>
/// Defines the interface for user data operations.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Inserts a new user into the database.
    /// </summary>
    /// <param name="user">The user to insert.</param>
    Task InsertUserAsync(User user);

    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <returns>The <see cref="User"/> if found; otherwise, null.</returns>
    Task<User?> GetUserByUsernameAsync(string username);

    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The <see cref="User"/> if found; otherwise, null.</returns>
    Task<User?> GetUserByIdAsync(Guid id);

    /// <summary>
    /// Searches for users by a username query.
    /// </summary>
    /// <param name="username">The username query to search for.</param>
    /// <returns>A list of matching users.</returns>
    Task<List<User>> SearchUsersByUsernameAsync(string username);
}

/// <summary>
/// Provides methods for accessing user data in the database.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly IDatabase _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="db">The database instance.</param>
    public UserRepository(IDatabase db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task InsertUserAsync(User user)
    {
        GameLogger.Debug($"Attempting to insert user: {user.Username}");
        var parameters = new Dictionary<string, object>
        {
            {"@Id", user.Id},
            {"@Username", user.Username},
            {"@PasswordHash", user.PasswordHash},
            {"@PasswordSalt", user.PasswordSalt},
            {"@Rating", user.Rating }
        };

        var sql = @"INSERT INTO Users (id, username, password_hash, password_salt, rating) 
                    VALUES (@Id, @Username, @PasswordHash, @PasswordSalt, @Rating)";
        
        await _db.ExecuteNonQueryWithTransactionAsync(sql, parameters);
        GameLogger.Info($"Successfully inserted user: {user.Username}");
    } 
    
    /// <inheritdoc/>
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        GameLogger.Debug($"Attempting to get user by username: {username}");
        var parameters = new Dictionary<string, object>
        {
            { "@Username", username }
        };

        var sql = @"SELECT * 
                    FROM Users 
                    WHERE username = @Username";
        
        var reader = await _db.ExecuteQueryAsync(sql, parameters);
        
        if (reader.Rows.Count > 0)
        {
            GameLogger.Debug($"Found user: {username}");
            var row = reader.Rows[0];
            return new User
            {
                Id = (Guid)row["id"],
                Username = (string)row["username"],
                PasswordHash = (byte[])row["password_hash"],
                PasswordSalt = (byte[])row["password_salt"],
                Rating = (int)row["rating"]
            };
        }

        GameLogger.Warning($"User not found: {username}");
        return null;
    }
    
    /// <inheritdoc/>
    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        GameLogger.Debug($"Attempting to get user by ID: {id}");
        var parameters = new Dictionary<string, object>
        {
            { "@Id", id }
        };

        var sql = @"SELECT * 
                    FROM Users 
                    WHERE id = @Id";
        
        var reader = await _db.ExecuteQueryAsync(sql, parameters);
        
        if (reader.Rows.Count > 0)
        {
            GameLogger.Debug($"Found user with ID: {id}");
            var row = reader.Rows[0];
            return new User
            {
                Id = (Guid)row["id"],
                Username = (string)row["username"],
                PasswordHash = (byte[])row["password_hash"],
                PasswordSalt = (byte[])row["password_salt"],
                Rating = (int)row["rating"]
            };
        }

        GameLogger.Warning($"User not found with ID: {id}");
        return null;
    }
    
    /// <inheritdoc/>
    public async Task<List<User>> SearchUsersByUsernameAsync(string username)
    {
        GameLogger.Debug($"Searching for users with query: {username}");
        var parameters = new Dictionary<string, object>
        {
            { "@Username", "%" + username + "%" }
        };

        var sql = @"SELECT * 
                    FROM Users 
                    WHERE username ILIKE @Username";
        
        var reader = await _db.ExecuteQueryAsync(sql, parameters);
        
        var users = new List<User>();

        foreach (DataRow row in reader.Rows)
        {
            users.Add(new User
            {
                Id = (Guid)row["id"],
                Username = (string)row["username"],
                PasswordHash = (byte[])row["password_hash"],
                PasswordSalt = (byte[])row["password_salt"],
                Rating = (int)row["rating"]
            });
        }

        GameLogger.Debug($"Found {users.Count} users matching query: {username}");
        return users;
    }
}