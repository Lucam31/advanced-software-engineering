using Npgsql.Replication;

namespace chess_server.Api.Hub;

public interface  IActiveGame
{
    
}

public class ActiveGame : IActiveGame
{
    public Guid Id { get; private set; }
    private Guid WhitePlayerId { get; set; }
    private Guid? BlackPlayerId { get; set; }
    
    public ActiveGame(Guid id, Guid whitePlayerId)
    {
        Id = id;
        WhitePlayerId = id;
    }

    public void JoinGame(Guid clientId)
    {
        BlackPlayerId = clientId;
    }
    
}