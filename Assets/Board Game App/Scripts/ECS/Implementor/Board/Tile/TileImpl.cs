﻿using System.Collections.Generic;
using System.Linq;
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
            if (CurrentColorStates.Any())
            {
                sprite.color = Color.yellow;
            }
            else // HighlightState.DEFAULT or nothing
            {
                sprite.color = Color.gray;
            }
        }
    }
}
