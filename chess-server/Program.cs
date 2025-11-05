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

        var enableConsoleLog = !string.Equals(Environment.GetEnvironmentVariable("CONSOLE_LOG"), "false",
            StringComparison.OrdinalIgnoreCase);
        var enableFileLog = !string.Equals(Environment.GetEnvironmentVariable("FILE_LOG"), "false",
            StringComparison.OrdinalIgnoreCase);

        GameLogger.Configure(
            minLevel: LogLevel.Debug,
            logToConsole: enableConsoleLog,
            logToFile: enableFileLog,
            logFilePath: "logs/server_log.txt"
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

            container.Register<IDatabase, Database>(() => new Database(connectionString));
            container.Register<IUserRepository, UserRepository>(() => new UserRepository(container.Resolve<IDatabase>()));
            container.Register<IGameRepository, GameRepository>(() => new GameRepository(container.Resolve<IDatabase>()));
            container.Register<IFriendsRepository, FriendsRepository>(() => new FriendsRepository(container.Resolve<IDatabase>()));
            container.Register<IUserService, UserService>(() => new UserService(container.Resolve<IUserRepository>()));
            container.Register<IGameService, GameService>(() => new GameService(
                container.Resolve<IGameRepository>(),
                container.Resolve<IUserRepository>()));
            container.Register<IFriendsService, FriendsService>(() => new FriendsService(
                container.Resolve<IFriendsRepository>(),
                container.Resolve<IUserRepository>()));
            container.Register<UserController>(() => new UserController(container.Resolve<IUserService>()));
            container.Register<GameController>(() => new GameController(container.Resolve<IGameService>()));
            container.Register<FriendsController>(() => new FriendsController(container.Resolve<IFriendsService>()));
            GameLogger.Debug("DI container configured.");

            var router = new Router(container, new ExceptionMiddleware());

            router.RegisterController<UserController>();
            router.RegisterController<GameController>();
            router.RegisterController<FriendsController>();
            router.RegisterHub(new WebSocketHub());

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