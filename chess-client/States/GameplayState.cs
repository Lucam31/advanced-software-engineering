using Shared;
using Shared.Logger;
using Shared.WebSocketMessages;

namespace chess_client.States;

/// <summary>
/// Handles incoming WebSocket messages while a match is in progress.
/// </summary>
public class GameplayState : IGameState
{
    private TaskCompletionSource<StartGamePayload>? _startGameTcs;
    private TaskCompletionSource<GameTurnPayload>? _gameTurnTcs;
    private TaskCompletionSource<GameOverPayload>? _gameOverTcs;
    private JsonParser _parser = new JsonParser();

    /// <summary>
    /// Returns a task that completes when the server sends a <c>StartGame</c> message.
    /// </summary>
    /// <returns>A task that resolves to the received start-game payload.</returns>
    public Task<StartGamePayload> WaitForGameStartAsync()
    {
        _startGameTcs = new TaskCompletionSource<StartGamePayload>();
        return _startGameTcs.Task;
    }

    /// <summary>
    /// Returns a task that completes when the server sends a <c>GameTurn</c> message.
    /// </summary>
    /// <returns>A task that resolves to the received turn payload.</returns>
    public Task<GameTurnPayload> WaitForOpponentTurnAsync()
    {
        _gameTurnTcs = new TaskCompletionSource<GameTurnPayload>();
        return _gameTurnTcs.Task;
    }

    /// <summary>
    /// Returns a task that completes when the server sends a <c>GameOver</c> message.
    /// </summary>
    /// <returns>A task that resolves to the received game-over payload.</returns>
    public Task<GameOverPayload> WaitForGameOverAsync()
    {
        _gameOverTcs = new TaskCompletionSource<GameOverPayload>();
        return _gameOverTcs.Task;
    }

    /// <summary>
    /// Executes logic when entering the gameplay state.
    /// </summary>
    public void OnEnter() => GameLogger.Info("Entered Gameplay state.");

    /// <summary>
    /// Executes logic when leaving the gameplay state.
    /// </summary>
    public void OnExit() => GameLogger.Info("Leaving Gameplay state.");

    /// <summary>
    /// Handles gameplay-related WebSocket messages.
    /// </summary>
    /// <param name="message">The incoming WebSocket message.</param>
    /// <returns>A completed task after message processing finishes.</returns>
    public Task HandleMessageAsync(WebSocketMessage message)
    {
        switch (message.Type)
        {
            case MessageType.StartGame:
                GameLogger.Info("Received StartGame from server.");
                var startPayload = _parser.DeserializeJson<StartGamePayload>(message.Payload!.Value.GetRawText());
                if (startPayload != null)
                {
                    _startGameTcs?.TrySetResult(startPayload);
                }

                break;

            case MessageType.GameTurn:
                GameLogger.Info("Received GameTurn from server.");
                var turnPayload = _parser.DeserializeJson<GameTurnPayload>(message.Payload!.Value.GetRawText());
                if (turnPayload != null)
                {
                    _gameTurnTcs?.TrySetResult(turnPayload);
                }

                break;

            case MessageType.GameOver:
                GameLogger.Info("Received GameOver from server.");
                var gameOverPayload = _parser.DeserializeJson<GameOverPayload>(message.Payload!.Value.GetRawText());
                if (gameOverPayload != null)
                {
                    _gameOverTcs?.TrySetResult(gameOverPayload);
                    ConsoleHelper.PrintConsoleNewline(gameOverPayload.Winner != null
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