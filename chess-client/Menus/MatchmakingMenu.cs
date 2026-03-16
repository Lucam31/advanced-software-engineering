using chess_client.Services;
using chess_client.UserInterface;
using Shared.Logger;
using Shared.WebSocketMessages;

namespace chess_client.Menus;

/// <summary>
/// Manages the logic and server communication for the matchmaking queue.
/// </summary>
public class MatchmakingMenu(
    IGameService gameService,
    WebSocketService webSocketService)
{
    private readonly MatchmakingUi _ui = new();

    /// <summary>
    /// Enters the matchmaking queue and handles user input/cancellation.
    /// </summary>
    /// <returns>The StartGamePayload if a game is found, otherwise null.</returns>
    public async Task<StartGamePayload?> EnterQueueAsync(CancellationToken token, Func<StartGamePayload?> getPendingStartGame)
    {
        GameLogger.Info("Entering matchmaking queue...");
        string? queueError = null;
        
        _ui.DrawQueueScreen();
        await gameService.SearchGame();

        while (true)
        {
            if (queueError != null)
            {
                _ui.DrawQueueScreen(queueError);
                queueError = null;
            }

            try
            {
                var input = (await _ui.ReadInputAsync(token))?.Trim().ToUpper();
                
                if (input == "Q")
                {
                    var cancelMessage = new WebSocketMessage
                    {
                        Type = MessageType.CancelSearch,
                        Payload = null
                    };
                    await webSocketService.SendAsync(cancelMessage);
                    
                    _ui.ShowMessage("Matchmaking cancelled. Returning to menu...");
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
                // Dieser Block feuert, wenn über WebSockets ein Gegner gefunden wurde
                var pendingGame = getPendingStartGame();
                if (pendingGame != null)
                {
                    return pendingGame;
                }
                
                // Fallback, falls die Cancellation aus einem anderen Grund geworfen wurde
                return null;
            }
        }
    }
}