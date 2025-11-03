using System.Data;
using Npgsql;
using Shared.Logger;

namespace chess_server.Data;

/// <summary>
/// Defines the interface for database operations.
/// </summary>
public interface IDatabase
{
    /// <summary>
    /// Executes a SQL query and returns the result as a DataTable.
    /// </summary>
    /// <param name="sql">The SQL query to execute.</param>
    /// <param name="parameters">A dictionary of parameters to use in the query.</param>
    /// <returns>A <see cref="DataTable"/> containing the query results.</returns>
    Task<DataTable> ExecuteQueryAsync(string sql, Dictionary<string, object>? parameters = null);
    
    /// <summary>
    /// Executes a non-query SQL command within a transaction.
    /// </summary>
    /// <param name="sql">The SQL command to execute.</param>
    /// <param name="parameters">A dictionary of parameters to use in the command.</param>
    /// <returns>The number of rows affected.</returns>
    Task<int> ExecuteNonQueryWithTransactionAsync(string sql, Dictionary<string, object>? parameters = null);
}

/// <summary>
/// Provides methods for interacting with a PostgreSQL database.
/// </summary>
public class Database : IDatabase
{
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="Database"/> class.
    /// </summary>
    /// <param name="connectionString">The connection string for the database.</param>
    public Database(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <inheritdoc/>
    public async Task<DataTable> ExecuteQueryAsync(string sql, Dictionary<string, object>? parameters = null)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await using var command = new NpgsqlCommand(sql, connection);

        if (parameters != null)
        {
            foreach (var p in parameters)
                command.Parameters.AddWithValue(p.Key, p.Value);
        }

        await connection.OpenAsync();
        GameLogger.Debug($"Executing query: {sql}");
        await using var reader = await command.ExecuteReaderAsync();
        var table = new DataTable();
        table.Load(reader);
        return table;
    }

    /// <inheritdoc/>
    public async Task<int> ExecuteNonQueryWithTransactionAsync(string sql, Dictionary<string, object>? parameters = null)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();
        await using var command = new NpgsqlCommand(sql, connection, transaction);

        if (parameters != null)
        {
            foreach (var p in parameters)
                command.Parameters.AddWithValue(p.Key, p.Value);
        }

        try
        {
            GameLogger.Debug($"Executing non-query: {sql}");
            var affected = await command.ExecuteNonQueryAsync();
            await transaction.CommitAsync();
            GameLogger.Debug($"{affected} rows affected.");
            return affected;
        }
        catch(Exception ex)
        {
            GameLogger.Error("Transaction failed, rolling back.", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }
}