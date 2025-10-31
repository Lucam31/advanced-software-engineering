using chess_server.Models;
using chess_server.Repositories;
using Shared.Exceptions;
using Shared.InputDtos;

namespace chess_server.Services;

public interface IUserService
{
    Task RegisterAsync(UserDto dto);
    Task<Guid> LoginAsync(UserDto dto);
    Task<List<string>> SearchUsersAsync(string query);
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
        var existingUser = await _repository.GetUserByUsernameAsync(dto.Username);
        if (existingUser != null)
        {
            throw new UserAlreadyExists();
        }
        
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
    }
    
    public async Task<Guid> LoginAsync(UserDto dto)
    {
        var user = await _repository.GetUserByUsernameAsync(dto.Username);
        if (user == null)
        {
            throw new UserNotFound();
        }

        if (!VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
        {
            throw new InvalidCredentials();
        }
        
        return user.Id;
    }
    
    public async Task<List<string>> SearchUsersAsync(string query)
    {
        var users = await _repository.SearchUsersByUsernameAsync(query);
        
        return users.Select(u => u.Username).ToList();
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