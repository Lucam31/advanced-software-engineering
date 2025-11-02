using chess_server.Api.ActionResults;
using chess_server.Api.Attributes;
using chess_server.Services;
using Shared.InputDtos;

namespace chess_server.Api.Controller;

public interface IGameController
{
    Task<IActionResult> InsertGameAsync(InsertGame dto);
    Task<IActionResult> GetLastGlobalPlayedGamesAsync();
    Task<IActionResult> GetLastUserPlayedGamesAsync(Guid userId);
}

[Route("api/games")]
public class GameController : IGameController
{
    private readonly IGameService _gameService;
    
    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }
    
    [HttpMethod("POST")]
    [Route("/insert")]
    public async Task<IActionResult> InsertGameAsync(InsertGame dto)
    {
        await _gameService.InsertGameAsync(dto);
        return Results.Ok();
    }

    [HttpMethod("GET")]
    [Route("/recent/global")]
    public async Task<IActionResult> GetLastGlobalPlayedGamesAsync()
    {
        var games = await _gameService.GetLastGlobalPlayedGamesAsync();
        return Results.Ok(games);
    }
    
    [HttpMethod("GET")]
    [Route("/recent/user")]
    public async Task<IActionResult> GetLastUserPlayedGamesAsync([FromQuery] Guid userId)
    {
        var games = await _gameService.GetLastUserPlayedGamesAsync(userId);
        return Results.Ok(games);
    }
}