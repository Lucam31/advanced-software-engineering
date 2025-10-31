using Shared.Pieces;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Bishop bishop = new Bishop("A3", true);
Console.WriteLine(bishop);

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