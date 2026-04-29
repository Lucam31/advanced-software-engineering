using chess_client.Logic;
using chess_client.Services;
using chess_client.States;
using Shared.Logger;
using Shared.WebSocketMessages;
using chess_client.UserInterface;

namespace chess_client.Menus;

/// <summary>
/// Represents the possible outcomes when leaving the friendship menu.
/// </summary>
public enum FriendshipMenuResult
{
    /// <summary>
    /// User returned to the previous menu.
    /// </summary>
    Back,

    /// <summary>
    /// User requested to quit the client.
    /// </summary>
    Quit
}

/// <summary>
/// Coordinates friendship features such as searching users, listing friends, and creating friend games.
/// </summary>
/// <param name="userContainer">Shared user state that contains the active user id.</param>
/// <param name="friendshipServices">Service for searching users and managing friendships.</param>
/// <param name="gameService">Service used to create games against friends.</param>
/// <param name="webSocketService">WebSocket connection used for realtime friendship and game events.</param>
public class FriendshipMenu(
    UserContainer userContainer,
    FriendshipServices friendshipServices,
    IGameService gameService,
    WebSocketService webSocketService)
{
    private readonly FriendshipMenuUi _ui = new();
    
    /// <summary>
    /// Displays the friendship menu and routes user actions to search or list flows.
    /// </summary>
    /// <returns>
    /// <see cref="FriendshipMenuResult.Back"/> when the user navigates back,
    /// or <see cref="FriendshipMenuResult.Quit"/> when the user exits the client.
    /// </returns>
    public async Task<FriendshipMenuResult> DisplayMenu()
    {
        var state = new FriendshipMenuState();
        state.OnFriendsRefreshRequested += () =>
        {
            GameLogger.Info("Friends-Refresh angefordert (via WebSocket).");
        };
        webSocketService.TransitionTo(state);

        string? currentErrorMessage = null;

        while (true)
        {
            GameLogger.Info("Displaying friendship menu.");

            FriendshipMenuUi.DrawMainMenu(currentErrorMessage);
            currentErrorMessage = null;

            var input = BaseMenuUi.ReadKey();
            GameLogger.Debug($"User pressed key: '{input.Key}'");

            switch (input.Key)
            {
                case ConsoleKey.S:
                    GameLogger.Info("User selected 'Search'.");
                    var searchResult = await SearchView();
                    if (searchResult == FriendshipMenuResult.Quit) return FriendshipMenuResult.Quit;
                    break;

                case ConsoleKey.L:
                    GameLogger.Info("User selected 'List'.");
                    var listResult = await ListView(state);
                    if (listResult == FriendshipMenuResult.Quit) return FriendshipMenuResult.Quit;
                    break;

                case ConsoleKey.B:
                case ConsoleKey.Escape:
                    GameLogger.Info("User selected 'Back'.");
                    return FriendshipMenuResult.Back;

                case ConsoleKey.Q:
                    GameLogger.Info("User selected 'Quit'.");
                    return FriendshipMenuResult.Quit;

                default:
                    GameLogger.Warning($"Invalid menu input: '{input.Key}'");
                    currentErrorMessage = "Invalid input. Please press S, L, B, or Q.";
                    break;
            }
        }
    }

    /// <summary>
    /// Displays the user search flow and optionally sends a friend request.
    /// </summary>
    /// <returns>
    /// <see cref="FriendshipMenuResult.Back"/> when the flow finishes or the user goes back,
    /// or <see cref="FriendshipMenuResult.Quit"/> when the user exits the client.
    /// </returns>
    private async Task<FriendshipMenuResult> SearchView()
    {
        string? currentErrorMessage = null;

        while (true)
        {
            GameLogger.Info("Displaying SearchView in friendship menu.");

            FriendshipMenuUi.DrawSearchPrompt(currentErrorMessage);
            currentErrorMessage = null;

            var input = BaseMenuUi.ReadInput();
            GameLogger.Debug($"User entered search input: '{input}'");

            if (input?.ToUpper() == "Q" || input?.ToUpper() == "QUIT") return FriendshipMenuResult.Quit;
            if (input?.ToUpper() == "B" || input?.ToUpper() == "BACK" || string.IsNullOrEmpty(input))
                return FriendshipMenuResult.Back;

            var users = await friendshipServices.SearchUsers(input);
            string? searchResultError = null;

            while (true)
            {
                FriendshipMenuUi.DrawSearchResults(users, searchResultError);
                searchResultError = null;

                var num = BaseMenuUi.ReadInput();
                GameLogger.Debug($"User entered list selection: '{num}'");

                if (num?.ToUpper() == "Q" || num?.ToUpper() == "QUIT") return FriendshipMenuResult.Quit;
                if (num?.ToUpper() == "B" || num?.ToUpper() == "BACK" || string.IsNullOrEmpty(num))
                    break;

                if (int.TryParse(num, out var index) && index > 0 && index <= users.Count)
                {
                    var selectedUser = users[index - 1];
                    GameLogger.Info($"User selected to add friend: '{selectedUser}'");

                    await friendshipServices.SendFriendRequest(userContainer.Id, selectedUser);
                    BaseMenuUi.ShowMessageAndWait($"Friend request sent to {selectedUser}.");
                    return FriendshipMenuResult.Back;
                }
                else
                {
                    GameLogger.Warning($"Invalid input for selecting user: '{num}'");
                    searchResultError = "Invalid input. Please try again.";
                }
            }
        }
    }

    /// <summary>
    /// Displays the friend list flow and handles delete/play actions for selected friends.
    /// </summary>
    /// <param name="state">Active friendship menu state used to receive realtime start-game events.</param>
    /// <returns>
    /// <see cref="FriendshipMenuResult.Back"/> when the user returns to the previous menu,
    /// or <see cref="FriendshipMenuResult.Quit"/> when the user exits the client.
    /// </returns>
    private async Task<FriendshipMenuResult> ListView(FriendshipMenuState state)
    {
        var friends = await friendshipServices.ListFriends(userContainer.Id);
        string? currentErrorMessage = null;

        while (true)
        {
            GameLogger.Info("Displaying ListView in friendship menu.");

            var friendNames = friends.Select(f => (string)f.Name).ToList();

            FriendshipMenuUi.DrawListView(friendNames, currentErrorMessage);
            currentErrorMessage = null;

            var input = BaseMenuUi.ReadInput()?.ToUpper();

            if (string.IsNullOrEmpty(input))
            {
                GameLogger.Info("Reloading friends list.");
                friends = await friendshipServices.ListFriends(userContainer.Id);
                continue;
            }

            if (input is "Q" or "QUIT") return FriendshipMenuResult.Quit;
            if (input is "B" or "BACK") return FriendshipMenuResult.Back;

            if (input.Length < 2)
            {
                currentErrorMessage = "Invalid input. Example: 1D or 2P";
                continue;
            }

            var action = input[^1];
            var numberStr = input[..^1];

            if (!int.TryParse(numberStr, out var index) || index < 1 || index > friends.Count)
            {
                currentErrorMessage = $"Invalid number. Choose between 1 and {friends.Count}.";
                continue;
            }

            var selected = friends[index - 1];

            switch (action)
            {
                case 'D':
                    GameLogger.Info($"Removing friend '{selected.Name}'.");
                    await friendshipServices.RemoveFriend(selected);
                    BaseMenuUi.ShowMessageAndWait($"Removed {selected.Name}.");
                    friends = await friendshipServices.ListFriends(userContainer.Id);
                    break;
                case 'P':
                    GameLogger.Info($"Create Game with player '{selected.Name}'.");
                    await gameService.CreateGame(selected.UserId);

                    using (var cts = new CancellationTokenSource())
                    {
                        StartGamePayload? pendingStartGame = null;
                        state.OnStartGame += payload =>
                        {
                            GameLogger.Info("Game Start as color " + payload.Color);
                            pendingStartGame = payload;
                            cts.Cancel();
                        };

                        BaseMenuUi.ShowMessage("Waiting for opponent to accept... Press B to cancel.");

                        try
                        {
                            var cancelInput = (await BaseMenuUi.ReadInputAsync(cts.Token))?.ToUpper();
                            if (cancelInput == "Q") return FriendshipMenuResult.Quit;
                            if (cancelInput == "B" || cancelInput == "BACK")
                            {
                                GameLogger.Info("User cancelled game invitation.");
                                // TODO: Send cancel message to server
                                BaseMenuUi.ShowMessageAndWait("Game invitation cancelled.");
                                break;
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            if (pendingStartGame != null)
                            {
                                GameLogger.Info("Starting game with ID " + pendingStartGame.GameId);
                                var game = new GameLogic();
                                await game.StartGame(webSocketService, pendingStartGame);
                                return FriendshipMenuResult.Back;
                            }
                        }
                    }

                    break;
                default:
                    currentErrorMessage = "Unknown action. Use D to delete or P to play.";
                    break;
            }
        }
    }
}