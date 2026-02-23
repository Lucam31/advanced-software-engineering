namespace chess_client.States;

using Shared.WebSocketMessages;
using Shared.Logger;

public class MainMenuState : IGameState
{
    public void OnEnter() => GameLogger.Info("Entered MainMenu state.");
    public void OnExit() => GameLogger.Info("Leaving MainMenu state.");

    public Task HandleMessageAsync(WebSocketMessage message)
    {
        switch (message.Type)
        {
                // Hier könnten weitere Nachrichten behandelt werden, z.B. Benachrichtigungen über neue Freundschaftsanfragen
                default:
                    GameLogger.Debug($"MainMenuState received unhandled message type: {message.Type}");
                    break;
        }
        return Task.CompletedTask;
    }
}