using Npgsql.Replication;
using Shared;
using Shared.Dtos;

namespace chess_server.Api.Hub;

public interface  IActiveGame
{
    /// <summary>
    /// Sets the client as a black player in the game.
    /// </summary>
    /// <param name="client"></param>
    void JoinGame(Player client);
}

public class ActiveGame(Guid id, Player whitePlayer) : IActiveGame
{
    /// <summary>
    /// The unique identifier of the game.
    /// </summary>
    public Guid Id { get; init; } = id;
    /// <summary>
    /// The player playing as white.
    /// </summary>
    private Player WhitePlayer { get; set; } = whitePlayer;
    /// <summary>
    /// The player playing as black.
    /// </summary>
    private Player? BlackPlayer { get; set; }
    /// <summary>
    /// The list of spectators watching the game.
    /// </summary>
    private List<Guid> Spectators { get; set; } = new();
    /// <summary>
    /// The history of moves made in the game, stored as a list of strings.
    /// </summary>
    private List<String> MoveHistory { get; set; } = new();
    /// <summary>
    /// Synchronization object for thread-safe operations.
    /// </summary>
    internal readonly object SyncRoot = new();

    /// <summary>
    /// Joins a player as the black player in the game.
    /// </summary>
    /// <param name="client">The client who wants to join the game.</param>
    public void JoinGame(Player client)
    {
        BlackPlayer = client;
    }
    
    /// <summary>
    /// Appends a move to the move history of the game in a thread-safe manner.
    /// </summary>
    /// <param name="move"></param>
    public void AppendMove(String move)
    {
        lock (SyncRoot)
        {
            MoveHistory.Add(move);
        }
    }
    
    /// <summary>
    /// Retrieves the move history of the game in a thread-safe manner.
    /// </summary>
    public List<String> GetMoveHistory()
    {
        lock (SyncRoot)
        {
            return new List<String>(MoveHistory);
        }
    }
    
    /// <summary>
    /// Gets the unique identifier of the white player.
    /// </summary>
    /// <returns>Id</returns>
    public Guid GetWhitePlayerId () => WhitePlayer.Id;
    /// <summary>
    /// Gets the unique identifier of the black player, or null if no black player has joined yet.
    /// </summary>
    /// <returns>Id or null</returns>
    public Guid? GetBlackPlayerId () => BlackPlayer?.Id;
}