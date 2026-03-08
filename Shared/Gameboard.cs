using Shared.Dtos;

namespace Shared;

using Pieces;
using System;

/// <summary>
/// Represents the chessboard, containing the grid of tiles and managing piece positions.
/// </summary>
public class Gameboard
{
    private readonly Tile[,] _tiles;

    /// <summary>
    /// Gets the number of rows on the game board.
    /// </summary>
    private int Rows => _tiles.GetLength(0);

    /// <summary>
    /// Gets the number of columns on the game board.
    /// </summary>
    private int Cols => _tiles.GetLength(1);

    private readonly List<Piece> _capturedBlackPieces = [];
    private readonly List<Piece> _capturedWhitePieces = [];
    private readonly List<Move> _moves = [];

    /// <summary>
    /// Gets a list of all captured white pieces.
    /// </summary>
    public List<Piece> CapturedWhitePieces => [.._capturedWhitePieces];

    /// <summary>
    /// Gets a list of all captured black pieces.
    /// </summary>
    public List<Piece> CapturedBlackPieces => [.._capturedBlackPieces];


    /// <summary>
    /// Initializes a new instance of the <see cref="Gameboard"/> class, setting up the initial state of the board.
    /// </summary>
    public Gameboard()
    {
        _tiles = new Tile[8, 8];
        InitializeTiles();
    }

