using Shared.Dtos;

namespace Shared.Pieces;

/// <summary>
/// Represents the abstract base class for all chess pieces.
/// </summary>
/// <param name="position">The initial position of the piece in algebraic notation.</param>
/// <param name="name">The name of the piece (e.g., "Pawn").</param>
/// <param name="isWhite">A value indicating whether the piece is white.</param>
/// <param name="isCaptured">A value indicating whether the piece is captured.</param>
public abstract class Piece(string position, string name, bool isWhite, bool isCaptured = false)
{
    /// <summary>
    /// Gets the name of the piece.
    /// </summary>
    public string Name { get; private set; } = name;
    
    /// <summary>
    /// Gets the current position of the piece on the board.
    /// </summary>
    public string Position { get; private set; } = position;
    
    /// <summary>
    /// Gets a value indicating whether the piece is white.
    /// </summary>
    public bool IsWhite { get; private set; } = isWhite;

    /// <summary>
    /// Gets or sets a value indicating whether the piece has been captured.
    /// </summary>
    public bool IsCaptured { get; set; } = isCaptured;
    
    /// <summary>
    /// Gets a value indicating whether the piece has moved from its starting position.
    /// </summary>
    public bool Moved { get; private set; } = false;

    /// <summary>
    /// Gets the Unicode symbol representing the piece.
    /// </summary>
    public abstract string UnicodeSymbol { get; }

    /// <summary>
    /// Returns the Unicode symbol of the piece.
    /// </summary>
    /// <returns>The Unicode symbol as a string.</returns>
    public override string ToString() => UnicodeSymbol;

    /// <summary>
    /// Updates the piece's position and marks it as moved.
    /// </summary>
    /// <param name="newPosition">The new position for the piece.</param>
    public void Move(string newPosition)
    {
        Position = newPosition;
        Moved = true;
    }
    
    /// <summary>
    /// Create a Data Transfer Object (DTO) representation of the Piece.
    /// </summary>
    /// <returns>Returns PieceDto</returns>
    public PieceDto ToDto()
    {
        return new PieceDto
        {
            Position = Position,
            IsWhite = IsWhite,
            Type = Name,
            UnicodeSymbol = UnicodeSymbol,
            IsCaptured = IsCaptured,
            Moved = Moved
        };
    }
    
    /// <summary>
    /// Creates a Piece instance from a PieceDto.
    /// </summary>
    /// <param name="dto">The Data Transfer Object (DTO) representation of the Piece.</param>
    /// <returns>An instance of Piece</returns>
    /// <exception cref="ArgumentException">Unknown Piece Type</exception>
    public static Piece FromDto(PieceDto dto)
    {
        Piece piece = dto.Type switch
        {
            "Pawn" => new Pawn(dto.Position, dto.IsWhite),
            "Rook" => new Rook(dto.Position, dto.IsWhite),
            "Knight" => new Knight(dto.Position, dto.IsWhite),
            "Bishop" => new Bishop(dto.Position, dto.IsWhite),
            "Queen" => new Queen(dto.Position, dto.IsWhite),
            "King" => new King(dto.Position, dto.IsWhite),
            _ => throw new ArgumentException($"Unknown piece type: {dto.Type}")
        };
    
        piece.IsCaptured = dto.IsCaptured;
    
        if (dto.Moved && piece.Position != dto.Position)
        {
            piece.Move(dto.Position);
        }
    
        return piece;
    }
}