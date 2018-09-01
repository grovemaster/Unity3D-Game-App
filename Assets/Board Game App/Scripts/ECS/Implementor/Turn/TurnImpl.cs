using Data.Enums.Player;
using ECS.Component.Player;
using UnityEngine;

namespace ECS.Implementor.Turn
{
    class TurnImpl : MonoBehaviour, IImplementor, IPlayerComponent, ICheckComponent, IForcedRearrangementComponent
    {
        public PlayerColor PlayerColor { get; set; }

        public bool CommanderInCheck { get; set; }
        public bool ForcedRearrangmentActive { get; set; }
    }
}
