using System.Text.Json;
using Shared;
using Shared.Logger;
using Shared.WebSocketMessages;

namespace chess_client.States;

public class GameMenuState : IGameState
{
    public event Action<GameInvitationPayload>? OnGameInvitation;
    public event Action<StartGamePayload>? OnStartGame;
    private readonly JsonParser _jsonParser = new();
    
    public void OnEnter() => GameLogger.Info("Entered MainMenu state.");
    public void OnExit() => GameLogger.Info("Leaving MainMenu state.");

    public Task HandleMessageAsync(WebSocketMessage message)
    {
        switch (message.Type)
        {
            case MessageType.GameInvitation: 
                GameLogger.Info("Received game invitation while in MainMenu state.");
                var payload = _jsonParser.DeserializeJsonElement<GameInvitationPayload>(message.Payload!.Value);
                OnGameInvitation?.Invoke(payload);
                break;
            case MessageType.StartGame:
                GameLogger.Info("Received start game while in MainMenuState.");
                var startGamePayload = _jsonParser.DeserializeJsonElement<StartGamePayload>(message.Payload!.Value);
                OnStartGame?.Invoke(startGamePayload);
                break;
            default:
                GameLogger.Debug($"GameMenuState received unhandled message type: {message.Type}");
                break;
        }
        return Task.CompletedTask;
    }
}