using System.Text.Json;
using Shared.Logger;
using Shared.WebSocketMessages;

namespace chess_client.States;

public class FriendshipMenuState : IGameState
{
    
    public event Action? OnFriendsRefreshRequested;

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
        }
        return Task.CompletedTask;
    }
}