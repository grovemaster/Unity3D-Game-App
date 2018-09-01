using Data.Enums.Piece;
using Data.Enums.Piece.Side;
using ECS.Component.Modal;
using Svelto.ECS;
using UnityEngine;

namespace ECS.Implementor.Modal
{
    class ModalDropFrontBackImpl : MonoBehaviour, IImplementor, IDropFrontBackComponent
    {
        public int TileReferenceId { get; set; }
        public int HandPieceReferenceId { get; set; }
        public PieceType Front { get; set; }
        public PieceType Back { get; set; }
        public DispatchOnSet<PieceSide> Answer { get; set; }

        void Awake()
        {
            Answer = new DispatchOnSet<PieceSide>(gameObject.GetInstanceID());
        }
    }
}
