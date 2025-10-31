using Shared.Models;

namespace Shared.InputDtos;

public class UpdateFriendship
{
    public Guid FriendshipId { get; set; }
    public FriendshipStatus Status { get; set; }
}