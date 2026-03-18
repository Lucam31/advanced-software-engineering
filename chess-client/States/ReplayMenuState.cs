using Shared.WebSocketMessages;
using Shared.Logger;

namespace chess_client.States;

/// <summary>
/// Represents the replay menu state and logs messages that are not handled there.
/// </summary>
public class ReplayMenuState : IGameState
{
    /// <summary>
    /// Executes logic when entering the replay menu state.
    /// </summary>
    public void OnEnter() => GameLogger.Info("Entered ReplayMenu state.");

    /// <summary>
    /// Executes logic when leaving the replay menu state.
    /// </summary>
    public void OnExit() => GameLogger.Info("Leaving ReplayMenu state.");

    /// <summary>
    /// Handles incoming messages while the replay menu state is active.
    /// </summary>
    /// <param name="message">The incoming WebSocket message.</param>
    /// <returns>A completed task after message handling finishes.</returns>
    public Task HandleMessageAsync(WebSocketMessage message)
    {
        switch (message.Type)
        {
            default:
                GameLogger.Debug($"ReplayMenuState received unhandled message type: {message.Type}");
                break;
        }

        return Task.CompletedTask;
    }
}