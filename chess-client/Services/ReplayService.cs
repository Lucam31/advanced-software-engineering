using Shared.ServerResponseObjects;
using Shared;
using Shared.Dtos;
using Shared.Logger;

namespace chess_client.Services;

/// <summary>
/// Interface for game services
/// </summary>
public interface IReplayService
{
    /// <summary>
    /// Fetches the games of a user by their ID
    /// </summary>
    /// <param name="userId">The userId</param>
    /// <returns>A List of played games</returns>
    Task<List<PlayedGame>> GetGames(Guid userId);
    
}

/// <summary>
/// Provides authentication services for the client.
/// </summary>
public class ReplayService : IReplayService
{
    private readonly HttpClient _client = new();
    private readonly JsonParser _jsonParser = new();

    public async Task<List<PlayedGame>> GetGames(Guid userId)
    {
        var response = await _client.GetAsync($"http://localhost:8080/api/games/recent/user?userId={userId}");
        GameLogger.Info("Response: " + response);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
                
        var res = _jsonParser.DeserializeJson<List<PlayedGame>>(content);
        if (res == null)
            return new List<PlayedGame>();
                        
        return res;
    }
    
}