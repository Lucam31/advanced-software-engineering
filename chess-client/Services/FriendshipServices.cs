using chess_server.OutputDtos;
using Shared;
using Shared.Dtos;

namespace chess_client.Services;

/// <summary>
/// Defines friendship features such as user search, friend requests, and friend list management.
/// </summary>
public interface IFriendshipServices
{
    /// <summary>
    /// Searches for users by username.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <returns>A list of user IDs matching the search criteria.</returns>
    Task<List<string>> SearchUsers(string username);

    /// <summary>
    /// Sends a friend request from one user to another.
    /// </summary>
    /// <param name="fromUserId">The ID of the user sending the friend request.</param>
    /// <param name="toUsername">The username of the user receiving the friend request.</param>
    /// <returns>A task that completes when the request has been sent.</returns>
    Task SendFriendRequest(Guid fromUserId, string toUsername);

    /// <summary>
    /// Lists all friends of a user.
    /// </summary>
    /// <param name="userId">The ID of the user whose friends are being listed.</param>
    /// <returns>A list of user IDs representing the user's friends.</returns>
    Task<List<Friend>> ListFriends(Guid userId);

    /// <summary>
    /// Removes a friend from the user's friend list.
    /// </summary>
    /// <param name="friendshipDto">The friendship entry to remove.</param>
    /// <returns>A task that completes when the friend has been removed.</returns>
    Task RemoveFriend(Friend friendshipDto);
}

/// <summary>
/// Calls friendship-related HTTP endpoints used by the client.
/// </summary>
public class FriendshipServices : IFriendshipServices
{
    private readonly HttpClient _client = new();
    private readonly JsonParser _jsonParser = new();

    /// <inheritdoc/>
    public async Task<List<string>> SearchUsers(string username)
    {
        var response = await _client.GetAsync($"http://localhost:8080/api/user/search?query={username}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var res = _jsonParser.DeserializeJson<Usernames>(content);
        return res == null ? [] : res.UsernamesList;
    }

    /// <inheritdoc/>
    public async Task SendFriendRequest(Guid fromUserId, string toUsername)
    {
        var dto = new FriendRequest
        {
            UserId = fromUserId,
            FriendUsername = toUsername
        };
        var content = new StringContent(_jsonParser.SerializeJson(dto), System.Text.Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("http://localhost:8080/api/friends/add", content);
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc/>
    public async Task<List<Friend>> ListFriends(Guid userId)
    {
        var response = await _client.GetAsync($"http://localhost:8080/api/friends/list?userId={userId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(content);
        var res = _jsonParser.DeserializeJson<List<Friend>>(content);

        return res ?? new List<Friend>();
    }

    /// <inheritdoc/>
    public async Task RemoveFriend(Friend friendDto)
    {
        var response =
            await _client.DeleteAsync(
                $"http://localhost:8080/api/friends/remove?friendshipId={friendDto.FriendshipId}");
        response.EnsureSuccessStatusCode();
    }
}