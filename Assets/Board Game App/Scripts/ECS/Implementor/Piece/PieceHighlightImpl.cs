using System.Collections.Generic;
using System.Linq;
using Data.Enum;
using ECS.Component.SharedComponent;
using Svelto.ECS;
using UnityEngine;

namespace ECS.Implementor.Piece
{
    class PieceHighlightImpl : MonoBehaviour, IImplementor, IHighlight, IChangeColorComponent
    {
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
            IsPressed = new DispatchOnSet<bool>(gameObject.GetInstanceID());
            IsHighlighted = false;
            CurrentColorStates = new HashSet<HighlightState>();
        }

        void OnMouseDown()
        {
            Debug.Log("Pawn OnMouseDown");
            IsPressed.value = true;
        }

        private void ChangeColor()
        {
            Debug.Log("Changing color of Pawn");
            var sprite = GetComponentInChildren<SpriteRenderer>();
            if (CurrentColorStates.Any())
            {
                sprite.color = Color.blue;
            }
            else // HighlightState.DEFAULT or nothing
            {
                sprite.color = Color.red;
            }
        }
    }
}
