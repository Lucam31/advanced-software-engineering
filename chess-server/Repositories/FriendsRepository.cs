using System.Data;
using chess_server.Data;
using chess_server.Models;
using chess_server.OutputDtos;
using Shared.InputDtos;
using Shared.Models;

namespace chess_server.Repositories;

public interface IFriendsRepository
{
    Task AddFriendshipAsync(Guid fromUserId, Guid toUserId);
    Task<List<Friendship>> GetFriendsAsync(Guid userId);
    Task<List<Friendship>> GetPendingFriendRequestsAsync(Guid userId);
    Task UpdateFriendshipStatusAsync(UpdateFriendship dto);
}

public class FriendsRepository : IFriendsRepository
{
    private readonly IDatabase _database;

    public FriendsRepository(IDatabase database)
    {
        _database = database;
    }

    public async Task AddFriendshipAsync(Guid fromUserId, Guid toUserId)
    {
        var sql = @"
            INSERT INTO friendships (id, user_id_1, user_id_2, initiated_by, status, created_at)
            VALUES (@FriendshipId, @UserId1, @UserId2, @InitiatedBy, 'pending', NOW())";

        var parameters = new Dictionary<string, object>
        {
            { "@UserId1", fromUserId < toUserId ? fromUserId : toUserId },
            { "@UserId2", fromUserId < toUserId ? toUserId : fromUserId },
            { "@InitiatedBy", fromUserId }
        };

        await _database.ExecuteNonQueryWithTransactionAsync(sql, parameters);
    }

    public async Task<List<Friendship>> GetFriendsAsync(Guid userId)
    {
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
        return ConvertDataTableToFriendships(dataTable);
    }
    
    public async Task<List<Friendship>> GetPendingFriendRequestsAsync(Guid userId)
    {
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
        return ConvertDataTableToFriendships(dataTable);
    }

    public async Task UpdateFriendshipStatusAsync(UpdateFriendship dto)
    {
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
    }
    
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