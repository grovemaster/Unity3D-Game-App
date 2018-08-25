using Data.Enum;

namespace ECS.Component.Piece
{
    public interface IPieceComponent : IComponent
    {
        PieceType PieceType { get; set; }
        PieceType Front { get; set; }
        PieceType Back { get; set; }
        Direction Direction { get; set; }
    }
}
