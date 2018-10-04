using System.Collections.Generic;
using Data.Constants.ColorConst;
using Data.Enums;
using Data.Enums.Player;
using ECS.Component.Player;
using ECS.Component.SharedComponent;
using Svelto.ECS;
using UI.Modal;
using UnityEngine;
using View.Piece;

namespace ECS.Implementor.Piece
{
    class PieceHighlightOwnerImpl : MonoBehaviour, IImplementor, IHighlightComponent, IPlayerComponent, IChangeColorComponent
    {
        private PieceViewService pieceViewService = new PieceViewService();

        private TrackIsModalOpen isModalOpen;

        public DispatchOnSet<bool> IsPressed { get; set; }
        public bool IsHighlighted { get; set; }
        public HashSet<HighlightState> CurrentColorStates { get; set; }
        public PlayerColor PlayerColor { get; set; }

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
            var sprite = transform.Find("Background").GetComponentInChildren<SpriteRenderer>();

            bool hasBlackClick = CurrentColorStates.Contains(HighlightState.PLAYER_BLACK_CLICK_HIGHLIGHT);
            bool hasWhiteClick = CurrentColorStates.Contains(HighlightState.PLAYER_WHITE_CLICK_HIGHLIGHT);

            if (hasBlackClick && hasWhiteClick)
            {
                sprite.color = Color.cyan;
            }
            else if (hasBlackClick)
            {
                sprite.color = Color.blue;
            }
            else if (hasWhiteClick)
            {
                sprite.color = Color.green;
            }
            else // HighlightState.DEFAULT or nothing
            {
                sprite.color = PlayerColor == PlayerColor.BLACK ? ColorConst.LightGray : ColorConst.VeryLightRed;
                pieceViewService.ChangePlayerBorder(gameObject, PlayerColor);
            }
        }
    }
}
