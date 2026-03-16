using chess_client.Services;
using chess_client.States;
using Shared.Logger;
using Shared.WebSocketMessages;
using chess_client.UserInterface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace chess_client.Menus;

public enum GameMenuResult
{
    Logout,
    Quit
}

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
    public async Task<GameMenuResult> DisplayMainMenu()
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

            ConsoleKeyInfo input;
            try
            {
                input = await _ui.ReadKeyAsync(cts.Token);
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

            GameLogger.Debug($"User pressed key: '{input.Key}'");

            switch (input.Key)
            {
                case ConsoleKey.P:
                    GameLogger.Info("User selected 'Play'.");
                    var matchmakingMenu = new MatchmakingMenu(gameService, webSocketService);

                    var startGamePayload = await matchmakingMenu.EnterQueueAsync(cts.Token, () => pendingStartGame);

                    if (startGamePayload != null)
                    {
                        GameLogger.Info("Starting game with ID " + startGamePayload.GameId);
                        var game = new GameLogic();
                        await game.StartGame(webSocketService, startGamePayload);
                    }

                    break;

                case ConsoleKey.F:
                    GameLogger.Info("User selected 'Friends'.");

                    var friendResult = await friendshipMenu.DisplayMenu();

                    if (friendResult == FriendshipMenuResult.Quit)
                    {
                        return GameMenuResult.Quit;
                    }

                    break;

                case ConsoleKey.G:
                    GameLogger.Info("User selected 'Games'.");
                    var replayMenu = new ReplayMenu(userContainer, webSocketService);
                    await replayMenu.DisplayMenu();
                    break;

                case ConsoleKey.L:
                    GameLogger.Info("User selected 'Logout'.");
                    return GameMenuResult.Logout;

                case ConsoleKey.Q:
                    GameLogger.Info("User selected 'Quit'.");
                    return GameMenuResult.Quit;

                default:
                    GameLogger.Warning($"Invalid menu input: '{input.Key}'");
                    currentErrorMessage = "Invalid input. Please press P, F, G, L, or Q.";
                    break;
            }
        }
    }
}