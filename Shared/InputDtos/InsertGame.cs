namespace Shared.InputDtos;

public class InsertGame
{
    public Guid Id { get; set; }
    public Guid WhitePlayerId { get; set; }
    public Guid BlackPlayerId { get; set; }
    public List<string> Moves { get; set; } = new();
}