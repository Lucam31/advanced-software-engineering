using System.Data;
using chess_server.Data;
using chess_server.Models;

namespace chess_server.Repositories;

public interface IUserRepository
{
    Task InsertUserAsync(User user);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<List<User>> SearchUsersByUsernameAsync(string username);
}

public class UserRepository : IUserRepository
{
    private readonly IDatabase _db;
    
    public UserRepository(IDatabase db)
    {
        _db = db;
    }

    public async Task InsertUserAsync(User user)
    {
        var parameters = new Dictionary<string, object>
        {
            {"@Id", user.Id},
            {"@Username", user.Username},
            {"@PasswordHash", user.PasswordHash},
            {"@PasswordSalt", user.PasswordSalt},
            {"@Rating", user.Rating}
        };
        
        var sql = @"INSERT INTO Users (id, username, password_hash, password_salt, rating) 
                    VALUES (@Id, @Username, @PasswordHash, @PasswordSalt, @Rating)";
        
        var aff = await _db.ExecuteNonQueryWithTransactionAsync(sql, parameters);
        
        Console.WriteLine("Affected: " + aff);
    } 
    
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        var parameters = new Dictionary<string, object>
        {
            {"@Username", username}
        };
        
        var sql = @"SELECT * 
                    FROM Users 
                    WHERE username = @Username";
        
        var reader = await _db.ExecuteQueryAsync(sql, parameters);
        
        if (reader.Rows.Count > 0)
        {
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

        return null;
    }
    
    public async Task<List<User>> SearchUsersByUsernameAsync(string username)
    {
        var parameters = new Dictionary<string, object>
        {
            {"@Username", "%" + username + "%"}
        };
        
        var sql = @"SELECT * 
                    FROM Users 
                    WHERE username = @Username";
        
        var reader = await _db.ExecuteQueryAsync(sql, parameters);
        
        if (reader.Rows.Count > 0)
        {
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

            return users;
        }

        return new List<User>();
    }
}