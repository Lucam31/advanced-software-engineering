using Shared.Logger;
using Shared.WebSocketMessages;

namespace chess_client.States;

public class GameMenuState : IGameState
{
    public void OnEnter() => GameLogger.Info("Entered MainMenu state.");
    public void OnExit() => GameLogger.Info("Leaving MainMenu state.");

    public Task HandleMessageAsync(WebSocketMessage message)
    {
        switch (message.Type)
        {
            // If user is in game menu
            default:
                GameLogger.Debug($"GameMenuState received unhandled message type: {message.Type}");
                break;
        }
        return Task.CompletedTask;
    }
}