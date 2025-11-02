namespace chess_server.Models;

public class Friendship
{
    public Guid Id { get; set; }
    public Guid UserId1 { get; set; }
    public Guid UserId2 { get; set; }
    public string Status { get; set; } = "pending";
    public Guid InitiatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
