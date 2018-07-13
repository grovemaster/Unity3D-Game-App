using UnityEngine;

namespace ECS.Component.Piece.Move
{
    public interface IMovePiece : IComponent
    {
        Vector3 NewLocation { set; }
    }
}
