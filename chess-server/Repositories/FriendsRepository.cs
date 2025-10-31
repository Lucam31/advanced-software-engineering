using System.Data;
using chess_server.Data;
using chess_server.OutputDtos;
using Shared.InputDtos;

namespace chess_server.Repositories;

public interface IFriendsRepository
{
    Task<Guid> AddFriendshipAsync(Guid fromUserId, Guid toUserId);
    Task<List<Friend>> GetFriendsAsync(Guid userId);
    Task<List<PendingFriendRequest>> GetPendingFriendRequestsAsync(Guid userId);
    Task UpdateFriendshipStatusAsync(UpdateFriendship dto);
}

public class FriendsRepository : IFriendsRepository
{
    private readonly IDatabase _database;

    public FriendsRepository(IDatabase database)
    {
        _database = database;
    }

    public async Task<Guid> AddFriendshipAsync(Guid fromUserId, Guid toUserId)
    {
        var friendshipId = Guid.NewGuid();
        
        var sql = @"
            INSERT INTO friendships (id, user_id_1, user_id_2, initiated_by, status, created_at)
            VALUES (@FriendshipId, @UserId1, @UserId2, @InitiatedBy, 'pending', NOW())";

        var parameters = new Dictionary<string, object>
        {
            { "@FriendshipId", friendshipId },
            { "@UserId1", fromUserId < toUserId ? fromUserId : toUserId },
            { "@UserId2", fromUserId < toUserId ? toUserId : fromUserId },
            { "@InitiatedBy", fromUserId }
        };

        await _database.ExecuteNonQueryWithTransactionAsync(sql, parameters);
        return friendshipId;
    }

    public async Task<List<Friend>> GetFriendsAsync(Guid userId)
    {
        var sql = @"
            SELECT 
                u.username as user_username, 
                f.id as friendship_id
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
        return ConvertDataTableToFriends(dataTable);
    }

    public async Task<List<PendingFriendRequest>> GetPendingFriendRequestsAsync(Guid userId)
    {
        var sql = @"
            SELECT 
                f.id as friendship_id, 
                u.username as user_username, 
                f.created_at as friendship_created_at
            FROM friendships f
            JOIN users u ON u.id = f.initiated_by
            WHERE (f.user_id_1 = @UserId OR f.user_id_2 = @UserId)
              AND f.status = 'pending'
              AND f.initiated_by != @UserId";

        var parameters = new Dictionary<string, object>
        {
            { "@UserId", userId }
        };

        var dataTable = await _database.ExecuteQueryAsync(sql, parameters);
        return ConvertDataTableToPendingRequests(dataTable);
    }

    public async Task UpdateFriendshipStatusAsync(UpdateFriendship dto)
    {
        var sql = @"
            UPDATE friendships
            SET status = @Status
            WHERE id = @FriendshipId";

        var parameters = new Dictionary<string, object>
        {
            { "@Status", dto.Status },
            { "@FriendshipId", dto.FriendshipId }
        };

        await _database.ExecuteNonQueryWithTransactionAsync(sql, parameters);
    }
    
    private List<Friend> ConvertDataTableToFriends(DataTable dataTable)
    {
        var friends = new List<Friend>();
        foreach (DataRow row in dataTable.Rows)
        {
            friends.Add(new Friend
            {
                Name = row["user_username"].ToString() ?? "",
                FriendshipId = (Guid)row["friendship_id"]
            });
        }
        return friends;
    }

    private List<PendingFriendRequest> ConvertDataTableToPendingRequests(DataTable dataTable)
    {
        var requests = new List<PendingFriendRequest>();
        foreach (DataRow row in dataTable.Rows)
        {
            requests.Add(new PendingFriendRequest
            {
                RequestId = (Guid)row["friendship_id"],
                FromUsername = row["user_username"].ToString() ?? "",
                CreatedAt = (DateTime)row["friendship_created_at"]
            });
        }
        return requests;
    }
}