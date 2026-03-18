using chess_client.Services;
using chess_client.States;
using Shared.Logger;
using Shared.WebSocketMessages;
using chess_client.UserInterface;

namespace chess_client.Menus;

/// <summary>
/// Represents the possible outcomes when leaving the dashboard menu.
/// </summary>
public enum GameMenuResult
{
    /// <summary>
    /// User requested to log out and return to the startup flow.
    /// </summary>
    Logout,

    /// <summary>
    /// User requested to quit the client.
    /// </summary>
    Quit
}

/// <summary>
/// Coordinates the dashboard menu, including navigation, WebSocket-driven events, and game starts.
/// </summary>
/// <param name="userContainer">Shared user state used by nested menus.</param>
/// <param name="friendshipMenu">Friendship menu used when the user opens friend features.</param>
/// <param name="gameService">Service for creating, accepting, and managing games.</param>
/// <param name="webSocketService">WebSocket connection used for realtime invitations and game start events.</param>
public class GameMenu(
    UserContainer userContainer,
    FriendshipMenu friendshipMenu,
    IGameService gameService,
    WebSocketService webSocketService)
{
    private readonly GameMenuUi _ui = new();

    /// <summary>
    /// Displays the dashboard menu and processes user input until logout or quit.
    /// </summary>
    /// <returns>
    /// <see cref="GameMenuResult.Logout"/> when the user logs out,
    /// or <see cref="GameMenuResult.Quit"/> when the user exits the client.
    /// </returns>
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

            GameMenuUi.DrawMainMenu(currentErrorMessage);
            currentErrorMessage = null;

            ConsoleKeyInfo input;
            try
            {
                input = await BaseMenuUi.ReadKeyAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                if (pendingInvitation != null)
                {
                    GameLogger.Info("Received game invitation.");
                    BaseMenuUi.ShowMessage("Received game invitation. Accepting...");
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

                    var startGamePayload = await matchmakingMenu.EnterQueueAsync(() => pendingStartGame, cts.Token);

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