using Data.Enum;
using ECS.Component.Board.Tile;
using ECS.Component.SharedComponent;
using Svelto.ECS;
using UnityEngine;

namespace ECS.Implementor.Board.Tile
{
    class TileImpl : MonoBehaviour, IImplementor, ITile, IHighlight, ILocation
    {
        public DispatchOnSet<bool> IsHighlighted { get; set; }

        public DispatchOnSet<HighlightState> CurrentColor { get; set; }

        public Vector3 Location { get; set; }

        public int? PieceRefEntityId { get; set; }

        void Awake()
        {
            IsHighlighted = new DispatchOnSet<bool>(gameObject.GetInstanceID());
            IsHighlighted.value = false;
            CurrentColor = new DispatchOnSet<HighlightState>(gameObject.GetInstanceID());
            CurrentColor.value = HighlightState.DEFAULT;
            //Location = new Vector3(0, 0, 1);

            CurrentColor.NotifyOnValueSet(ChangeColor);
        }

        void OnMouseDown()
        {
            Debug.Log("Tile OnMouseDown " + Location.ToString());
            IsHighlighted.value = !IsHighlighted.value;
        }

        private void ChangeColor(int id, HighlightState state)
        {
            Debug.Log("Changing color of Tile " + Location.ToString());
            var sprite = GetComponentInChildren<SpriteRenderer>();
            if (state == HighlightState.DEFAULT)
            {
                sprite.color = Color.gray;
            }
            else
            {
                sprite.color = Color.yellow;
            }
        }
    }
}
