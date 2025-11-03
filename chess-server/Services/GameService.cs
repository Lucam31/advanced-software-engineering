using chess_server.Models;
using chess_server.OutputDtos;
using chess_server.Repositories;
using Shared.Exceptions;
using Shared.InputDtos;

namespace chess_server.Services;

/// <summary>
/// Defines the interface for game-related business logic.
/// </summary>
public interface IGameService
{
    /// <summary>
    /// Inserts a new game record.
    /// </summary>
    /// <param name="dto">The game data transfer object.</param>
    Task InsertGameAsync(InsertGame dto);

    /// <summary>
    /// Retrieves a list of the last globally played games.
    /// </summary>
    /// <returns>A list of <see cref="PlayedGame"/> objects.</returns>
    Task<List<PlayedGame>> GetLastGlobalPlayedGamesAsync();

    /// <summary>
    /// Retrieves a list of the last games played by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of <see cref="PlayedGame"/> objects.</returns>
    Task<List<PlayedGame>> GetLastUserPlayedGamesAsync(Guid userId);
}

/// <summary>
/// Implements the business logic for game-related operations.
/// </summary>
public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameService"/> class.
    /// </summary>
    /// <param name="gameRepository">The game repository.</param>
    /// <param name="userRepository">The user repository.</param>
    public GameService(IGameRepository gameRepository, IUserRepository userRepository)
    {
        _gameRepository = gameRepository;
        _userRepository = userRepository;
    }

    /// <inheritdoc/>
    public async Task InsertGameAsync(InsertGame dto)
    {
        var game = new Game
        {
            Guid = dto.Id,
            WhitePlayerId = dto.WhitePlayerId,
            BlackPlayerId = dto.BlackPlayerId,
            Moves = dto.Moves
        };

        await _gameRepository.InsertGameAsync(game);
    }

    /// <inheritdoc/>
    public async Task<List<PlayedGame>> GetLastGlobalPlayedGamesAsync()
    {
        var games = await _gameRepository.GetRecentGamesAsync();

        var playedGames = new List<PlayedGame>();

        foreach (var g in games)
        {
            var whitePlayer = _userRepository.GetUserByIdAsync(g.WhitePlayerId).Result;
            var blackPlayer = _userRepository.GetUserByIdAsync(g.BlackPlayerId).Result;

            if (whitePlayer == null || blackPlayer == null)
                throw new UserNotFound();

            playedGames.Add(new PlayedGame
            {
                Id = g.Guid,
                WhitePlayerUsername = whitePlayer.Username,
                BlackPlayerUsername = blackPlayer.Username,
                Moves = g.Moves
            });
        }

        return playedGames;
    }

    /// <inheritdoc/>
    public async Task<List<PlayedGame>> GetLastUserPlayedGamesAsync(Guid userId)
    {
        var games = await _gameRepository.GetGamesByUserIdAsync(userId);

        var playedGames = new List<PlayedGame>();

        foreach (var g in games)
        {
            var whitePlayer = _userRepository.GetUserByIdAsync(g.WhitePlayerId).Result;
            var blackPlayer = _userRepository.GetUserByIdAsync(g.BlackPlayerId).Result;

            if (whitePlayer == null || blackPlayer == null)
                throw new UserNotFound();

            playedGames.Add(new PlayedGame
            {
                Id = g.Guid,
                WhitePlayerUsername = whitePlayer.Username,
                BlackPlayerUsername = blackPlayer.Username,
                Moves = g.Moves
            });
        }

        return playedGames;
    }
}