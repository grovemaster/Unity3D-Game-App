using System;
using Data.Enum;
using ECS.Component.Piece;
using UnityEngine;
using View.Piece;

namespace ECS.Implementor.Piece
{
    class PieceImpl : MonoBehaviour, IImplementor, IPieceComponent
    {
        private PieceViewService pieceViewService = new PieceViewService();

        private PieceType pieceType;
        public PieceType PieceType
        {
            get
            {
                return pieceType;
            }
            set
            {
                pieceType = value;
                ChangePieceUpSideText();
            }
        }

        public Direction Direction { get; set; }

        private void ChangePieceUpSideText()
        {
            pieceViewService.ChangePieceUpSideText(gameObject, PieceType);
        }
    }
}
