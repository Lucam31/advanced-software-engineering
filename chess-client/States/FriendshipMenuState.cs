using Shared;
using Shared.Logger;
using Shared.WebSocketMessages;

namespace chess_client.States;

public class FriendshipMenuState : IGameState
{
    private readonly JsonParser _jsonParser = new();
    
    public event Action? OnFriendsRefreshRequested;
    public event Action<StartGamePayload>? OnStartGame;

    public void OnEnter() => GameLogger.Info("Entered FriendshipMenu state.");
    public void OnExit() => GameLogger.Info("Leaving FriendshipMenu state.");

    public Task HandleMessageAsync(WebSocketMessage message)
    {
        switch (message.Type)
        {
            case MessageType.FetchFriends:
                GameLogger.Info("FetchFriends received — triggering refresh.");
                OnFriendsRefreshRequested?.Invoke();
                break;
            case MessageType.StartGame:
                GameLogger.Info("Received start game while in FriendshipMenu.");
                var startGamePayload = _jsonParser.DeserializeJsonElement<StartGamePayload>(message.Payload!.Value);
                OnStartGame?.Invoke(startGamePayload);
                break;
            default:
                GameLogger.Error($"Unknown message type {message.Type}");
                break;
        }
        return Task.CompletedTask;
    }
}