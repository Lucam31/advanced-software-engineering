using chess_client;
using Shared;

Console.OutputEncoding = System.Text.Encoding.UTF8;

GameMenu gameMenu = new GameMenu();
Gameboard gameboard = new Gameboard();
GameLogic gameLogic = new GameLogic(gameboard);
ReplayHandler replayHandler = new ReplayHandler(gameboard);

int start = gameMenu.DisplayMainMenu();

if (start == 1)
{
    CLIOutput.PrintConsole("Starting a new game...");
    gameLogic.StartNewGame();
}
else if (start == 2)
{
    CLIOutput.PrintConsole("Replaying a game...");
    replayHandler.StartReplay(["E2E4", "E7E5", "G1F3", "B8C6", "F1C4", "G8F6"]);
}

CLIOutput.PrintConsoleNewline("Closing Application...");