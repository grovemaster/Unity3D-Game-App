using System.Collections.Generic;
using Data.Enum;
using ECS.Component.Board.Tile;
using ECS.Component.SharedComponent;
using UnityEngine;

namespace ECS.Implementor.Board.Tile
{
    class TileImpl : MonoBehaviour, IImplementor, ITile, ILocation
    {
        public Vector3 Location { get; set; }

        public int? PieceRefEntityId { get; set; }

        void Awake()
        {
            // Location is set in Context
        }
    }
}
