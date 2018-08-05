using ECS.Component.Board.Tile;
using ECS.Component.SharedComponent;
using UnityEngine;

namespace ECS.Implementor.Board.Tile
{
    class TileImpl : MonoBehaviour, IImplementor, ITileComponent, ILocationComponent
    {
        public Vector2 Location { get; set; }

        public int? PieceRefEntityId { get; set; }

        void Awake()
        {
            // Location is set in Context
        }
    }
}
