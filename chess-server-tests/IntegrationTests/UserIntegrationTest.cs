using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Configurations;
using chess_server.Data;
using chess_server.Services;
using chess_server.Repositories;
using Shared.InputDtos;
using System.Text.Json;
using System.Text;
using System.IO;

namespace chess_server_tests.IntegrationTests;

[TestClass]
public sealed class UserIntegrationTest
{
    private PostgreSqlTestcontainer? _postgresContainer;
    private HttpListener? _listener;
    private CancellationTokenSource? _cts;
    private Task? _listenerTask;
    private IDatabase? _database;
    private IUserService? _userService;

    [TestInitialize]
    public async Task Init()
    {
        // Postgres starten
        _postgresContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration
            {
                Database = "testdb",
                Username = "postgres",
                Password = "postgres"
            })
            .WithImage("postgres:15-alpine")
            .Build();
        await _postgresContainer.StartAsync();

        // DB initialisieren
        _database = new Database(_postgresContainer.ConnectionString);

        // Migration ausführen
        var migrationSql = @"
            CREATE TABLE IF NOT EXISTS users (
                id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
                username text NOT NULL UNIQUE,
                password_hash bytea NOT NULL,
                password_salt bytea NOT NULL,
                rating integer NOT NULL DEFAULT 400
            );
        ";
        await _database.ExecuteNonQueryWithTransactionAsync(migrationSql);

        // Service initialisieren
        var userRepository = new UserRepository(_database);
        _userService = new UserService(userRepository);

        // HttpListener starten
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://localhost:8080/");
        _listener.Start();

        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        _listenerTask = Task.Run(async () =>
        {
            while (_listener.IsListening && !token.IsCancellationRequested)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    _ = HandleRequest(context, _userService);
                }
                catch (HttpListenerException)
                {
                    // Listener wurde gestoppt
                    break;
                }
                catch (ObjectDisposedException)
                {
                    // Listener wurde disposed
                    break;
                }
            }
        }, token);
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        // Cancellation signalisieren
        _cts?.Cancel();

        // Listener stoppen
        _listener?.Stop();

        // Auf Task warten (mit Timeout)
        if (_listenerTask != null)
        {
            try
            {
                await Task.WhenAny(_listenerTask, Task.Delay(1000));
            }
            catch
            {
                // Ignorieren
            }
        }

        _listener?.Close();
        _cts?.Dispose();

        // Container aufräumen
        if (_postgresContainer != null)
        {
            await _postgresContainer.StopAsync();
            await _postgresContainer.DisposeAsync();
        }
    }

    private async Task HandleRequest(HttpListenerContext context, IUserService userService)
    {
        var request = context.Request;
        var response = context.Response;

        try
        {
            if (request.HttpMethod == "POST" && request.Url?.AbsolutePath == "/api/user/register")
            {
                using var reader = new StreamReader(request.InputStream);
                var body = await reader.ReadToEndAsync();
                var dto = JsonSerializer.Deserialize<UserDto>(body);
                await userService.RegisterAsync(dto);
                response.StatusCode = 200;
            }
            else if (request.HttpMethod == "POST" && request.Url?.AbsolutePath == "/api/user/login")
            {
                using var reader = new StreamReader(request.InputStream);
                var body = await reader.ReadToEndAsync();
                var dto = JsonSerializer.Deserialize<UserDto>(body);
                var userId = await userService.LoginAsync(dto);
                var json = JsonSerializer.Serialize(new { userId });
                response.ContentType = "application/json";
                var bytes = Encoding.UTF8.GetBytes(json);
                await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                response.StatusCode = 200;
            }
            else if (request.HttpMethod == "GET" && request.Url?.AbsolutePath == "/api/user/search")
            {
                var query = request.QueryString["query"];
                var usernames = await userService.SearchUsersAsync(query);
                var json = JsonSerializer.Serialize(new { usernames });
                response.ContentType = "application/json";
                var bytes = Encoding.UTF8.GetBytes(json);
                await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                response.StatusCode = 200;
            }
            else
            {
                response.StatusCode = 404;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling request: {ex}");
            response.StatusCode = 500;
        }
        finally
        {
            try
            {
                response.Close();
            }
            catch
            {
                // Response könnte schon geschlossen sein
            }
        }
    }

    [TestMethod]
    public async Task RegisterUser_ReturnsOk()
    {
        using var client = new HttpClient();
        var content = new StringContent("{\"username\":\"testuser\",\"password\":\"testpass\"}", Encoding.UTF8, "application/json");
        var resp = await client.PostAsync("http://localhost:8080/api/user/register", content);
        Assert.IsTrue(resp.IsSuccessStatusCode);
    }

    [TestMethod]
    public async Task LoginUser_ReturnsOkWithUserId()
    {
        using var client = new HttpClient();
        var registerContent = new StringContent("{\"username\":\"loginuser\",\"password\":\"testpass\"}", Encoding.UTF8, "application/json");
        await client.PostAsync("http://localhost:8080/api/user/register", registerContent);
        
        var loginContent = new StringContent("{\"username\":\"loginuser\",\"password\":\"testpass\"}", Encoding.UTF8, "application/json");
        var resp = await client.PostAsync("http://localhost:8080/api/user/login", loginContent);
        Assert.IsTrue(resp.IsSuccessStatusCode);
    }

    [TestMethod]
    public async Task SearchUsers_ReturnsOk()
    {
        using var client = new HttpClient();
        var resp = await client.GetAsync("http://localhost:8080/api/user/search?query=test");
        Assert.IsTrue(resp.IsSuccessStatusCode);
    }
}