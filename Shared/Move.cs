namespace Shared;
using Shared.Pieces;

public readonly struct Move
{
    public readonly Piece Piece;
    public readonly string From;
    public readonly string To;
    public readonly Piece? CapturedPiece;

    public Move(Piece piece, string from, string to, Piece? capturedPiece)
    {
        Piece = piece;
        From = from;
        To = to;
        CapturedPiece = capturedPiece;
    }
}