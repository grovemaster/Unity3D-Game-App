using ECS.Component.Modal;
using Svelto.ECS;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using View.Piece;

namespace View.Modal
{
    class TowerModalViewService
    {
        private readonly UnityAction closePanel;
        private readonly PieceViewService pieceViewService = new PieceViewService();

        public TowerModalViewService(UnityAction closePanel)
        {
            this.closePanel = closePanel;
        }

        public void DeactivateButton(Button pieceTierButton)
        {
            pieceTierButton.onClick.RemoveAllListeners();
            pieceTierButton.gameObject.SetActive(false);
        }

        internal void SetupButtonHandlers(Button pieceTierButton, ITowerOptionBase config, DispatchOnSet<int> answer, bool addClosePanel)
        {
            DeactivateButton(pieceTierButton); // Reset button first

            pieceTierButton.onClick.AddListener(delegate ()
            {
                // Trigger confirmation that clicked item was referenced piece id (engine will pick it up)
                answer.value = config.ReferencedPieceId;
            });

            if (addClosePanel)
            {
                pieceTierButton.onClick.AddListener(closePanel);
            }

            ActivateButton(pieceTierButton, config.Enabled);
        }

        internal void SetupButtonAppearance(GameObject pieceTier, ITowerOptionBase config)
        {
            pieceViewService.ChangeIcon(pieceTier, config.PieceType, config.Back);
            pieceViewService.ChangePlayerBorder(pieceTier, config.Team);
        }

        internal void SetPieceOpacity(GameObject pieceObject, bool enabled)
        {
            if (enabled)
            {
                SetPieceEnabled(pieceObject);
            }
            else
            {
                SetPieceDisabled(pieceObject);
            }
        }

        internal void SetPieceHidden(GameObject pieceObject)
        {
            SetPieceOpacity(pieceObject, 0);
        }

        internal void SetPieceEnabled(GameObject pieceObject)
        {
            SetPieceOpacity(pieceObject, 1f);
        }

        private void SetPieceDisabled(GameObject pieceObject)
        {
            SetPieceOpacity(pieceObject, 0.5f);
        }

        private void SetPieceOpacity(GameObject pieceObject, float alpha)
        {
            SpriteRenderer[] spriteRenderers = pieceObject.transform.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < spriteRenderers.Length; ++i)
            {
                spriteRenderers[i].color = new Color(1f, 1f, 1f, alpha);
            }
        }

        private void ActivateButton(Button button, bool enabled)
        {
            button.gameObject.SetActive(enabled);
            button.interactable = enabled; // Button is activated by clicking piece icon, not button itself; that will prevent "double clicks"
        }
    }
}
