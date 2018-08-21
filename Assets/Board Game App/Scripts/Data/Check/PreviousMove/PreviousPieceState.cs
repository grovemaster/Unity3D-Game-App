using ECS.EntityView.Piece;
using UnityEngine;

namespace Data.Check.PreviousMove
{
    public struct PreviousPieceState
    {
        public PieceEV Piece;
        public Vector2 Location;
        public int Tier;
        public bool TopOfTower;
    }
}
