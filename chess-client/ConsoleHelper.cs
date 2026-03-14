namespace chess_client;

/// <summary>
/// Provides helper constants and methods for the console interface.
/// </summary>
public static class ConsoleHelper
{
    /// <summary>
    /// Reads a line from the console in a truly cancellable way.
    /// Unlike <c>Task.Run(() => Console.ReadLine())</c>, this method stops reading
    /// when the token is cancelled — no orphaned ReadLine call is left behind that
    /// would consume the next user input.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The line entered by the user, or null if cancelled.</returns>
    public static async Task<string?> ReadLineAsync(CancellationToken ct)
    {
        var buffer = new System.Text.StringBuilder();
        while (!ct.IsCancellationRequested)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept: false);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return buffer.ToString();
                }

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (buffer.Length > 0)
                    {
                        buffer.Remove(buffer.Length - 1, 1);
                        // Erase the character on screen (ReadKey already moved cursor back)
                        Console.Write(' ');
                        Console.Write('\b');
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    buffer.Append(key.KeyChar);
                }
            }
            else
            {
                await Task.Delay(50, ct).ConfigureAwait(false);
            }
        }

        ct.ThrowIfCancellationRequested();
        return null;
    }

    // Die erste leere Zeile beim @-String weglassen, damit es im Terminal nicht 
    // ungewollt nach unten rutscht.

    public const string GameMenu =
        """
        ┌────────────────────┐
        │      Play (P)      │
        ├────────────────────┤
        │    Friends (F)     │
        ├────────────────────┤
        │     Games (G)      │
        ├────────────────────┤
        │      Quit (Q)      │
        └────────────────────┘
        """;

    public const string FriendsMenu =
        """
        ┌────────────────────┐
        │     Search (S)     │
        ├────────────────────┤
        │      List (L)      │
        ├────────────────────┤
        │      Quit (Q)      │
        └────────────────────┘
        """;
    
    /// <summary>
    /// A string representing the replay menu text
    /// </summary>
    public const string ReplayMenu = 
        """
        ┌────────────────────┐
        │     Replays (R)    │
        ├────────────────────┤
        │      Quit (Q)      │
        └────────────────────┘
        """;


    public const string LoginMenu =
        """
          Welcome to ChessLI!
        ┌────────────────────┐
        │      Login (L)     │
        ├────────────────────┤
        │    Register (R)    │
        ├────────────────────┤
        │      Quit (Q)      │
        └────────────────────┘
        """;
}