using Data.Enum;
using Data.Enum.Player;
using ECS.Component.Hand;
using ECS.Component.Player;
using Svelto.ECS;
using System;
using UnityEngine;

namespace ECS.Implementor.Hand
{
    class HandPieceImpl : MonoBehaviour, IImplementor, IHandPieceComponent, IPlayerComponent
    {
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
            GameObject frontPieceTextComponent = gameObject.transform.Find("Front Piece Text").gameObject;
            TextMesh frontPieceTextMesh = frontPieceTextComponent.GetComponent<TextMesh>();

            switch(PieceType)
            {
                case PieceType.COMMANDER:
                    frontPieceTextMesh.text = "C";
                    break;
                case PieceType.PAWN:
                    frontPieceTextMesh.text = "P";
                    break;
                default:
                    throw new InvalidOperationException("Unsupported PieceType for HandPiece");
            }
        }
    }
}
