using chess_client;
using Shared;
using Shared.Pieces;
using System.Timers;

Console.OutputEncoding = System.Text.Encoding.UTF8;

GameMenu gameMenu = new GameMenu();
Gameboard gameboard = new Gameboard();
GameLogic gameLogic = new GameLogic(gameboard);

bool start = gameMenu.DisplayMainMenu();

if (start)
{
    CLIOutput.PrintConsole("Starting a new game...");
    gameLogic.StartNewGame();
}

CLIOutput.PrintConsoleNewline("Closing Application...");