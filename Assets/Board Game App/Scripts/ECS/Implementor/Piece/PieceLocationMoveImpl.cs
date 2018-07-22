using Data.Constants.Board;
using ECS.Component.Piece.Move;
using ECS.Component.SharedComponent;
using UnityEngine;

namespace ECS.Implementor.Piece
{
    class PieceLocationMoveImpl : MonoBehaviour, IImplementor, IMovePieceComponent, ILocationComponent
    {
        public Vector3 Location { get; set; }

        public Vector3 NewLocation
        {
            set
            {
                SetNewLocation(value);
            }
        }

        void Awake()
        {
            // Location is set in Context
        }

        private void SetNewLocation(Vector3 newLocation)
        {
            gameObject.transform.position = new Vector3(
                BoardConst.TOP_LEFT_CORNER.x + BoardConst.TILE_SIZE * newLocation.x,
                BoardConst.TOP_LEFT_CORNER.y + BoardConst.TILE_SIZE * newLocation.y,
                0);
        }
    }
}
