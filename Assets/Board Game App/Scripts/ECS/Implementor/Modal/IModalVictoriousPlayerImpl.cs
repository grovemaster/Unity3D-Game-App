using Data.Enums.Player;
using ECS.Component.Player;
using UnityEngine;

namespace ECS.Implementor.Modal
{
    class IModalVictoriousPlayerImpl : MonoBehaviour, IImplementor, IPlayerComponent
    {
        public PlayerColor PlayerColor { get; set; }
    }
}