    /// <summary>
    /// Sets up the initial placement of all pieces on the board.
    /// </summary>
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
                    1 => new Pawn(position, true),
                    6 => new Pawn(position, false),
                    0 => c switch
                    {
                        0 or 7 => new Rook(position, true),
                        1 or 6 => new Knight(position, true),
                        2 or 5 => new Bishop(position, true),
                        3 => new Queen(position, true),
                        4 => new King(position, true),
                        _ => null
                    },
                    7 => c switch
                    {
                        0 or 7 => new Rook(position, false),
                        1 or 6 => new Knight(position, false),
                        2 or 5 => new Bishop(position, false),
                        3 => new Queen(position, false),
                        4 => new King(position, false),
                        _ => null
                    },
                    _ => piece
                };

                _tiles[r, c] = new Tile(r, c, isWhiteTile, piece);
            }
        }
    }

    /// <summary>
    /// Gets or sets the tile at the specified row and column.
    /// </summary>
    /// <param name="row">The zero-based row index.</param>
    /// <param name="col">The zero-based column index.</param>
    /// <returns>The <see cref="Tile"/> at the specified position.</returns>
    public Tile this[int row, int col]
    {
        get => _tiles[row, col];
        set => _tiles[row, col] = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Prints the current state of the board to the console, including pieces and coordinates
    /// </summary>
    /// <param name="whitePlayer">If true, prints the board from white's perspective (reversed orientation)</param>
    public void PrintBoard(bool whitePlayer = true)
    {
        const int cellWidth = 3;
        var leftPad = new string(' ', cellWidth);

        Console.Write(leftPad);
        for (var i = 0; i < 8; i++)
        {
            var fileChar = whitePlayer ? (char)('A' + i) : (char)('H' - i);
            Console.Write($" {fileChar} ");
        }

        Console.WriteLine();

        for (var i = 0; i < 8; i++)
        {
            var rank = whitePlayer ? 8 - i : i + 1;
            Console.Write($" {rank} ");

            for (var j = 0; j < 8; j++)
            {
                var file = whitePlayer ? j : 7 - j;
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
        for (var i = 0; i < 8; i++)
        {
            var fileChar = whitePlayer ? (char)('A' + i) : (char)('H' - i);
            Console.Write($" {fileChar} ");
        }
    }

    /// <summary>
    /// Retrieves the piece at a given algebraic notation position.
    /// </summary>
    /// <param name="position">The algebraic notation of the tile (e.g., "A1", "H8").</param>
    /// <returns>The <see cref="Piece"/> at the specified position, or null if the tile is empty.</returns>
    public Piece? GetPieceAtPosition(string position)
    {
        return _tiles[position[1] - '1', position[0] - 'A'].CurrentPiece;
    }

    /// <summary>
    /// Executes a move on the board, updating piece positions and handling captures
    /// </summary>
    /// <param name="move">The move to be executed</param>
    /// <param name="enPassant">The move was an en passant</param>
    /// <param name="castling">The move was a castling</param>
    /// <returns>True if the move was successfully executed</returns>
    public bool Move(Move move, bool enPassant = false, bool castling = false)
    {
        var fromCol = move.From[0] - 'A';
        var fromRow = move.From[1] - '1';
        var toCol = move.To[0] - 'A';
        var toRow = move.To[1] - '1';

        if (enPassant)
        {
            // The captured pawn is on the destination column but on the starting row
            var capturedPawn = _tiles[fromRow, toCol].CurrentPiece;
            if (capturedPawn != null)
            {
                capturedPawn.IsCaptured = true;
                if (capturedPawn.IsWhite)
                    _capturedWhitePieces.Add(capturedPawn);
                else
                    _capturedBlackPieces.Add(capturedPawn);

                _tiles[fromRow, toCol].CurrentPiece = null;
            }

            _moves.Add(new Move(move.From, move.To, capturedPawn, $"{(char)('A' + toCol)}{fromRow + 1}"));
        }
        else
        {
            _moves.Add(new Move(move.From, move.To, _tiles[toRow, toCol].CurrentPiece,
                $"{(char)('A' + toCol)}{toRow + 1}"));

            // If the destination tile is occupied, capture the piece
            if (_tiles[toRow, toCol].CurrentPiece != null)
            {
                _tiles[toRow, toCol].CurrentPiece!.IsCaptured = true;

                if (_tiles[toRow, toCol].CurrentPiece!.IsWhite)
                    _capturedWhitePieces.Add(_tiles[toRow, toCol].CurrentPiece!);
                else
                    _capturedBlackPieces.Add(_tiles[toRow, toCol].CurrentPiece!);
            }
        }

        // Move the piece to the destination
        _tiles[toRow, toCol].CurrentPiece = _tiles[fromRow, fromCol].CurrentPiece;
        _tiles[toRow, toCol].CurrentPiece?.Move(move.To);
        _tiles[fromRow, fromCol].CurrentPiece = null;

        if (castling)
        {
            // Determine rook positions based on castling direction
            var row = fromRow; // King and rook are on the same row
            if (toCol > fromCol)
            {
                // Kingside castling: rook moves from H to F
                const int rookFromCol = 7; // H
                const int rookToCol = 5; // F
                _tiles[row, rookToCol].CurrentPiece = _tiles[row, rookFromCol].CurrentPiece;
                _tiles[row, rookToCol].CurrentPiece?.Move($"{(char)('A' + rookToCol)}{row + 1}");
                _tiles[row, rookFromCol].CurrentPiece = null;
            }
            else
            {
                // Queenside castling: rook moves from A to D
                const int rookFromCol = 0; // A
                const int rookToCol = 3; // D
                _tiles[row, rookToCol].CurrentPiece = _tiles[row, rookFromCol].CurrentPiece;
                _tiles[row, rookToCol].CurrentPiece?.Move($"{(char)('A' + rookToCol)}{row + 1}");
                _tiles[row, rookFromCol].CurrentPiece = null;
            }
        }

        return true;
    }

    public void UndoLastMove()
    {
        if (_moves.Count == 0) return;
    }

    /// <summary>
    /// Create a Data Transfer Object (DTO) representation of the Gameboard.
    /// </summary>
    /// <returns>Returns GameboardDto</returns>
    public GameboardDto ToDto()
    {
        var dto = new GameboardDto();

        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Cols; c++)
            {
                var tile = _tiles[r, c];
                dto.Tiles[r, c] = tile.ToDto();
            }
        }

        dto.CapturedWhitePieces = _capturedWhitePieces.Select(p => p.ToDto()).ToList();
        dto.CapturedBlackPieces = _capturedBlackPieces.Select(p => p.ToDto()).ToList();

        return dto;
    }

    /// <summary>
    /// Creates a Gameboard instance from a Data Transfer Object (DTO).
    /// </summary>
    /// <param name="dto">The Data Transfer Object (DTO) representation of the Gameboard.</param>
    /// <returns>An instance of Gameboard</returns>
    public static Gameboard FromDto(GameboardDto dto)
    {
        var gameboard = new Gameboard();

        for (var r = 0; r < 8; r++)
        {
            for (var c = 0; c < 8; c++)
            {
                var tileDto = dto.Tiles[r, c];
                var piece = tileDto.CurrentPiece != null ? Piece.FromDto(tileDto.CurrentPiece) : null;
                gameboard._tiles[r, c] = new Tile(tileDto.Row, tileDto.Col, tileDto.IsWhite, piece);
            }
        }

        gameboard._capturedWhitePieces.AddRange(dto.CapturedWhitePieces.Select(Piece.FromDto));
        gameboard._capturedBlackPieces.AddRange(dto.CapturedBlackPieces.Select(Piece.FromDto));

        return gameboard;
    }
}