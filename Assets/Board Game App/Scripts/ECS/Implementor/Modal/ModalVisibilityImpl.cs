using ECS.Component.Modal;
using ECS.Component.Visibility;
using Svelto.ECS;
using UnityEngine;

namespace ECS.Implementor.Modal
{
    class ModalVisibilityImpl : MonoBehaviour, IImplementor, ICancelComponent, IVisibilityComponent
    {
        public DispatchOnSet<bool> Cancel { get; set; }
        public DispatchOnSet<bool> IsVisible { get; set; }

        void Awake()
        {
            Cancel = new DispatchOnSet<bool>(gameObject.GetInstanceID())
            {
                value = false
            };

            IsVisible = new DispatchOnSet<bool>(gameObject.GetInstanceID())
            {
                value = false
            };
        }
    }
}
