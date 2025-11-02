namespace Shared.InputDtos;

public class FriendRequest
{
    public Guid UserId { get; set; }
    public string FriendUsername { get; set; } = string.Empty;
}