using System.Data;
using Microsoft.Data.SqlClient;

namespace chess_server.Data;

public interface IDatabase
{
    Task<DataTable> ExecuteQueryAsync(string sql, Dictionary<string, object>? parameters = null);
    Task<int> ExecuteNonQueryWithTransactionAsync(string sql, Dictionary<string, object>? parameters = null);
}

public class Database
{
    private readonly string _connectionString;
    
    public Database(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task<DataTable> ExecuteQueryAsync(string sql, Dictionary<string, object>? parameters = null)
    { 
        var connection = new SqlConnection(_connectionString);
        var command = new SqlCommand(sql, connection);

        if (parameters != null)
        {
            foreach (var p in parameters)
                command.Parameters.AddWithValue(p.Key, p.Value);
        }

        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();
        var table = new DataTable();
        table.Load(reader);
        return table;
    }
    
    public async Task<int> ExecuteNonQueryWithTransactionAsync(string sql, Dictionary<string, object>? parameters = null)
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var transaction = connection.BeginTransaction();
        
        var command = new SqlCommand(sql, connection, transaction);

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