using chess_server.API;
using chess_server.API.Controller;
using chess_server.Data;
using chess_server.Models;
using chess_server.Repositories;
using chess_server.Services;

namespace chess_server;
class Program
{
    static async Task Main()
    {
        
        string? connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? "fallback_connection_string";
        
        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("Umgebungsvariable 'DB_CONNECTION_STRING' nicht gesetzt.");
        
        var database = new Database(connectionString);
        var userRepo = new UserRepository(database);
        var userService = new UserService(userRepo);

        var router = new Router();
        
        router.RegisterController<UserController>(() => new UserController(userService));

        Console.WriteLine("Routes registered");
    
        var api = new Api(router);
        await api.Run();
    }
}