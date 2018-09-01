using Data.Enums.Piece;
using Data.Enums.Player;
using ECS.EntityView.Piece;
using UnityEngine;

namespace Data.Check.PreviousMove
{
    public struct PreviousPieceState
    {
        public PieceEV Piece;
        public PlayerColor PlayerColor;
        public PieceType PieceType;
        public Vector2 Location;
        public int Tier;
        public bool TopOfTower;
    }
}
