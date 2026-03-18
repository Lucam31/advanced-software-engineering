using Shared.WebSocketMessages;

namespace chess_client.States;

/// <summary>
/// Defines the lifecycle and message-handling contract for a client game state.
/// </summary>
public interface IGameState
{
    /// <summary>
    /// Executes logic when the state becomes active.
    /// </summary>
    void OnEnter();

    /// <summary>
    /// Executes cleanup logic when the state is no longer active.
    /// </summary>
    void OnExit();

    /// <summary>
    /// Processes an incoming WebSocket message while this state is active.
    /// </summary>
    /// <param name="message">The received WebSocket message.</param>
    /// <returns>A task that completes after the message has been handled.</returns>
    Task HandleMessageAsync(WebSocketMessage message);
}