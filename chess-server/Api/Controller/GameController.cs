using chess_server.Api.ActionResults;
using chess_server.Api.Attributes;
using chess_server.Services;
using Shared.Dtos;
using Shared.Logger;

namespace chess_server.Api.Controller;

/// <summary>
/// Defines the interface for the game controller.
/// </summary>
public interface IGameController
{
    /// <summary>
    /// Inserts a new game record.
    /// </summary>
    /// <param name="dto">The game data.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
    Task<IActionResult> InsertGameAsync(InsertGame dto);

    /// <summary>
    /// Gets the last globally played games.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing a list of recent games.</returns>
    Task<IActionResult> GetLastGlobalPlayedGamesAsync();

    /// <summary>
    /// Gets the last games played by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>An <see cref="IActionResult"/> containing a list of the user's recent games.</returns>
    Task<IActionResult> GetLastUserPlayedGamesAsync(Guid userId);
}

/// <summary>
/// API controller for game-related actions.
/// </summary>
[Route("api/games")]
public class GameController : IGameController
{
    private readonly IGameService _gameService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameController"/> class.
    /// </summary>
    /// <param name="gameService">The game service.</param>
    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    /// <inheritdoc/>
    [HttpMethod("POST")]
    [Route("/insert")]
    public async Task<IActionResult> InsertGameAsync(InsertGame dto)
    {
        GameLogger.Info($"HTTP POST /api/games/insert Id={dto.Id} White={dto.WhitePlayerId} Black={dto.BlackPlayerId}");
        await _gameService.InsertGameAsync(dto);
        GameLogger.Info($"Inserted game {dto.Id}");
        return Results.Ok();
    }

    /// <inheritdoc/>
    [HttpMethod("GET")]
    [Route("/recent/global")]
    public async Task<IActionResult> GetLastGlobalPlayedGamesAsync()
    {
        GameLogger.Info("HTTP GET /api/games/recent/global");
        var games = await _gameService.GetLastGlobalPlayedGamesAsync();
        GameLogger.Info($"Returning {games.Count} recent global games");
        return Results.Ok(games);
    }

    /// <inheritdoc/>
    [HttpMethod("GET")]
    [Route("/recent/user")]
    public async Task<IActionResult> GetLastUserPlayedGamesAsync([FromQuery] Guid userId)
    {
        GameLogger.Info($"HTTP GET /api/games/recent/user?userId={userId}");
        var games = await _gameService.GetLastUserPlayedGamesAsync(userId);
        GameLogger.Info($"Returning {games.Count} games for user {userId}");
        return Results.Ok(games);
    }
}