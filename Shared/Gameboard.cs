namespace Shared;

using Pieces;
using System;

public class Gameboard
{
    private readonly Tile[,] _tiles;

    private int Rows => _tiles.GetLength(0);
    private int Cols => _tiles.GetLength(1);

    private readonly List<Piece> _capturedBlackPieces = [];
    private readonly List<Piece> _capturedWhitePieces = [];

    public List<Piece> CapturedWhitePieces => [.._capturedWhitePieces];
    public List<Piece> CapturedBlackPieces => [.._capturedBlackPieces];


    public Gameboard()
    {
        _tiles = new Tile[8, 8];
        InitializeTiles();
    }

    private void InitializeTiles()
    {
        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Cols; c++)
            {
                var isWhiteTile = ((r + c) % 2) == 0;
                Piece? piece = null;

                var position = $"{(char)('a' + c)}{8 - r}";

                piece = r switch
                {
                    1 => new Pawn(position, false),
                    6 => new Pawn(position, true),
                    0 => c switch
                    {
                        0 or 7 => new Rook(position, false),
                        1 or 6 => new Knight(position, false),
                        2 or 5 => new Bishop(position, false),
                        3 => new Queen(position, false),
                        4 => new King(position, false),
                        _ => null
                    },
                    7 => c switch
                    {
                        0 or 7 => new Rook(position, true),
                        1 or 6 => new Knight(position, true),
                        2 or 5 => new Bishop(position, true),
                        3 => new Queen(position, true),
                        4 => new King(position, true),
                        _ => null
                    },
                    _ => piece
                };

                _tiles[r, c] = new Tile(r, c, isWhiteTile, piece);
            }
        }
    }

    public Tile this[int row, int col]
    {
        get => _tiles[row, col];
        set => _tiles[row, col] = value ?? throw new ArgumentNullException(nameof(value));
    }

    public void PrintBoard()
    {
        const int cellWidth = 3;
        var leftPad = new string(' ', cellWidth);

        Console.Write(leftPad);
        for (var file = 0; file < 8; file++)
        {
            var fileChar = (char)('A' + file);
            Console.Write($" {fileChar} ");
        }

        Console.WriteLine();

        for (var rank = 8; rank >= 1; rank--)
        {
            Console.Write($" {rank} ");

            for (var file = 0; file < 8; file++)
            {
                var isDark = (file + rank) % 2 == 0;

                Console.BackgroundColor = isDark ? ConsoleColor.Gray : ConsoleColor.DarkGray;


                var tile = _tiles[rank - 1, file];
                if (tile.CurrentPiece != null)
                {
                    Console.ForegroundColor = tile.CurrentPiece.IsWhite ? ConsoleColor.White : ConsoleColor.Black;
                    Console.Write($" {tile.CurrentPiece?.UnicodeSymbol} ");
                }
                else
                {
                    Console.Write("   ");
                }
            }

            Console.ResetColor();
            Console.WriteLine($" {rank}");
        }

        Console.Write(leftPad);
        for (var file = 0; file < 8; file++)
        {
            var fileChar = (char)('A' + file);
            Console.Write($" {fileChar} ");
        }
    }

    public Piece? GetPieceAtPosition(string position)
    {
        return _tiles[position[1] - '1', position[0] - 'A'].CurrentPiece;
    }

    public bool Move(Move move)
    {
        var fromIndexLetter = move.From[0] - 'A';
        var fromIndexNumber = move.From[1] - '1';
        var toIndexLetter = move.To[0] - 'A';
        var toIndexNumber = move.To[1] - '1';

        // If the tile is occupied, the piece must be captured
        if (_tiles[toIndexNumber, toIndexLetter].CurrentPiece != null)
        {
            _tiles[toIndexNumber, toIndexLetter].CurrentPiece!.IsCaptured = true;

            // Add the captured piece to the captured pieces array
            if (_tiles[toIndexNumber, toIndexLetter].CurrentPiece!.IsWhite)
            {
                _capturedWhitePieces.Add(_tiles[toIndexNumber, toIndexLetter].CurrentPiece!);
            }
            else
            {
                _capturedBlackPieces.Add(_tiles[toIndexNumber, toIndexLetter].CurrentPiece!);
            }
        }

        _tiles[toIndexNumber, toIndexLetter].CurrentPiece = _tiles[fromIndexNumber, fromIndexLetter].CurrentPiece;
        _tiles[toIndexNumber, toIndexLetter].CurrentPiece?.Move(move.To);
        _tiles[fromIndexNumber, fromIndexLetter].CurrentPiece = null;
        return true;
    }
}