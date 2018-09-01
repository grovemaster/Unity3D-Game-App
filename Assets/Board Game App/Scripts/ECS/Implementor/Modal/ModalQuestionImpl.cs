using Data.Enums.Modal;
using ECS.Component.Modal;
using Svelto.ECS;
using UnityEngine;

namespace ECS.Implementor.Modal
{
    class ModalQuestionImpl : MonoBehaviour, IImplementor, ICaptureOrStackComponent
    {
        public DispatchOnSet<ModalQuestionAnswer> Answer { get; set; }
        public int TileReferenceId { get; set; }

        void Awake()
        {
            Answer = new DispatchOnSet<ModalQuestionAnswer>(gameObject.GetInstanceID());
        }
    }
}
