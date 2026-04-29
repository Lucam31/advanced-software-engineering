using Shared;
using Shared.Logger;
using Shared.WebSocketMessages;

namespace chess_client.States;

/// <summary>
/// Handles incoming messages while the game menu is active.
/// </summary>
public class GameMenuState : IGameState
{
    /// <summary>
    /// Raised when a game invitation is received.
    /// </summary>
    public event Action<GameInvitationPayload>? OnGameInvitation;

    /// <summary>
    /// Raised when a start-game message is received.
    /// </summary>
    public event Action<StartGamePayload>? OnStartGame;
    private readonly JsonParser _jsonParser = new();

    /// <summary>
    /// Executes logic when entering the game menu state.
    /// </summary>
    public void OnEnter() => GameLogger.Info("Entered MainMenu state.");

    /// <summary>
    /// Executes logic when leaving the game menu state.
    /// </summary>
    public void OnExit() => GameLogger.Info("Leaving MainMenu state.");

    /// <summary>
    /// Handles messages relevant to game-menu interactions.
    /// </summary>
    /// <param name="message">The incoming WebSocket message.</param>
    /// <returns>A completed task after processing the message.</returns>
    public Task HandleMessageAsync(WebSocketMessage message)
    {
        switch (message.Type)
        {
            case MessageType.GameInvitation:
                GameLogger.Info("Received game invitation while in MainMenu state.");
                var payload = _jsonParser.DeserializeJsonElement<GameInvitationPayload>(message.Payload!.Value);
                OnGameInvitation?.Invoke(payload!);
                break;
            case MessageType.StartGame:
                GameLogger.Info("Received start game while in MainMenuState.");
                var startGamePayload = _jsonParser.DeserializeJsonElement<StartGamePayload>(message.Payload!.Value);
                OnStartGame?.Invoke(startGamePayload!);
                break;
            default:
                GameLogger.Debug($"GameMenuState received unhandled message type: {message.Type}");
                break;
        }

        return Task.CompletedTask;
    }
}