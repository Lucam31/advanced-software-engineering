using chess_client;
using Shared;
using Shared.Pieces;
using System.Timers;

Console.OutputEncoding = System.Text.Encoding.UTF8;

GameMenu gameMenu = new GameMenu();
GameLogic gameLogic = new GameLogic();
Gameboard gameBoard = new Gameboard();

bool start = gameMenu.DisplayMainMenu();

if (start)
{
    CLIOutput.PrintConsole("Starting a new game...");
    gameLogic.StartNewGame(gameBoard);
}

CLIOutput.PrintConsoleNewline("Closing Application...");