namespace chess_server.Models;

public class User
{
    public Guid Id { get; init; }
    public required string Username { get; init; }
    public required byte[] PasswordHash { get; init; }
    public required byte[] PasswordSalt { get; init; }
    public required int Rating { get; set; }
}