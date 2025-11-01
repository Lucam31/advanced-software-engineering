namespace chess_server.Models;

public class Game
{
    public Guid Guid { get; init; }
    public required Guid WhitePlayerId { get; init; }
    public required Guid BlackPlayerId { get; init; }
    public required List<string> Moves { get; set; }
}