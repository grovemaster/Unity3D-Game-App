using Data.Enum;

namespace ECS.Component.Piece
{
    public interface IPieceComponent : IComponent
    {
        PieceType PieceType { get; set; }
        Direction Direction { get; set; }
    }
}
