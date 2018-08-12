using ECS.Component.Modal;
using ECS.EntityView.Piece;
using Svelto.ECS;
using UnityEngine;

namespace ECS.Implementor.Modal
{
    class ModalConfirmImpl : MonoBehaviour, IImplementor, IConfirmComponent
    {
        public PieceEV PieceMoved { get; set; }
        public PieceEV? PieceCaptured { get; set; }
        public DispatchOnSet<bool> Answer { get; set; }

        void Awake()
        {
            Answer = new DispatchOnSet<bool>(gameObject.GetInstanceID());
        }
    }
}
