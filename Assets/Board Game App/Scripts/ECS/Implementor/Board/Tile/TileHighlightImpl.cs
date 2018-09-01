using System.Collections.Generic;
using Data.Constants.ColorConst;
using Data.Enums;
using ECS.Component.SharedComponent;
using Svelto.ECS;
using UI.Modal;
using UnityEngine;

namespace ECS.Implementor.Board.Tile
{
    class TileHighlightImpl : MonoBehaviour, IImplementor, IHighlightComponent, IChangeColorComponent
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
                sprite.color = ColorConst.LightBlue;
            }
            else if (hasBlackRange)
            {
                sprite.color =  ColorConst.LightGreen;
            }
            else if (hasWhiteRange)
            {
                sprite.color =  ColorConst.LightPink;
            }
            else // HighlightState.DEFAULT or nothing
            {
                sprite.color = Color.gray;
            }
        }
    }
}
