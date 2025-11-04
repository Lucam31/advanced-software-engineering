using Shared.InputDtos;
using Shared.ServerResponseObjects;

namespace chess_client.Services;

/// <summary>
/// Interface for authentication services.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Logs in a user with the given credentials.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <returns>The user ID.</returns>
    Task<Guid> Login(string username, string password);
    
    /// <summary>
    /// Registers a new user with the given credentials.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    Task Register(string username, string password);
}

/// <summary>
/// Provides authentication services for the client.
/// </summary>
public class AuthService : IAuthService
{
    private readonly HttpClient _client = new();
    private readonly JsonParser _jsonParser = new();
    
    /// <inheritdoc/>
    public async Task<Guid> Login(string username, string password)
    {
        
        var loginDto = new UserDto
        {
            Username = username,
            Password = password
        };
        
        var content = new StringContent(_jsonParser.SerializeJson(loginDto), System.Text.Encoding.UTF8, "application/json");

        var res = await _client.PostAsync("http://localhost:8080/api/user/login", content);
        
        if (!res.IsSuccessStatusCode)
        {
            throw new Exception("Registration failed.");
        }
        
        var responseContent = await res.Content.ReadAsStringAsync();
        
        var dto = _jsonParser.DeserializeJson<LoginResponseDto>(responseContent);
        
        if (dto == null)
            throw new Exception("Failed to parse login response.");
        
        return dto.UserId;
    }
    
    /// <inheritdoc/>
    public async Task Register(string username, string password)
    {
        var loginDto = new UserDto
        {
            Username = username,
            Password = password
        };
        
        var content = new StringContent(_jsonParser.SerializeJson(loginDto), System.Text.Encoding.UTF8, "application/json");

        var res = await _client.PostAsync("http://localhost:8080/api/user/register", content);
        
        if (!res.IsSuccessStatusCode)
        {
            throw new Exception("Registration failed.");
        }
    }
}