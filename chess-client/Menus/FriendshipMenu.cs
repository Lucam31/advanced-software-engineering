using chess_client.Services;
using chess_client.States;
using Shared.Logger;
using Shared.WebSocketMessages;
using chess_client.UserInterface;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace chess_client.Menus;

public enum FriendshipMenuResult
{
    Back,
    Quit
}

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

    public async Task<FriendshipMenuResult> DisplayMenu()
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

            var input = _ui.ReadKey();
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

    private async Task<FriendshipMenuResult> SearchView()
    {
        string? currentErrorMessage = null;

        while (true)
        {
            GameLogger.Info("Displaying SearchView in friendship menu.");

            _ui.DrawSearchPrompt(currentErrorMessage);
            currentErrorMessage = null;

            var input = _ui.ReadInput();
            GameLogger.Debug($"User entered search input: '{input}'");

            if (input?.ToUpper() == "Q" || input?.ToUpper() == "QUIT") return FriendshipMenuResult.Quit;
            if (input?.ToUpper() == "B" || input?.ToUpper() == "BACK" || string.IsNullOrEmpty(input))
                return FriendshipMenuResult.Back;

            var users = await friendshipServices.SearchUsers(input);
            string? searchResultError = null;

            while (true)
            {
                _ui.DrawSearchResults(users, searchResultError);
                searchResultError = null;

                var num = _ui.ReadInput();
                GameLogger.Debug($"User entered list selection: '{num}'");

                if (num?.ToUpper() == "Q" || num?.ToUpper() == "QUIT") return FriendshipMenuResult.Quit;
                if (num?.ToUpper() == "B" || num?.ToUpper() == "BACK" || string.IsNullOrEmpty(num))
                    break; // Bricht die Schleife ab, geht zurück zur Such-Eingabe

                if (int.TryParse(num, out var index) && index > 0 && index <= users.Count)
                {
                    var selectedUser = users[index - 1];
                    GameLogger.Info($"User selected to add friend: '{selectedUser}'");

                    await friendshipServices.SendFriendRequest(userContainer.Id, selectedUser);
                    _ui.ShowMessageAndWait($"Friend request sent to {selectedUser}.");
                    return FriendshipMenuResult.Back; // Nach erfolgreicher Anfrage zurück ins Friends-Hauptmenü
                }
                else
                {
                    GameLogger.Warning($"Invalid input for selecting user: '{num}'");
                    searchResultError = "Invalid input. Please try again.";
                }
            }
        }
    }

    private async Task<FriendshipMenuResult> ListView(FriendshipMenuState state)
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

            if (input == "Q" || input == "QUIT") return FriendshipMenuResult.Quit;
            if (input == "B" || input == "BACK") return FriendshipMenuResult.Back;

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

                    using (var cts = new CancellationTokenSource())
                    {
                        StartGamePayload? pendingStartGame = null;
                        state.OnStartGame += payload =>
                        {
                            GameLogger.Info("Game Start as color " + payload.Color);
                            pendingStartGame = payload;
                            cts.Cancel();
                        };

                        _ui.ShowMessage("Waiting for opponent to accept... Press B to cancel.");

                        try
                        {
                            var cancelInput = (await _ui.ReadInputAsync(cts.Token))?.ToUpper();
                            if (cancelInput == "Q") return FriendshipMenuResult.Quit;
                            if (cancelInput == "B" || cancelInput == "BACK")
                            {
                                GameLogger.Info("User cancelled game invitation.");
                                // TODO: Send cancel message to server
                                _ui.ShowMessageAndWait("Game invitation cancelled.");
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
                                return FriendshipMenuResult.Back; // Nach dem Spiel zurück ins Friends-Menü
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