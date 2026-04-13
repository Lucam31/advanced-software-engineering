using chess_server.Api;
using chess_server.Api.Controller;
using chess_server.Data;
using chess_server.Repositories;
using chess_server.Services;
using System.Text;
using chess_server.Api.Hub;
using chess_server.Api.Middlewares;
using Shared.Logger;

namespace chess_server;

/// <summary>
/// The main entry point for the chess server application.
/// </summary>
internal static class Program
{
    /// <summary>
    /// The main method that configures and runs the server.
    /// </summary>
    private static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        GameLogger.Configure(
            minLevel: LogLevel.Debug,
            logToConsole: true,
            logToFile: true,
            logFilePath: "logs/server.log"
        );

        try
        {
            GameLogger.Info("Server application starting...");

            GameLogger.Debug("Reading connection string...");
            var connectionString =
                Environment.GetEnvironmentVariable("DATABASE_URL") ?? "fallback_connection_string";

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("'DB_CONNECTION_STRING' is not set");
            }

            GameLogger.Debug("Connection string loaded.");

            GameLogger.Debug("Initializing Dependency Injection container...");
            var container = new DiContainer();

            var dbConfig = new DatabaseConfig(connectionString);

            container.Register<DatabaseConfig>(() => dbConfig);
            container.Register<IDatabase, Database>();
            container.Register<IUserRepository, UserRepository>();
            container.Register<IGameRepository, GameRepository>();
            container.Register<IFriendsRepository, FriendsRepository>();
            container.Register<IUserService, UserService>();
            container.Register<IGameService, GameService>();
            container.Register<IFriendsService, FriendsService>();
            container.Register<UserController>();
            container.Register<GameController>();
            container.Register<FriendsController>();
            GameLogger.Debug("DI container configured.");

            var router = new Router(container, new ExceptionMiddleware());

            var webSocketHub = new WebSocketHub(container.Resolve<IGameService>());
            container.Register<INotificationSender>(() => new NotificationSender(webSocketHub.NotificationWriter));

            router.RegisterController<UserController>();
            router.RegisterController<GameController>();
            router.RegisterController<FriendsController>();
            router.RegisterHub(webSocketHub);

            GameLogger.Info("Routes registered successfully.");

            var api = new Api.Api(router);

            GameLogger.Info("API is configured. Starting server run loop...");
            await api.Run();

            GameLogger.Info("Server run loop finished. Shutting down.");
        }
        catch (Exception ex)
        {
            GameLogger.Fatal("An unhandled exception occurred! Server will crash.", ex);
            throw;
        }
    }
}