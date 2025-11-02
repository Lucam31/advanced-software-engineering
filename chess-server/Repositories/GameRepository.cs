using System.Data;
using chess_server.Data;
using chess_server.Models;

namespace chess_server.Repositories;

public interface IGameRepository
{
    Task InsertGameAsync(Game game);
    Task<List<Game>> GetRecentGamesAsync(int limit = 10);
    Task<List<Game>> GetGamesByUserIdAsync(Guid userId, int limit = 10);
}

public class GameRepository : IGameRepository
{
    private readonly IDatabase _database;

    public GameRepository(IDatabase database)
    {
        _database = database;
    }

    public async Task InsertGameAsync(Game game)
    {
        var sql = @"
            INSERT INTO games (id, white_player_id, black_player_id, moves)
            VALUES (@Id, @WhitePlayerId, @BlackPlayerId, @Moves)";

        var parameters = new Dictionary<string, object>
        {
            { "@WhitePlayerId", game.WhitePlayerId },
            { "@BlackPlayerId", game.BlackPlayerId },
            { "@Moves", game.Moves.ToArray() }
        };

        await _database.ExecuteNonQueryWithTransactionAsync(sql, parameters);
    }

    public async Task<List<Game>> GetRecentGamesAsync(int limit = 10)
    {
        var sql = @"
            SELECT id, white_player_id, black_player_id, moves
            FROM games
            ORDER BY created_at DESC
            LIMIT @Limit";

        var parameters = new Dictionary<string, object>
        {
            { "@Limit", limit }
        };

        var dataTable = await _database.ExecuteQueryAsync(sql, parameters);
        return ConvertDataTableToGames(dataTable);
    }

    public async Task<List<Game>> GetGamesByUserIdAsync(Guid userId, int limit = 10)
    {
        var sql = @"
            SELECT id, white_player_id, black_player_id, moves
            FROM games
            WHERE white_player_id = @UserId OR black_player_id = @UserId
            ORDER BY created_at DESC
            LIMIT @Limit";

        var parameters = new Dictionary<string, object>
        {
            { "@UserId", userId },
            { "@Limit", limit }
        };

        var dataTable = await _database.ExecuteQueryAsync(sql, parameters);
        return ConvertDataTableToGames(dataTable);
    }

    private List<Game> ConvertDataTableToGames(DataTable dataTable)
    {
        var games = new List<Game>();
        foreach (DataRow row in dataTable.Rows)
        {
            games.Add(new Game
            {
                Guid = (Guid)row["id"],
                WhitePlayerId = (Guid)row["white_player_id"],
                BlackPlayerId = (Guid)row["black_player_id"],
                Moves = ((string[])row["moves"]).ToList()
            });
        }
        return games;
    }
}