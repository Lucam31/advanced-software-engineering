namespace Shared;

public readonly struct Move(string from, string to)
{
    public readonly string From = from;
    public readonly string To = to;
}