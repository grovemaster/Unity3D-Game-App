using Data.Constants.ColorConst;
using Data.Enum;
using ECS.Component.SharedComponent;
using Svelto.ECS;
using System.Collections.Generic;
using System.Linq;
using UI.Modal;
using UnityEngine;

namespace ECS.Implementor.Hand
{
    class HandPieceHighlightImpl : MonoBehaviour, IImplementor, IHighlightComponent, IChangeColorComponent
    {
        private TrackIsModalOpen isModalOpen;

        public DispatchOnSet<bool> IsPressed { get; set; }

        public bool IsHighlighted { get; set; }
        public HashSet<HighlightState> CurrentColorStates { get; set; }

        public bool PlayChangeColor
        {
            set
            {
                if (value)
                {
                    ChangeColor();
                }
            }
        }

        void Awake()
        {
            isModalOpen = FindObjectOfType<TrackIsModalOpen>();

            IsPressed = new DispatchOnSet<bool>(gameObject.GetInstanceID());
            IsHighlighted = false;
            CurrentColorStates = new HashSet<HighlightState>();
        }

        void OnMouseDown()
        {
            if (!isModalOpen.IsModalOpen)
            {
                IsPressed.value = true;
            }
        }

        private void ChangeColor()
        {
            var sprite = GetComponentInChildren<SpriteRenderer>();

            if (CurrentColorStates.Any())
            {
                sprite.color = Color.yellow;
            }
            else // HighlightState.DEFAULT or nothing
            {
                sprite.color = ColorConst.LightOrangeBrown;
            }
        }
    }
}
