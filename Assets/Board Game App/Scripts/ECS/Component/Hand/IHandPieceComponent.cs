using Data.Enums.Piece;
using Svelto.ECS;

namespace ECS.Component.Hand
{
    public interface IHandPieceComponent : IComponent
    {
        PieceType Back { get; set; }
        PieceType PieceType { get; set; }
        DispatchOnSet<int> NumPieces { get; set; }
    }
}
