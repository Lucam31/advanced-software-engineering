using Shared.ServerResponseObjects;
using Shared;
using Shared.Dtos;

namespace chess_client.Services;

/// <summary>
/// Defines authentication operations for client login and registration.
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
    /// Registers a new user with the provided credentials.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <returns>A task that completes when registration succeeds.</returns>
    Task Register(string username, string password);
}

/// <summary>
/// Sends authentication-related HTTP requests to the server API.
/// </summary>
public class AuthService(HttpClient client) : IAuthService
{
    private readonly JsonParser _jsonParser = new();

    /// <summary>
    /// Initializes a new instance of <see cref="AuthService"/> with a default <see cref="HttpClient"/>.
    /// </summary>
    public AuthService() : this(new HttpClient())
    {
    }

    /// <inheritdoc/>
    public async Task<Guid> Login(string username, string password)
    {
        var loginDto = new UserDto
        {
            Username = username,
            Password = password
        };

        var content = new StringContent(_jsonParser.SerializeJson(loginDto), System.Text.Encoding.UTF8,
            "application/json");

        var res = await client.PostAsync("http://localhost:8080/api/user/login", content);

        if (!res.IsSuccessStatusCode)
        {
            var errorContent = await res.Content.ReadAsStringAsync();
            var errorMessage = "Unknown error occurred.";

            try
            {
                var errorDto = _jsonParser.DeserializeJson<ErrorResponseDto>(errorContent);
                if (!string.IsNullOrEmpty(errorDto?.Error))
                {
                    errorMessage = TranslateServerErrorCode(errorDto.Error);
                }
            }
            catch
            {
                errorMessage = errorContent;
            }

            throw new Exception(errorMessage);
        }

        var responseContent = await res.Content.ReadAsStringAsync();
        var dto = _jsonParser.DeserializeJson<LoginResponseDto>(responseContent);

        return dto?.UserId ?? throw new Exception("Failed to parse login response.");
    }

    /// <inheritdoc/>
    public async Task Register(string username, string password)
    {
        var loginDto = new UserDto
        {
            Username = username,
            Password = password
        };

        var content = new StringContent(_jsonParser.SerializeJson(loginDto), System.Text.Encoding.UTF8,
            "application/json");

        var res = await client.PostAsync("http://localhost:8080/api/user/register", content);

        if (!res.IsSuccessStatusCode)
        {
            var errorContent = await res.Content.ReadAsStringAsync();
            var errorMessage = "Unknown error occurred.";

            try
            {
                var errorDto = _jsonParser.DeserializeJson<ErrorResponseDto>(errorContent);
                if (!string.IsNullOrEmpty(errorDto?.Error))
                {
                    errorMessage = TranslateServerErrorCode(errorDto.Error);
                }
            }
            catch
            {
                errorMessage = errorContent;
            }

            throw new Exception(errorMessage);
        }
    }

    /// <summary>
    /// Translates server error codes into user-friendly messages.
    /// </summary>
    /// <param name="errorCode">The error code returned by the server.</param>
    /// <returns>A human-readable error message for display in the UI.</returns>
    private string TranslateServerErrorCode(string errorCode)
    {
        return errorCode.ToUpper() switch
        {
            "USER_ALREADY_EXISTS" => "This username is already taken. Please choose another one.",
            "USER_NOT_FOUND" => "We couldn't find an account with that username.",
            "INVALID_CREDENTIALS" => "The username or password you entered is incorrect.",
            "PASSWORD_TOO_WEAK" => "Your password is too weak. It needs to be longer.",

            _ => $"An unexpected error occurred ({errorCode}). Please try again."
        };
    }
}