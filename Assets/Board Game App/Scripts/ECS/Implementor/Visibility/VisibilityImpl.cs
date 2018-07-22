using ECS.Component.Visibility;
using Svelto.ECS;
using UnityEngine;

namespace ECS.Implementor.Visibility
{
    class VisibilityImpl : MonoBehaviour, IImplementor, IVisibility
    {
        public DispatchOnSet<bool> IsVisible { get; set; }

        void Awake()
        {
            IsVisible = new DispatchOnSet<bool>(gameObject.GetInstanceID());
            IsVisible.value = true;

            IsVisible.NotifyOnValueSet(ChangeVisibility);
        }

        private void ChangeVisibility(int id, bool visibilityValue)
        {
            gameObject.SetActive(visibilityValue);
        }
    }
}
