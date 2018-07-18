using System.Collections.Generic;
using Data.Enum;
using ECS.Component.SharedComponent;
using Svelto.ECS;
using UnityEngine;

namespace ECS.Implementor.Board.Tile
{
    class TileHighlightImpl : MonoBehaviour, IImplementor, IHighlight, IChangeColorComponent
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
            IsPressed.value = true;
        }

        private void ChangeColor()
        {
            var sprite = GetComponentInChildren<SpriteRenderer>();

            bool hasBlackClick = CurrentColorStates.Contains(HighlightState.PLAYER_BLACK_CLICK_HIGHLIGHT);
            bool hasWhiteClick = CurrentColorStates.Contains(HighlightState.PLAYER_WHITE_CLICK_HIGHLIGHT);

            bool hasBlackRange = CurrentColorStates.Contains(HighlightState.PLAYER_BLACK_RANGE_HIGHLIGHT);
            bool hasWhiteRange = CurrentColorStates.Contains(HighlightState.PLAYER_WHITE_RANGE_HIGHLIGHT);

            if (hasBlackClick && hasWhiteClick)
            {
                sprite.color = Color.yellow;
            }
            else if (hasBlackClick)
            {
                sprite.color = Color.magenta;
            }
            else if (hasWhiteClick)
            {
                sprite.color = Color.red;
            }
            else if (hasBlackRange && hasWhiteRange)
            {
                sprite.color = new Color(0.169f, 0.694f, 0.824f);
            }
            else if (hasBlackRange)
            {
                sprite.color =  new Color(0.6f, 1f, 0.6f);
            }
            else if (hasWhiteRange)
            {
                sprite.color =  new Color(0.8f, 0.6f, 1f);
            }
            else // HighlightState.DEFAULT or nothing
            {
                sprite.color = Color.gray;
            }
        }
    }
}
