using UnityEngine;

namespace ECS.Component.Piece.Move
{
    public interface IMovePiece : IComponent
    {
        // TODO Later this will be related to an animation component
        Vector3 NewLocation { set; }
    }
}
