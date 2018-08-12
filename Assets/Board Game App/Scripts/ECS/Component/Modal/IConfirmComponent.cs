using ECS.EntityView.Piece;
using Svelto.ECS;

namespace ECS.Component.Modal
{
    public interface IConfirmComponent : IComponent
    {
        PieceEV PieceMoved { get; set; }
        PieceEV? PieceCaptured { get; set; }
        DispatchOnSet<bool> Answer { get; set; }
    }
}
