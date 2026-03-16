using chess_client.Services;
using chess_client.States;
using Shared.Logger;
using Shared.WebSocketMessages;
using chess_client.UserInterface;

namespace chess_client.Menus;

/// <summary>
/// Manages the friendship menu of the game.
/// </summary>
public class FriendshipMenu(
    UserContainer userContainer,
    FriendshipServices friendshipServices,
    IGameService gameService,
    WebSocketService webSocketService)
{
    private readonly FriendshipMenuUi _ui = new();

    private volatile bool _refreshRequested = false;

    public async Task DisplayMenu()
    {
        var state = new FriendshipMenuState();
        state.OnFriendsRefreshRequested += () =>
        {
            GameLogger.Info("Friends-Refresh angefordert (via WebSocket).");
            _refreshRequested = true;
        };
        webSocketService.TransitionTo(state);

        string? currentErrorMessage = null;

        while (true)
        {
            GameLogger.Info("Displaying friendship menu.");

            _ui.DrawMainMenu(currentErrorMessage);
            currentErrorMessage = null;

            var input = _ui.ReadInput()?.ToUpper();
            GameLogger.Debug($"User entered menu input: '{input}'");

            switch (input)
            {
                case "S":
                case "SEARCH":
                    GameLogger.Info("User selected 'Search'.");
                    await SearchView();
                    break;
                case "L":
                case "LIST":
                    GameLogger.Info("User selected 'List'.");
                    await ListView(state);
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

    /// <summary>
    /// Displays the list of friends and handles user input for the list view.
    /// </summary>
    private async Task SearchView()
    {
        string? currentErrorMessage = null;

        while (true)
        {
            GameLogger.Info("Displaying SearchView in friendship menu.");

            _ui.DrawSearchPrompt(currentErrorMessage);
            currentErrorMessage = null;

            var input = _ui.ReadInput();
            GameLogger.Debug($"User entered menu input: '{input}'");

            if (input == "Q" || input == "QUIT" || input == null)
            {
                GameLogger.Info("User selected 'Quit' in SearchView.");
                return;
            }

            // search for users with the given username and display results
            var users = await friendshipServices.SearchUsers(input);
            string? searchResultError = null;

            while (true)
            {
                _ui.DrawSearchResults(users, searchResultError);
                searchResultError = null;

                var num = _ui.ReadInput();
                GameLogger.Debug($"User entered menu input: '{num}'");

                if (num == "Q" || num == "QUIT" || num == null)
                {
                    GameLogger.Info("User selected 'Quit' in SearchView.");
                    return;
                }

                if (int.TryParse(num, out var index) && index > 0 && index <= users.Count)
                {
                    var selectedUser = users[index - 1];
                    GameLogger.Info($"User selected to add friend: '{selectedUser}'");

                    await friendshipServices.SendFriendRequest(userContainer.Id, selectedUser);
                    _ui.ShowMessageAndWait($"Friend request sent to {selectedUser}.");
                    break;
                }
                else
                {
                    GameLogger.Warning($"Invalid input for selecting user: '{num}'");
                    searchResultError = "Invalid input. Please try again.";
                }
            }

            return;
        }
    }

    /// <summary>
    /// Displays the list of friends and handles user input for the list view.
    /// </summary>
    private async Task ListView(FriendshipMenuState state)
    {
        var friends = await friendshipServices.ListFriends(userContainer.Id);
        string? currentErrorMessage = null;

        while (true)
        {
            GameLogger.Info("Displaying ListView in friendship menu.");

            var friendNames = friends.Select(f => (string)f.Name).ToList();

            _ui.DrawListView(friendNames, currentErrorMessage);
            currentErrorMessage = null;

            var input = _ui.ReadInput()?.ToUpper();

            if (string.IsNullOrEmpty(input))
            {
                _refreshRequested = false;
                GameLogger.Info("Reloading friends list.");
                friends = await friendshipServices.ListFriends(userContainer.Id);
                continue;
            }

            if (input == "Q" || input == "QUIT" || input == "q")
                return;

            // Format: <number><action>, z.B. "1D", "2P", "11D"
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
                    _ui.ShowMessageAndWait($"Removed {selected.Name}.");
                    friends = await friendshipServices.ListFriends(userContainer.Id);
                    break;
                case 'P':
                    GameLogger.Info($"Create Game with player '{selected.Name}'.");
                    await gameService.CreateGame(selected.UserId);

                    // Wait for the server to send StartGame, meanwhile let the user cancel with Q
                    using (var cts = new CancellationTokenSource())
                    {
                        StartGamePayload? pendingStartGame = null;
                        state.OnStartGame += payload =>
                        {
                            GameLogger.Info("Game Start as color " + payload.Color);
                            pendingStartGame = payload;
                            cts.Cancel();
                        };

                        _ui.ShowMessage("Waiting for opponent to accept... Press Q to cancel.");

                        try
                        {
                            var cancelInput = (await _ui.ReadInputAsync(cts.Token))?.ToUpper();
                            if (cancelInput == "Q")
                            {
                                GameLogger.Info("User cancelled game invitation.");
                                // TODO: Send cancel message to server in the future
                                _ui.ShowMessageAndWait("Game invitation cancelled.");
                                break;
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            // Server sent StartGame → start the game
                            if (pendingStartGame != null)
                            {
                                GameLogger.Info("Starting game with ID " + pendingStartGame.GameId);
                                var game = new GameLogic();
                                await game.StartGame(webSocketService, pendingStartGame);
                                return;
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