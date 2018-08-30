using Data.Enum.Piece;
using Data.Enum.Player;
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
