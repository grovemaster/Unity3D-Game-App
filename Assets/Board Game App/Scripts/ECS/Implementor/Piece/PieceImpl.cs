using Data.Constants.Board;
using Data.Enum;
using ECS.Component.Piece;
using ECS.Component.Piece.Move;
using ECS.Component.SharedComponent;
using Svelto.ECS;
using UnityEngine;

namespace ECS.Implementor.Piece
{
    class PieceImpl : MonoBehaviour, IImplementor, IPiece, IHighlight, IMovePiece, ILocation
    {
        public PieceType PieceType { get; set; }

        public Direction Direction { get; set; }

        public DispatchOnSet<bool> IsHighlighted { get; set; }

        public DispatchOnSet<HighlightState> CurrentColor { get; set; }

        public Vector3 Location { get; set; }

        public Vector3 NewLocation
        {
            set
            {
                SetNewLocation(value);
            }
        }

        void Awake()
        {
            PieceType = PieceType.PAWN;
            Direction = Direction.UP;
            IsHighlighted = new DispatchOnSet<bool>(gameObject.GetInstanceID());
            IsHighlighted.value = false;
            CurrentColor = new DispatchOnSet<HighlightState>(gameObject.GetInstanceID());
            Location = new Vector3(0, 0, 1);

            CurrentColor.NotifyOnValueSet(ChangeColor);
        }

        void OnMouseDown()
        {
            Debug.Log("Pawn OnMouseDown");
            IsHighlighted.value = !IsHighlighted.value;
        }

        private void ChangeColor(int id, HighlightState state)
        {
            Debug.Log("Changing color of Pawn");
            var sprite = GetComponentInChildren<SpriteRenderer>();
            if (state == HighlightState.DEFAULT)
            {
                sprite.color = Color.red;
            }
            else
            {
                sprite.color = Color.blue;
            }
        }

        private void SetNewLocation(Vector3 newLocation)
        {
            gameObject.transform.position = new Vector3(
                BoardConst.TOP_LEFT_CORNER.x + BoardConst.TILE_SIZE * newLocation.x,
                BoardConst.TOP_LEFT_CORNER.y + BoardConst.TILE_SIZE * newLocation.y,
                0);
        }
    }
}
