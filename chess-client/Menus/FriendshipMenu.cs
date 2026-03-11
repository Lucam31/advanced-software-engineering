using chess_client.Services;
using chess_client.States;
using Shared.Logger;
using Shared.WebSocketMessages;

namespace chess_client.Menus;

/// <summary>
/// Manages the friendship menu of the game.
/// </summary>
public class FriendshipMenu
{
    private readonly UserContainer _userContainer;
    private readonly FriendshipServices _friendshipServices;
    private readonly WebSocketService _webSocketService;
    private readonly IGameService _gameService;
    
    private volatile bool _refreshRequested = false;
    
    /// <summary>
    /// Initializes a new instance of the GameMenu class.
    /// </summary>
    /// <param name="userContainer">The user container.</param>
    /// <param name="friendshipServices">The friendship services.</param>
    /// <param name="webSocketService">The web socket service.</param>
    public FriendshipMenu(UserContainer userContainer, FriendshipServices friendshipServices, IGameService gameService, WebSocketService webSocketService)
    {
        _userContainer = userContainer;
        _friendshipServices = friendshipServices;
        _gameService = gameService;
        _webSocketService = webSocketService;
    }
    
    public async Task DisplayMenu()
    {
        
        var state = new FriendshipMenuState();
        state.OnFriendsRefreshRequested += () =>
        {
            GameLogger.Info("Friends-Refresh angefordert (via WebSocket).");
            _refreshRequested = true;
        };
        _webSocketService.TransitionTo(state);

        while (true)
        {

            GameLogger.Info("Displaying friendship menu.");

            CliOutput.PrintConsoleNewline(ConsoleHelper.FriendsMenu);
            CliOutput.PrintConsoleNewline("Please enter your choice: ");
            var input = Console.ReadLine()?.Trim().ToUpper();

            GameLogger.Debug($"User entered menu input: '{input}'");

            switch (input)
            {
                case "S":
                case "SEARCH":
                    GameLogger.Info("User selected 'Search'.");
                    await SearchView();
                    continue;
                case "L":
                case "LIST":
                    GameLogger.Info("User selected 'List'.");
                    await ListView(state);
                    continue;
                case "Q":
                case "QUIT":
                    GameLogger.Info("User selected 'Quit'.");
                    return;
                default:
                    GameLogger.Warning($"Invalid menu input: '{input}'");
                    CliOutput.PrintConsoleNewline("Invalid input. Please try again.");
                    continue;
            }
        }
    }
    
    /// <summary>
    /// Displays the list of friends and handles user input for the list view.
    /// </summary>
    private async Task SearchView()
    {
        while (true)
        {
            GameLogger.Info("Displaying SearchView in friendship menu.");

            CliOutput.PrintConsoleNewline("Please enter the username you want so search for (or 'Q' to quit): ");
            var input = Console.ReadLine()?.Trim();

            GameLogger.Debug($"User entered menu input: '{input}'");
            
            if (input == "Q" || input == "QUIT" || input == null)
            {
                GameLogger.Info("User selected 'Quit' in SearchView.");
                return;
            }
            
            // search for users with the given username and display results
            var users = await _friendshipServices.SearchUsers(input);
            while (true)
            {
                foreach (var user in users)
                {
                    CliOutput.PrintConsoleNewline($"[1] {user}");
                }

                CliOutput.PrintConsoleNewline("Please enter the number of the username you want so add to your friendlist (or 'Q' to quit): ");
                var num = Console.ReadLine()?.Trim();

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
                    await _friendshipServices.SendFriendRequest(_userContainer.Id, selectedUser);
                    CliOutput.PrintConsoleNewline($"Friend request sent to {selectedUser}.");
                    break;
                }
                else
                {
                    GameLogger.Warning($"Invalid input for selecting user: '{num}'");
                    CliOutput.PrintConsoleNewline("Invalid input. Please try again.");
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
        var friends = await _friendshipServices.ListFriends(_userContainer.Id);
        
        while (true)
        {
            GameLogger.Info("Displaying ListView in friendship menu.");

            Console.Clear();
            if (friends.Count == 0)
            {
                CliOutput.PrintConsoleNewline("You have no friends yet. Try adding some!");
            }
            else
            {
                for (var i = 0; i < friends.Count; i++)
                    CliOutput.PrintConsoleNewline($"[{i + 1}] {friends[i].Name}");
            }
            
            CliOutput.PrintConsoleNewline("Actions: <number>D = delete, <number>P = play");
            CliOutput.PrintConsoleNewline("Enter action, press 'Q' to go back, or just Enter to refresh friendlist.");

            var input = Console.ReadLine()?.Trim().ToUpper();

            if (string.IsNullOrEmpty(input))
            {
                _refreshRequested = false;
                GameLogger.Info("Reloading friends list.");
                friends = await _friendshipServices.ListFriends(_userContainer.Id);
                continue;
            }

            if (input == "Q" || input == "QUIT")
                return;

            // Format: <number><action>, z.B. "1D", "2P", "11D"
            if (input.Length < 2)
            {
                CliOutput.PrintConsoleNewline("Invalid input. Example: 1D or 2P");
                continue;
            }

            var action = input[^1];
            var numberStr = input[..^1];

            if (!int.TryParse(numberStr, out var index) || index < 1 || index > friends.Count)
            {
                CliOutput.PrintConsoleNewline($"Invalid number. Choose between 1 and {friends.Count}.");
                continue;
            }

            var selected = friends[index - 1];

            switch (action)
            {
                case 'D':
                    GameLogger.Info($"Removing friend '{selected.Name}'.");
                    await _friendshipServices.RemoveFriend(selected);
                    CliOutput.PrintConsoleNewline($"Removed {selected.Name}.");
                    friends = await _friendshipServices.ListFriends(_userContainer.Id);
                    break;
                case 'P':
                    GameLogger.Info($"Create Game with player '{selected.Name}'.");
                    await _gameService.CreateGame(selected.UserId);
                    
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

                        CliOutput.PrintConsoleNewline("Waiting for opponent to accept... Press Q to cancel.");

                        try
                        {
                            var cancelInput = (await ConsoleHelper.ReadLineAsync(cts.Token))?.Trim().ToUpper();
                            if (cancelInput == "Q")
                            {
                                GameLogger.Info("User cancelled game invitation.");
                                // TODO: Send cancel message to server in the future
                                CliOutput.PrintConsoleNewline("Game invitation cancelled.");
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
                                await game.StartGame(_webSocketService, pendingStartGame);
                                return;
                            }
                        }
                    }
                    break;
                default:
                    CliOutput.PrintConsoleNewline("Unknown action. Use D to delete or P to play.");
                    break;
            }
        }
    }
}