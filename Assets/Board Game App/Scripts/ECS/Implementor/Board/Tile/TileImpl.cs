using Data.Enum;
using ECS.Component.Board.Tile;
using ECS.Component.SharedComponent;
using Svelto.ECS;
using UnityEngine;

namespace ECS.Implementor.Board.Tile
{
    class TileImpl : MonoBehaviour, IImplementor, ITile, IHighlight, ILocation
    {
        public DispatchOnSet<bool> IsPressed { get; set; }

        public bool IsHighlighted { get; set; }

        public DispatchOnSet<HighlightState> CurrentColor { get; set; }

        public Vector3 Location { get; set; }

        public int? PieceRefEntityId { get; set; }

        void Awake()
        {
            IsPressed = new DispatchOnSet<bool>(gameObject.GetInstanceID());
            IsHighlighted = false;
            CurrentColor = new DispatchOnSet<HighlightState>(gameObject.GetInstanceID());
            CurrentColor.value = HighlightState.DEFAULT;
            //Location = new Vector3(0, 0, 1);

            CurrentColor.NotifyOnValueSet(ChangeColor);
        }

        void OnMouseDown()
        {
            Debug.Log("Tile OnMouseDown " + Location.ToString());
            IsPressed.value = true;
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
