namespace chess_client.States;

using Shared.WebSocketMessages;
using Shared.Logger;

public class ReplayMenuState : IGameState
{
    public void OnEnter() => GameLogger.Info("Entered ReplayMenu state.");
    public void OnExit() => GameLogger.Info("Leaving ReplayMenu state.");

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