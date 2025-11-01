using System.Data;
using Npgsql;

namespace chess_server.Data;

public interface IDatabase
{
    Task<DataTable> ExecuteQueryAsync(string sql, Dictionary<string, object>? parameters = null);
    Task<int> ExecuteNonQueryWithTransactionAsync(string sql, Dictionary<string, object>? parameters = null);
}

public class Database : IDatabase
{
    private readonly string _connectionString;

    public Database(string connectionString)
    {
        _connectionString = connectionString;
    }

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

        await using var reader = await command.ExecuteReaderAsync();
        var table = new DataTable();
        table.Load(reader);
        return table;
    }

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
            var affected = await command.ExecuteNonQueryAsync();
            await transaction.CommitAsync();
            return affected;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}