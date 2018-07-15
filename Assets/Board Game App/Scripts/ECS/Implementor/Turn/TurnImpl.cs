﻿using Data.Enum.Player;
using ECS.Component.Player;
using UnityEngine;

namespace ECS.Implementor.Turn
{
    class TurnImpl : MonoBehaviour, IImplementor, IPlayerComponent
    {
        public PlayerColor PlayerColor { get; set; }
    }
}