using chess_server.Models;
using chess_server.OutputDtos;
using chess_server.Repositories;
using Shared.Exceptions;
using Shared.InputDtos;

namespace chess_server.Services;

public interface IGameService
{
    Task InsertGameAsync(InsertGame dto);
    Task<List<PlayedGame>> GetLastGlobalPlayedGamesAsync();
    Task<List<PlayedGame>> GetLastUserPlayedGamesAsync(Guid userId);
}

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IUserRepository _userRepository;
    
    public GameService(IGameRepository gameRepository, IUserRepository userRepository)
    {
        _gameRepository = gameRepository;
        _userRepository = userRepository;
    }
    
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