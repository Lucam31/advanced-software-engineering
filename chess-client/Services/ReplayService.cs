using Shared;
using Shared.Dtos;
using Shared.Logger;

namespace chess_client.Services;

/// <summary>
/// Defines replay-related operations for retrieving previously played games.
/// </summary>
public interface IReplayService
{
    /// <summary>
    /// Fetches recently played games for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of played games.</returns>
    Task<List<PlayedGame>> GetGames(Guid userId);
}

/// <summary>
/// Fetches replay data from the server API.
/// </summary>
public class ReplayService : IReplayService
{
    private readonly HttpClient _client = new();
    private readonly JsonParser _jsonParser = new();

    /// <inheritdoc/>
    public async Task<List<PlayedGame>> GetGames(Guid userId)
    {
        var response = await _client.GetAsync($"http://localhost:8080/api/games/recent/user?userId={userId}");
        GameLogger.Info("Response: " + response);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var res = _jsonParser.DeserializeJson<List<PlayedGame>>(content);
        return res ?? [];
    }
}