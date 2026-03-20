using chess_client.Services;
using chess_client.UserInterface;
using Shared.Logger;
using Shared.WebSocketMessages;

namespace chess_client.Menus;

/// <summary>
/// Coordinates queue-based matchmaking, including queue cancellation and start-game handoff.
/// </summary>
/// <param name="gameService">Service used to enter matchmaking.</param>
/// <param name="webSocketService">WebSocket connection used to send queue cancellation messages.</param>
public class MatchmakingMenu(
    IGameService gameService,
    WebSocketService webSocketService)
{
    private readonly MatchmakingUi _ui = new();

    /// <summary>
    /// Enters the matchmaking queue and waits for either user cancellation or a game start event.
    /// </summary>
    /// <param name="getPendingStartGame">Callback returning the latest pending start-game payload, if available.</param>
    /// <param name="token">Cancellation token triggered when external realtime events interrupt queue input.</param>
    /// <returns>The <see cref="StartGamePayload"/> when a game is found; otherwise <c>null</c>.</returns>
    public async Task<StartGamePayload?> EnterQueueAsync(Func<StartGamePayload?> getPendingStartGame,
        CancellationToken token)
    {
        GameLogger.Info("Entering matchmaking queue...");
        string? queueError = null;

        MatchmakingUi.DrawQueueScreen();
        await gameService.SearchGame();

        while (true)
        {
            if (queueError != null)
            {
                MatchmakingUi.DrawQueueScreen(queueError);
                queueError = null;
            }

            try
            {
                var input = await BaseMenuUi.ReadKeyAsync(token);

                if (input.Key == ConsoleKey.Q)
                {
                    var cancelMessage = new WebSocketMessage
                    {
                        Type = MessageType.CancelSearch,
                        Payload = null
                    };
                    await webSocketService.SendAsync(cancelMessage);

                    BaseMenuUi.ShowMessage("Matchmaking cancelled. Returning to menu...");
                    await Task.Delay(1000, token);
                    return null;
                }
                else
                {
                    queueError = "Invalid input. Press Q to quit.";
                }
            }
            catch (OperationCanceledException)
            {
                var pendingGame = getPendingStartGame();
                return pendingGame ?? null;
            }
        }
    }
}