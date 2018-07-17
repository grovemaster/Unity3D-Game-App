using System.Collections.Generic;
using Data.Enum;
using ECS.Component.Board.Tile;
using ECS.Component.SharedComponent;
using Svelto.ECS;
using UnityEngine;

namespace ECS.Implementor.Board.Tile
{
    class TileImpl : MonoBehaviour, IImplementor, ITile, IHighlight, ILocation, IChangeColorComponent
    {
        public DispatchOnSet<bool> IsPressed { get; set; }

        public bool IsHighlighted { get; set; }

        public Vector3 Location { get; set; }

        public int? PieceRefEntityId { get; set; }

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
            // Location is set in Context
        }

        void OnMouseDown()
        {
            Debug.Log("Tile OnMouseDown " + Location.ToString());
            IsPressed.value = true;
        }

        private void ChangeColor()
        {
            Debug.Log("Changing color of Tile " + Location.ToString());
            var sprite = GetComponentInChildren<SpriteRenderer>();

            bool hasBlackClick = CurrentColorStates.Contains(HighlightState.PLAYER_BLACK_CLICK_HIGHLIGHT);
            bool hasWhiteClick = CurrentColorStates.Contains(HighlightState.PLAYER_WHITE_CLICK_HIGHLIGHT);

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
            else // HighlightState.DEFAULT or nothing
            {
                sprite.color = Color.gray;
            }
        }
    }
}
