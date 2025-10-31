namespace chess_server.OutputDtos;

public class PendingFriendRequest
{
    public Guid RequestId { get; set; }
    public string FromUsername { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
