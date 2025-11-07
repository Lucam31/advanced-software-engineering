namespace Shared.WebSocketMessages;

public static class MessageType
{
    // game related messages
    public const string ResyncGame = "RESYNC_GAME";
    public const string CreateGame = "CREATE_GAME";
    public const string GameCreated = "GAME_CREATED";
    public const string GameInvitation = "GAME_INVITATION";
    public const string JoinGame = "JOIN_GAME";
    public const string StartGame = "START_GAME";
    public const string MakeMove = "MAKE_MOVE";
    // spectator related messages
    public const string SpectatorJoin = "SPECTATOR_JOIN";
    public const string SpectatorLeave = "SPECTATOR_LEAVE";
    public const string SpectatorUpdate = "SPECTATOR_UPDATE";
    // friend related messages
    public const string FetchFriendRequest = "FETCH_FRIEND_REQUEST";
    public const string Error = "ERROR";
}