using Data.Enum.Player;
using ECS.Component.Player;
using UnityEngine;

namespace ECS.Implementor.Piece
{
    class PieceOwnerImpl : MonoBehaviour, IImplementor, IPlayerComponent
    {
        public PlayerColor PlayerColor { get; set; }
    }
}
