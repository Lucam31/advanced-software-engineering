namespace chess_client.UserInterface;

/// <summary>
/// Handles the visual representation and console interactions for the friendship menu.
/// </summary>
public class FriendshipMenuUi : BaseMenuUi
{
    private const string PromptChoice = "   Your choice: ";
    private const string InputPrompt = "   > ";
    private const string ErrorPrefix = "   ⚠ ";
    private const string SearchUsersHeader = "   === SEARCH USERS ===";
    private const string SearchResultsHeader = "   === SEARCH RESULTS ===";
    private const string FriendListHeader = "   === FRIEND LIST ===";
    private const string NoUsersFoundMessage = "   No users found.";
    private const string NoFriendsMessage = "   You have no friends yet. Try adding some!";
    private const string SearchInstruction = "   Please enter the username you want to search for.";
    private const string BackOrQuitInstruction = "   (Type 'B' to go back, 'Q' to quit)";
    private const string SearchResultActionInstruction = "   Enter the number to add to your friendlist.";
    private const string FriendListActionsInstruction = "   Actions: <number>D = delete, <number>P = play";

    private const string FriendListPromptInstruction =
        "   (Type 'B' to go back, 'Q' to quit, or press Enter to refresh)";

    private const string MainMenuLayout =
        """
                   === FRIENDS ===

           ┌──────────────────────────────┐
           │          FRIEND MENU         │
           ├──────────────────────────────┤
           │  [S] Search Users            │
           │  [L] Friend List             │
           │  [B] Back to Dashboard       │
           │  [Q] Quit Game               │
           └──────────────────────────────┘
        """;

    /// <summary>
    /// Draws the friendship main menu.
    /// </summary>
    /// <param name="errorMessage">Optional error message shown above the input prompt.</param>
    public static void DrawMainMenu(string? errorMessage = null)
    {
        ConsoleHelper.ClearTerminal();

        ConsoleHelper.PrintConsoleNewline(MainMenuLayout);
        ConsoleHelper.WriteEmptyLine();

        DrawOptionalError(errorMessage);

        ConsoleHelper.PrintConsoleNewline(PromptChoice);
    }


    /// <summary>
    /// Draws the user search prompt in the friendship section.
    /// </summary>
    /// <param name="errorMessage">Optional error message shown above the input prompt.</param>
    public static void DrawSearchPrompt(string? errorMessage = null)
    {
        ConsoleHelper.ClearTerminal();
        DrawSectionHeader(SearchUsersHeader);
        DrawOptionalError(errorMessage);

        ConsoleHelper.PrintConsoleNewline(SearchInstruction);
        ConsoleHelper.PrintConsoleNewline(BackOrQuitInstruction);
        DrawInputPrompt();
    }

    /// <summary>
    /// Displays search results and prompts for selecting a user to add as a friend.
    /// </summary>
    /// <param name="users">Usernames returned by the current search.</param>
    /// <param name="errorMessage">Optional error message shown above the input prompt.</param>
    public static void DrawSearchResults(List<string> users, string? errorMessage = null)
    {
        ConsoleHelper.ClearTerminal();
        DrawSectionHeader(SearchResultsHeader);

        if (users.Count == 0)
        {
            ConsoleHelper.PrintConsoleNewline(NoUsersFoundMessage);
        }
        else
        {
            DrawIndexedList(users);
        }

        ConsoleHelper.WriteEmptyLine();
        DrawOptionalError(errorMessage);

        ConsoleHelper.PrintConsoleNewline(SearchResultActionInstruction);
        ConsoleHelper.PrintConsoleNewline(BackOrQuitInstruction);
        DrawInputPrompt();
    }

    /// <summary>
    /// Displays the friend list and available actions for each entry.
    /// </summary>
    /// <param name="friendNames">Names of friends to display.</param>
    /// <param name="errorMessage">Optional error message shown above the input prompt.</param>
    public static void DrawListView(List<string> friendNames, string? errorMessage = null)
    {
        ConsoleHelper.ClearTerminal();
        DrawSectionHeader(FriendListHeader);

        if (friendNames.Count == 0)
        {
            ConsoleHelper.PrintConsoleNewline(NoFriendsMessage);
        }
        else
        {
            DrawIndexedList(friendNames);
        }

        ConsoleHelper.WriteEmptyLine();
        DrawOptionalError(errorMessage);

        ConsoleHelper.PrintConsoleNewline(FriendListActionsInstruction);
        ConsoleHelper.PrintConsoleNewline(FriendListPromptInstruction);
        DrawInputPrompt();
    }

    /// <summary>
    /// Draws a section header with a title.
    /// </summary>
    /// <param name="title">The text to display as the section header.</param>
    private static void DrawSectionHeader(string title)
    {
        ConsoleHelper.WriteEmptyLine();
        ConsoleHelper.PrintConsoleNewline(title);
        ConsoleHelper.WriteEmptyLine();
    }

    /// <summary>
    /// Draws an optional error message if provided.
    /// </summary>
    /// <param name="errorMessage">The error message to display, or null/empty to skip.</param>
    private static void DrawOptionalError(string? errorMessage)
    {
        if (string.IsNullOrEmpty(errorMessage))
        {
            return;
        }

        ConsoleHelper.SetForegroundColor(ConsoleColor.Red);
        ConsoleHelper.PrintConsoleNewline($"{ErrorPrefix}{errorMessage}");
        ConsoleHelper.ResetColor();
        ConsoleHelper.WriteEmptyLine();
    }

    /// <summary>
    /// Draws a numbered list of entries.
    /// </summary>
    /// <param name="entries">The list of strings to display as indexed entries.</param>
    private static void DrawIndexedList(IReadOnlyList<string> entries)
    {
        for (var i = 0; i < entries.Count; i++)
        {
            ConsoleHelper.PrintConsoleNewline($"   [{i + 1}] {entries[i]}");
        }
    }

    /// <summary>
    /// Draws the input prompt for user input.
    /// </summary>
    private static void DrawInputPrompt() => ConsoleHelper.PrintConsole(InputPrompt);
}