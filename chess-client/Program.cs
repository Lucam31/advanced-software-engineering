using Shared;
using Shared.Pieces;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var bishopWhite = new Bishop("A3", true);

Gameboard gameboard = new Gameboard();
gameboard.PrintBoard();
//
// Bishop bishopBlack = new Bishop("A3", false);
//

// bishopWhite.IsCaptured = true;
// if (!bishopWhite.Move("B4"))
// {
//     Console.WriteLine("Invalid Move");
// }
// else
// {
//     Console.WriteLine(bishopWhite.Name + " moved to " + bishopWhite.Position);
// }
//
// if (!bishopWhite.Move("A8"))
// {
//     Console.WriteLine("Invalid Move");
// }
// else
// {
//     Console.WriteLine(bishopWhite.Name + " moved to " + bishopWhite.Position);
// }
//
//
// Console.WriteLine("Hello World!");