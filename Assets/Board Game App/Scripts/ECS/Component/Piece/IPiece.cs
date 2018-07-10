using Data.Enum;

namespace ECS.Component.Piece
{
    public interface IPiece : IComponent
    {
        PieceType PieceType { get; set; }
        Direction Direction { get; set; }
    }
}
