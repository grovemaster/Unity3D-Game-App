using System;
using Data.Enums.Piece;
using Data.Enums.Player;
using ECS.Component.Hand;
using ECS.Component.Player;
using Svelto.ECS;
using UnityEngine;
using View.Hand;

namespace ECS.Implementor.Hand
{
    class HandPieceImpl : MonoBehaviour, IImplementor, IHandPieceComponent, IPlayerComponent
    {
        private HandViewService handViewService = new HandViewService();

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
                ChangeFrontPieceText();
                ChangePieceIcon();
            }
        }

        private PieceType back;

        public PieceType Back
        {
            get
            {
                return back;
            }

            set
            {
                back = value;
                ChangeBackPieceText();
            }
        }

        public DispatchOnSet<int> NumPieces { get; set; }
        public PlayerColor PlayerColor { get; set; }

        void Awake()
        {
            // PieceType is set in Context
            NumPieces = new DispatchOnSet<int>(gameObject.GetInstanceID());
            NumPieces.value = 0;

            NumPieces.NotifyOnValueSet(ChangeNumPieces);
        }

        private void ChangeNumPieces(int id, int numPieceValue)
        {
            GameObject numPieceTextComponent = gameObject.transform.Find("Num Piece Text").gameObject;
            TextMesh numPieceTextMesh = numPieceTextComponent.GetComponent<TextMesh>();
            numPieceTextMesh.text = numPieceValue.ToString();
        }

        private void ChangeFrontPieceText()
        {
            handViewService.ChangePieceUpSideText(gameObject, PieceType);
        }

        private void ChangeBackPieceText()
        {
            handViewService.ChangePieceDownSideText(gameObject, Back);
        }

        private void ChangePieceIcon()
        {
            handViewService.ChangePieceIcon(gameObject, PieceType, Back);
        }
    }
}
