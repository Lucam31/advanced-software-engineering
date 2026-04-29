using Shared;
using Shared.Logger;
using Shared.WebSocketMessages;

namespace chess_client.States;

/// <summary>
/// Handles incoming messages while the friendship menu is active.
/// </summary>
public class FriendshipMenuState : IGameState
{
    private readonly JsonParser _jsonParser = new();

    /// <summary>
    /// Raised when the friends list should be refreshed.
    /// </summary>
    public event Action? OnFriendsRefreshRequested;

    /// <summary>
    /// Raised when a start-game message is received.
    /// </summary>
    public event Action<StartGamePayload>? OnStartGame;

    /// <summary>
    /// Executes logic when entering the friendship menu state.
    /// </summary>
    public void OnEnter() => GameLogger.Info("Entered FriendshipMenu state.");

    /// <summary>
    /// Executes logic when leaving the friendship menu state.
    /// </summary>
    public void OnExit() => GameLogger.Info("Leaving FriendshipMenu state.");

    /// <summary>
    /// Handles messages relevant to friendship-menu behavior.
    /// </summary>
    /// <param name="message">The incoming WebSocket message.</param>
    /// <returns>A completed task after processing the message.</returns>
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
                OnStartGame?.Invoke(startGamePayload!);
                break;
            default:
                GameLogger.Error($"Unknown message type {message.Type}");
                break;
        }

        return Task.CompletedTask;
    }
}