using Shared.Pieces;

Bishop bishop = new Bishop("A3", true, false);
bishop.IsCaptured = true;
if (!bishop.Move("B4"))
{
    Console.WriteLine("Invalid Move");
}
else
{
    Console.WriteLine(bishop.Name + " moved to " + bishop.Position);
}

if (!bishop.Move("A8"))
{
    Console.WriteLine("Invalid Move");
}
else
{
    Console.WriteLine(bishop.Name + " moved to " + bishop.Position);
}


Console.WriteLine("Hello World!");