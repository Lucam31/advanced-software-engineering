using chess_server.Api;
using chess_server.Api.Controller;
using chess_server.Data;
using chess_server.Repositories;
using chess_server.Services;

namespace chess_server;
class Program
{
    static async Task Main()
    {
        
        string? connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? "fallback_connection_string";
        
        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("'DB_CONNECTION_STRING' is not set");

        var container = new DiContainer();
        
        container.Register<IDatabase,Database>(() => new Database(connectionString));
        container.Register<IUserRepository,UserRepository>(() => new UserRepository(container.Resolve<IDatabase>()));
        container.Register<IUserService,UserService>(() => new UserService(container.Resolve<IUserRepository>()));
        container.Register<UserController>(() => new UserController(container.Resolve<IUserService>()));

        var router = new Router(container);
        
        router.RegisterController<UserController>();

        Console.WriteLine("Routes registered");
    
        var api = new Api.Api(router);
        await api.Run();
    }
}