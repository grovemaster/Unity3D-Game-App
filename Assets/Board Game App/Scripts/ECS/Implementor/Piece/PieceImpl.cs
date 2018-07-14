using Data.Enum;
using ECS.Component.Piece;
using UnityEngine;

namespace ECS.Implementor.Piece
{
    class PieceImpl : MonoBehaviour, IImplementor, IPiece
    {
        public PieceType PieceType { get; set; }

        public Direction Direction { get; set; }

        void Awake()
        {
            PieceType = PieceType.PAWN;
        }
    }
}
