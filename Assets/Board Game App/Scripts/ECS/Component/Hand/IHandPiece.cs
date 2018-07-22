using Data.Enum;
using Svelto.ECS;

namespace ECS.Component.Hand
{
    public interface IHandPiece : IComponent
    {
        PieceType PieceType { get; set; }
        DispatchOnSet<int> NumPieces { get; set; }
    }
}
