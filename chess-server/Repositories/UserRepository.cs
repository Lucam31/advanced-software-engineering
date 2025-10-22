using chess_server.Data;
using chess_server.Models;

namespace chess_server.Repositories;

public interface IUserRepository
{
    public Task InsertUserAsync(User user);
}

public class UserRepository : IUserRepository
{
    private readonly Database _db;
    
    public UserRepository(Database db)
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
        
        var sql = "INSERT INTO Users (id, username, password_hash, password_salt, rating) " +
                  "VALUES (@Id, @Username, @PasswordHash, @PasswordSalt, @Rating)";
        
        var aff = await _db.ExecuteNonQueryWithTransactionAsync(sql, parameters);
        
        Console.WriteLine("Affected: " + aff);
    } 
    
}