using System.Data;
using chess_server.Repositories;
using chess_server.Models;
using chess_server.Data;

namespace chess_server_tests.UnitTests;

[TestClass]
public sealed class UserRepositoryTests
{
    private class MockDatabase : IDatabase
    {
        public DataTable QueryResult { get; set; } = new DataTable();
        public int NonQueryResult { get; set; } = 1;
        public bool ExecuteQueryCalled { get; private set; }
        public bool ExecuteNonQueryCalled { get; private set; }
        public string? LastSql { get; private set; }
        public Dictionary<string, object>? LastParameters { get; private set; }

        public Task<DataTable> ExecuteQueryAsync(string sql, Dictionary<string, object>? parameters = null)
        {
            ExecuteQueryCalled = true;
            LastSql = sql;
            LastParameters = parameters;
            return Task.FromResult(QueryResult);
        }

        public Task<int> ExecuteNonQueryWithTransactionAsync(string sql, Dictionary<string, object>? parameters = null)
        {
            ExecuteNonQueryCalled = true;
            LastSql = sql;
            LastParameters = parameters;
            return Task.FromResult(NonQueryResult);
        }

        public void AssertExecuteNonQueryCalledWith(string expectedSql, Dictionary<string, object> expectedParams)
        {
            Assert.IsTrue(ExecuteNonQueryCalled);
            Assert.AreEqual(expectedSql, LastSql);
            Assert.AreEqual(expectedParams.Count, LastParameters?.Count ?? 0);
            foreach (var kvp in expectedParams)
            {
                Assert.AreEqual(kvp.Value, LastParameters?[kvp.Key]);
            }
        }

        public void AssertExecuteQueryCalledWith(string expectedSql, Dictionary<string, object> expectedParams)
        {
            Assert.IsTrue(ExecuteQueryCalled);
            Assert.AreEqual(expectedSql, LastSql);
            Assert.AreEqual(expectedParams.Count, LastParameters?.Count ?? 0);
            foreach (var kvp in expectedParams)
            {
                Assert.AreEqual(kvp.Value, LastParameters?[kvp.Key]);
            }
        }
    }

    [TestMethod]
    public async Task InsertUserAsync_CallsExecuteNonQueryWithCorrectParameters()
    {
        // Arrange
        var mockDb = new MockDatabase();
        var repo = new UserRepository(mockDb);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = new byte[] { 1, 2, 3 },
            PasswordSalt = new byte[] { 4, 5, 6 },
            Rating = 1200
        };

        // Act
        await repo.InsertUserAsync(user);

        // Assert
        mockDb.AssertExecuteNonQueryCalledWith(
            "INSERT INTO Users (id, username, password_hash, password_salt, rating) VALUES (@Id, @Username, @PasswordHash, @PasswordSalt, @Rating)",
            new Dictionary<string, object>
            {
                { "@Id", user.Id },
                { "@Username", user.Username },
                { "@PasswordHash", user.PasswordHash },
                { "@PasswordSalt", user.PasswordSalt },
                { "@Rating", user.Rating }
            });
    }

    [TestMethod]
    public async Task GetUserByUsernameAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var mockDb = new MockDatabase();
        var table = new DataTable();
        table.Columns.Add("id", typeof(Guid));
        table.Columns.Add("username", typeof(string));
        table.Columns.Add("password_hash", typeof(byte[]));
        table.Columns.Add("password_salt", typeof(byte[]));
        table.Columns.Add("rating", typeof(int));
        var row = table.NewRow();
        var userId = Guid.NewGuid();
        row["id"] = userId;
        row["username"] = "testuser";
        row["password_hash"] = new byte[] { 1, 2, 3 };
        row["password_salt"] = new byte[] { 4, 5, 6 };
        row["rating"] = 1200;
        table.Rows.Add(row);
        mockDb.QueryResult = table;

        var repo = new UserRepository(mockDb);

        // Act
        var result = await repo.GetUserByUsernameAsync("testuser");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(userId, result.Id);
        Assert.AreEqual("testuser", result.Username);
        Assert.AreEqual(1200, result.Rating);
        mockDb.AssertExecuteQueryCalledWith("SELECT * FROM Users WHERE username = @Username", new Dictionary<string, object> { { "@Username", "testuser" } });
    }

    [TestMethod]
    public async Task GetUserByUsernameAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        var mockDb = new MockDatabase();
        mockDb.QueryResult = new DataTable(); // Empty table
        var repo = new UserRepository(mockDb);

        // Act
        var result = await repo.GetUserByUsernameAsync("nonexistent");

        // Assert
        Assert.IsNull(result);
        Assert.IsTrue(mockDb.ExecuteQueryCalled);
    }
}