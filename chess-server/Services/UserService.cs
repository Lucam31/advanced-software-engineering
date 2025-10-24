using chess_server.Repositories;
using Shared.InputDto;

namespace chess_server.Services;

public interface IUserService
{
    public Task RegisterAsync(UserDto dto);
}

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task RegisterAsync(UserDto dto)
    {
        CreatePasswordHash(dto.Password, out var hash, out var salt);
        
        var user = new Models.User
        {
            Id = Guid.NewGuid(),
            Username = dto.Username,
            PasswordHash = hash,
            PasswordSalt = salt,
            Rating = 1200
        };
        
        await _repository.InsertUserAsync(user);
    }
    
    private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        salt = hmac.Key;
        hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }

    private bool VerifyPasswordHash(string password, byte[] hash, byte[] salt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512(salt);
        var generatedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return generatedHash.SequenceEqual(hash);
    }
}