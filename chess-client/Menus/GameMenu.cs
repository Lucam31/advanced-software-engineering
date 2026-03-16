using chess_client.Services;
using chess_client.States;
using Shared.Logger;
using Shared.WebSocketMessages;
using chess_client.UserInterface;

namespace chess_client.Menus;

/// <summary>
/// Manages the main menu logic, state transitions, and server communication.
/// </summary>
public class GameMenu(
    UserContainer userContainer,
    FriendshipMenu friendshipMenu,
    IGameService gameService,
    WebSocketService webSocketService)
{
    private readonly GameMenuUi _ui = new();

    /// <summary>
    /// Displays the main menu and handles user input
    /// </summary>
    public async Task DisplayMainMenu()
    {
        string? currentErrorMessage = null;

        while (true)
        {
            var state = new GameMenuState();
            using var cts = new CancellationTokenSource();

            GameInvitationPayload? pendingInvitation = null;
            state.OnGameInvitation += payload =>
            {
                GameLogger.Info("Invitation-Refresh with opponentId " + payload.InviterId);
                pendingInvitation = payload;
                cts.Cancel();
            };

            StartGamePayload? pendingStartGame = null;
            state.OnStartGame += payload =>
            {
                GameLogger.Info("Game Start as color " + payload.Color);
                pendingStartGame = payload;
                cts.Cancel();
            };

            webSocketService.TransitionTo(state);

            GameLogger.Info("Displaying main menu.");

            _ui.DrawMainMenu(currentErrorMessage);
            currentErrorMessage = null;

            string? input = null;
            try
            {
                input = (await _ui.ReadInputAsync(cts.Token))?.ToUpper();
            }
            catch (OperationCanceledException)
            {
                if (pendingInvitation != null)
                {
                    GameLogger.Info("Received game invitation.");
                    _ui.ShowMessage("Received game invitation. Accepting...");
                    await gameService.AcceptGameInvitation(pendingInvitation.GameId);

                    while (pendingStartGame == null)
                    {
                        await Task.Delay(50);
                    }

                    GameLogger.Info("Starting game with ID " + pendingStartGame.GameId);
                    var game = new GameLogic();
                    await game.StartGame(webSocketService, pendingStartGame);
                }
                else if (pendingStartGame != null)
                {
                    GameLogger.Info("Starting game with ID " + pendingStartGame.GameId);
                    var game = new GameLogic();
                    await game.StartGame(webSocketService, pendingStartGame);
                }

                continue;
            }

            GameLogger.Debug($"User entered menu input: '{input}'");

            switch (input)
            {
                case "P":
                case "PLAY":
                    // NEU: Vollständig gekapselter Aufruf des Matchmaking-Menüs
                    var matchmakingMenu = new MatchmakingMenu(gameService, webSocketService);
                    
                    // Wir übergeben das Token und eine Funktion, um im Abbruchs-Fall an den Payload zu kommen
                    var startGamePayload = await matchmakingMenu.EnterQueueAsync(cts.Token, () => pendingStartGame);

                    // Wenn ein Spiel gefunden wurde (Rückgabe ist nicht null)
                    if (startGamePayload != null)
                    {
                        GameLogger.Info("Starting game with ID " + startGamePayload.GameId);
                        var game = new GameLogic();
                        await game.StartGame(webSocketService, startGamePayload);
                    }
                    break;

                case "F":
                case "FRIENDS":
                    GameLogger.Info("User selected 'Friends'.");
                    await friendshipMenu.DisplayMenu();
                    break;

                case "G":
                case "GAMES":
                    GameLogger.Info("User selected 'Games'.");
                    var replayMenu = new ReplayMenu(userContainer, webSocketService);
                    await replayMenu.DisplayMenu();
                    break;

                case "Q":
                case "QUIT":
                    GameLogger.Info("User selected 'Quit'.");
                    return;

                default:
                    GameLogger.Warning($"Invalid menu input: '{input}'");
                    currentErrorMessage = "Invalid input. Please try again.";
                    break;
            }
        }
    }
}