using Shared.WebSocketMessages;

namespace chess_client.States;

public interface IGameState
{
    void OnEnter();
    void OnExit();
    Task HandleMessageAsync(WebSocketMessage message);
}