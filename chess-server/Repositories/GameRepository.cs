using System.Data;
using chess_server.Data;
using chess_server.Models;

namespace chess_server.Repositories;

/// <summary>
/// Defines the interface for game data operations.
/// </summary>
public interface IGameRepository
{
    /// <summary>
    /// Inserts a new game into the database.
    /// </summary>
    /// <param name="game">The game to insert.</param>
    Task InsertGameAsync(Game game);

    /// <summary>
    /// Retrieves a list of the most recent games.
    /// </summary>
    /// <param name="limit">The maximum number of games to retrieve.</param>
    /// <returns>A list of <see cref="Game"/> objects.</returns>
    Task<List<Game>> GetRecentGamesAsync(int limit = 10);

    /// <summary>
    /// Retrieves a list of games played by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="limit">The maximum number of games to retrieve.</param>
    /// <returns>A list of <see cref="Game"/> objects.</returns>
    Task<List<Game>> GetGamesByUserIdAsync(Guid userId, int limit = 10);
}

/// <summary>
/// Provides methods for accessing game data in the database.
/// </summary>
public class GameRepository : IGameRepository
{
    private readonly IDatabase _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameRepository"/> class.
    /// </summary>
    /// <param name="database">The database instance.</param>
    public GameRepository(IDatabase database)
    {
        _database = database;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <summary>
    /// Converts a DataTable to a list of Game objects.
    /// </summary>
    /// <param name="dataTable">The DataTable to convert.</param>
    /// <returns>A list of <see cref="Game"/> objects.</returns>
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