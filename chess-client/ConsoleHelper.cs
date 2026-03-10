namespace chess_client;

/// <summary>
/// Provides helper constants and methods for the console interface.
/// </summary>
public static class ConsoleHelper
{
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