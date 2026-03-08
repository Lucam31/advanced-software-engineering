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
                case MessageType.GameInvitation:
                    GameLogger.Info("Received game invitation while in MainMenu state.");
                    
                    break;
                default:
                    GameLogger.Debug($"MainMenuState received unhandled message type: {message.Type}");
                    break;
        }
        return Task.CompletedTask;
    }
}