namespace chess_server.OutputDtos;

public class PlayedGame
{
    public Guid Id { get; set; }
    public string WhitePlayerUsername { get; set; } = "";
    public string BlackPlayerUsername { get; set; } = "";
    public List<string> Moves { get; set; } = new();
}