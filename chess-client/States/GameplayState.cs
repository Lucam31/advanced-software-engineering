using System.Text.Json;
using Shared.Logger;
using Shared.WebSocketMessages;

namespace chess_client.States;

/// <summary>
/// Handles WebSocket messages during active gameplay
/// </summary>
public class GameplayState : IGameState
{
    private TaskCompletionSource<StartGamePayload>? _startGameTcs;
    private TaskCompletionSource<GameTurnPayload>? _gameTurnTcs;

    /// <summary>
    /// Returns a task that completes when the server sends a StartGame message
    /// </summary>
    public Task<StartGamePayload> WaitForGameStartAsync()
    {
        _startGameTcs = new TaskCompletionSource<StartGamePayload>();
        return _startGameTcs.Task;
    }

    /// <summary>
    /// Returns a task that completes when the server sends a GameTurn message
    /// </summary>
    public Task<GameTurnPayload> WaitForOpponentTurnAsync()
    {
        _gameTurnTcs = new TaskCompletionSource<GameTurnPayload>();
        return _gameTurnTcs.Task;
    }

    public void OnEnter() => GameLogger.Info("Entered Gameplay state.");
    public void OnExit() => GameLogger.Info("Leaving Gameplay state.");

    public Task HandleMessageAsync(WebSocketMessage message)
    {
        switch (message.Type)
        {
            case MessageType.StartGame:
                GameLogger.Info("Received StartGame from server.");
                var startPayload = JsonSerializer.Deserialize<StartGamePayload>(message.Payload.GetRawText());
                if (startPayload != null)
                {
                    _startGameTcs?.TrySetResult(startPayload);
                }
                break;

            case MessageType.GameTurn:
                GameLogger.Info("Received GameTurn from server.");
                var turnPayload = JsonSerializer.Deserialize<GameTurnPayload>(message.Payload.GetRawText());
                if (turnPayload != null)
                {
                    _gameTurnTcs?.TrySetResult(turnPayload);
                }
                break;

            case MessageType.GameOver:
                GameLogger.Info("Received GameOver from server.");
                var gameOverPayload = JsonSerializer.Deserialize<GameOverPayload>(message.Payload.GetRawText());
                if (gameOverPayload != null)
                {
                    CliOutput.PrintConsoleNewline(gameOverPayload.Winner != null
                        ? $"Game over! Winner: {gameOverPayload.Winner}"
                        : "Game over! It's a draw!");
                }
                break;

            default:
                GameLogger.Debug($"GameplayState received unhandled message type: {message.Type}");
                break;
        }

        return Task.CompletedTask;
    }
}

