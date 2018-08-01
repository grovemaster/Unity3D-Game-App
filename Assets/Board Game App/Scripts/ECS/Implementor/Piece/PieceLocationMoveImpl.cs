using Data.Constants.Board;
using ECS.Component.Piece.Move;
using ECS.Component.Piece.Tower;
using ECS.Component.SharedComponent;
using UnityEngine;

namespace ECS.Implementor.Piece
{
    class PieceLocationMoveImpl : MonoBehaviour, IImplementor, IMovePieceComponent, ILocationComponent, ITowerTierComponent
    {
        public Vector3 Location { get; set; }

        public Vector3 NewLocation
        {
            set
            {
                SetNewLocation(value);
            }
        }

        public bool TopOfTower { get; set; }

        public int Tier { get; set; }

        void Awake()
        {
            // Location is set in Context
            TopOfTower = true;
            Tier = 1;
        }

        private void SetNewLocation(Vector3 newLocation)
        {
            gameObject.transform.position = new Vector3(
                BoardConst.TOP_LEFT_CORNER.x + BoardConst.TILE_SIZE * newLocation.x + (BoardConst.TOWER_OFFSET.x * (Tier - 1)),
                BoardConst.TOP_LEFT_CORNER.y + BoardConst.TILE_SIZE * newLocation.y + (BoardConst.TOWER_OFFSET.y * (Tier - 1)),
                BoardConst.TOWER_OFFSET.z * (Tier - 1));
        }
    }
}
