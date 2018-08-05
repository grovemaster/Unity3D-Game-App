using UnityEngine;

namespace ECS.Component.Piece.Move
{
    public interface IMovePieceComponent : IComponent
    {
        // TODO Later this will be related to an animation component
        Vector2 NewLocation { set; }
    }
}
