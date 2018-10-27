using Data.Constants.ColorConst;
using Data.Enums.Player;
using ECS.Component.InitialArrangement;
using ECS.Component.Player;
using UnityEngine;

namespace ECS.Implementor.Turn
{
    class TurnImpl : MonoBehaviour, IImplementor, IPlayerComponent, ICheckComponent, IForcedRearrangementComponent, IInitialArrangementComponent
    {
        private PlayerColor playerColor;
        public PlayerColor PlayerColor
        {
            get
            {
                return playerColor;
            }
            set
            {
                playerColor = value;
                SetTurnVisual(playerColor);
            }
        }

        private bool commanderInCheck;
        public bool CommanderInCheck
        {
            get
            {
                return commanderInCheck;
            }
            set
            {
                commanderInCheck = value;
                SetCheckVisual(commanderInCheck);
            }
        }

        public bool ForcedRearrangmentActive { get; set; }
        public bool IsInitialArrangementInEffect { get; set; }

        private void SetTurnVisual(PlayerColor turnPlayer)
        {
            SpriteRenderer blackTurnSprite = transform.Find("Black Player Turn").GetComponent<SpriteRenderer>();
            SpriteRenderer whiteTurnSprite = transform.Find("White Player Turn").GetComponent<SpriteRenderer>();
            blackTurnSprite.color = ColorConst.DullGreen;
            whiteTurnSprite.color = ColorConst.DullGreen;

            blackTurnSprite.enabled = turnPlayer == PlayerColor.BLACK;
            whiteTurnSprite.enabled = turnPlayer == PlayerColor.WHITE;
        }

        private void SetCheckVisual(bool isCommanderInCheck)
        {
            if (!isCommanderInCheck)
            {
                return;
            }

            string componentName = playerColor == PlayerColor.BLACK ? "Black Player Turn" : "White Player Turn";
            SpriteRenderer turnSprite = transform.Find(componentName).GetComponent<SpriteRenderer>();
            turnSprite.color = Color.red;
        }
    }
}
