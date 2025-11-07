using System.Data;
using chess_server.Data;
using chess_server.Models;
using chess_server.OutputDtos;
using Shared.Dtos;
using Shared.Logger;
using Shared.Models;

namespace chess_server.Repositories;

/// <summary>
/// Defines the interface for friendship data operations.
/// </summary>
public interface IFriendsRepository
{
    /// <summary>
    /// Adds a new friendship request to the database.
    /// </summary>
    /// <param name="fromUserId">The ID of the user initiating the request.</param>
    /// <param name="toUserId">The ID of the user receiving the request.</param>
    Task AddFriendshipAsync(Guid fromUserId, Guid toUserId);

    /// <summary>
    /// Retrieves a list of accepted friendships for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of <see cref="Friendship"/> objects.</returns>
    Task<List<Friendship>> GetFriendsAsync(Guid userId);

    /// <summary>
    /// Retrieves a list of pending friend requests for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of <see cref="Friendship"/> objects with a 'pending' status.</returns>
    Task<List<Friendship>> GetPendingFriendRequestsAsync(Guid userId);

    /// <summary>
    /// Updates the status of a friendship.
    /// </summary>
    /// <param name="dto">The data transfer object containing the friendship ID and new status.</param>
    Task UpdateFriendshipStatusAsync(UpdateFriendship dto);
}

/// <summary>
/// Provides methods for accessing friendship data in the database.
/// </summary>
public class FriendsRepository : IFriendsRepository
{
    private readonly IDatabase _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="FriendsRepository"/> class.
    /// </summary>
    /// <param name="database">The database instance.</param>
    public FriendsRepository(IDatabase database)
    {
        _database = database;
    }

    /// <inheritdoc/>
    public async Task AddFriendshipAsync(Guid fromUserId, Guid toUserId)
    {
        GameLogger.Debug($"Adding friendship between {fromUserId} and {toUserId}");
        var sql = @"
            INSERT INTO friendships (id, user_id_1, user_id_2, initiated_by, status, created_at)
            VALUES (gen_random_uuid(), @UserId1, @UserId2, @InitiatedBy, 'pending', NOW())";

        var parameters = new Dictionary<string, object>
        {
            { "@UserId1", fromUserId < toUserId ? fromUserId : toUserId },
            { "@UserId2", fromUserId < toUserId ? toUserId : fromUserId },
            { "@InitiatedBy", fromUserId }
        };

        await _database.ExecuteNonQueryWithTransactionAsync(sql, parameters);
        GameLogger.Info($"Friendship added between {fromUserId} and {toUserId}");
    }

    /// <inheritdoc/>
    public async Task<List<Friendship>> GetFriendsAsync(Guid userId)
    {
        GameLogger.Debug($"Getting friends for user {userId}");
        var sql = @"
            SELECT 
                f.id as friendship_id,
                f.user_id_1 as user_id_1,
                f.user_id_2 as user_id_2,
                f.status as status,
                f.initiated_by as initiated_by,
                f.created_at as created_at
            FROM users u
            JOIN friendships f ON (
                (f.user_id_1 = u.id AND f.user_id_2 = @UserId) OR
                (f.user_id_2 = u.id AND f.user_id_1 = @UserId)
            )
            WHERE f.status = 'accepted' AND u.id != @UserId";

        var parameters = new Dictionary<string, object>
        {
            { "@UserId", userId }
        };

        var dataTable = await _database.ExecuteQueryAsync(sql, parameters);
        var friendships = ConvertDataTableToFriendships(dataTable);
        GameLogger.Debug($"Found {friendships.Count} friends for user {userId}");
        return friendships;
    }
    
    /// <inheritdoc/>
    public async Task<List<Friendship>> GetPendingFriendRequestsAsync(Guid userId)
    {
        GameLogger.Debug($"Getting pending friend requests for user {userId}");
        var sql = @"
            SELECT 
                f.id as friendship_id,
                f.user_id_1 as user_id_1,
                f.user_id_2 as user_id_2,
                f.status as status,
                f.initiated_by as initiated_by,
                f.created_at as created_at
            FROM users u
            JOIN friendships f ON (
                (f.user_id_1 = u.id AND f.user_id_2 = @UserId) OR
                (f.user_id_2 = u.id AND f.user_id_1 = @UserId)
            )
            WHERE f.status = 'pending' AND f.initiated_by != @UserId";

        var parameters = new Dictionary<string, object>
        {
            { "@UserId", userId }
        };

        var dataTable = await _database.ExecuteQueryAsync(sql, parameters);
        var friendships = ConvertDataTableToFriendships(dataTable);
        GameLogger.Debug($"Found {friendships.Count} pending requests for user {userId}");
        return friendships;
    }

    /// <inheritdoc/>
    public async Task UpdateFriendshipStatusAsync(UpdateFriendship dto)
    {
        GameLogger.Debug($"Updating friendship {dto.FriendshipId} to status {dto.Status}");
        var sql = @"
            UPDATE friendships
            SET status = @Status
            WHERE id = @FriendshipId";

        var parameters = new Dictionary<string, object>
        {
            { "@Status", dto.Status.ToString().ToLower() },
            { "@FriendshipId", dto.FriendshipId }
        };

        await _database.ExecuteNonQueryWithTransactionAsync(sql, parameters);
        GameLogger.Info($"Friendship {dto.FriendshipId} status updated to {dto.Status}");
    }
    
    /// <summary>
    /// Converts a DataTable to a list of Friendship objects.
    /// </summary>
    /// <param name="dataTable">The DataTable to convert.</param>
    /// <returns>A list of <see cref="Friendship"/> objects.</returns>
    private List<Friendship> ConvertDataTableToFriendships(DataTable dataTable)
    {
        var friendships = new List<Friendship>();
        foreach (DataRow row in dataTable.Rows)
        {
            friendships.Add(new Friendship
            {
                Id = (Guid)row["friendship_id"],
                UserId1 = (Guid)row["user_id_1"],
                UserId2 = (Guid)row["user_id_2"],
                Status = row["status"].ToString() ?? "",
                InitiatedBy = (Guid)row["initiated_by"],
                CreatedAt = (DateTime)row["created_at"]
            });
        }

        return friendships;
    }
}