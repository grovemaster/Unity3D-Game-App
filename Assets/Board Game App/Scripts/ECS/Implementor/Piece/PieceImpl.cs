﻿using Data.Enums;
using Data.Enums.Piece;
using ECS.Component.Piece;
using UnityEngine;
using View.Piece;

namespace ECS.Implementor.Piece
{
    class PieceImpl : MonoBehaviour, IImplementor, IPieceComponent
    {
        private PieceViewService pieceViewService = new PieceViewService();

        private PieceHighlightOwnerImpl pieceHighlightOwnerImpl;

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

        public PieceType Front { get; set; }
        public PieceType Back { get; set; }
        public Direction Direction { get; set; }

        private void ChangePieceUpSideText()
        {
            if (pieceHighlightOwnerImpl == null)
            {
                pieceHighlightOwnerImpl = gameObject.GetComponentInChildren<PieceHighlightOwnerImpl>();
            }
            pieceViewService.ChangePieceUpSideText(gameObject, PieceType);
            pieceViewService.ChangePieceDownSideText(gameObject, PieceType == Front ? Back : Front);

            pieceViewService.ChangeIcon(gameObject, pieceHighlightOwnerImpl.PlayerColor, PieceType, Back);
        }
    }
}
