using Data.Enums.Modal;
using ECS.Component.Modal;
using Svelto.ECS;
using System;
using UI.Modal;
using UnityEngine;
using UnityEngine.UI;
using View.Modal;

namespace ECS.Implementor.Modal
{
    // TODO Need track is modal open, b/c tile highlight impl
    class TowerModalImpl : MonoBehaviour, IImplementor, IModalTypeComponent, IAnswerComponent, IImmobileCaptureStateComponent
    {
        public GameObject Overlay;                // Need overlay to hide/show modal, since it contains modal
        private TrackIsModalOpen isModalOpen;

        // Modal elements
        public GameObject PieceTier1;
        public GameObject PieceTier2;
        public GameObject PieceTier3;

        // Modal Button Elements
        public Button PieceButtonTier1;   // The first button
        public Button PieceButtonTier2;   // The second button
        public Button PieceButtonTier3;   // The third button
        public Button CancelButton; // The cancel modal button

        // Other Impls contained here so values may be used in logic
        // Value set through Unity3D Editor
        public Tier1OptionImpl tier1Option;
        public Tier2OptionImpl tier2Option;
        public Tier3OptionImpl tier3Option;
        public ModalVisibilityImpl visibility;

        // Services
        private TowerModalViewService towerModalViewService;

        public ModalType Type { get; set; }
        public DispatchOnSet<int> Answer { get; set; }
        public bool ImmobileCaptureDesignated { get; set; }

        void Awake()
        {
            isModalOpen = FindObjectOfType<TrackIsModalOpen>();

            towerModalViewService = new TowerModalViewService(ClosePanel);
            Answer = new DispatchOnSet<int>();
        }

        void Start()
        {
            visibility.IsVisible.NotifyOnValueSet(OnVisibilityChanged);
            visibility.Cancel.NotifyOnValueSet(CancelModal);

            CancelButton.onClick.AddListener(CancelModal);

            ClosePanel();
        }

        private void OnVisibilityChanged(int id, bool isVisible)
        {
            if (isVisible)
            {
                OpenModal();
            }
            else
            {
                ClosePanel();
            }
        }

        private void CancelModal()
        {
            visibility.Cancel.value = true;
        }

        private void CancelModal(int id, bool cancel)
        {
            if (cancel)
            {
                visibility.IsVisible.value = false;
            }
        }

        private void OpenModal()
        {
            switch (Type)
            {
                case ModalType.TOWER_2ND_TIER:
                    towerModalViewService.DeactivateButton(PieceButtonTier3);
                    towerModalViewService.SetupButtonHandlers(PieceButtonTier2, tier2Option, Answer, true);
                    towerModalViewService.SetupButtonHandlers(PieceButtonTier1, tier1Option, Answer, ImmobileCaptureDesignated);

                    towerModalViewService.SetupButtonAppearance(PieceTier2, tier2Option);
                    towerModalViewService.SetupButtonAppearance(PieceTier1, tier1Option);

                    towerModalViewService.SetPieceHidden(PieceTier3);
                    towerModalViewService.SetPieceEnabled(PieceTier2);
                    towerModalViewService.SetPieceOpacity(PieceTier1, tier1Option.Enabled);
                    break;
                case ModalType.TOWER_3RD_TIER:
                    towerModalViewService.SetupButtonHandlers(PieceButtonTier3, tier3Option, Answer, true);
                    towerModalViewService.SetupButtonHandlers(PieceButtonTier2, tier2Option, Answer, ImmobileCaptureDesignated);
                    towerModalViewService.SetupButtonHandlers(PieceButtonTier1, tier1Option, Answer, ImmobileCaptureDesignated);

                    towerModalViewService.SetupButtonAppearance(PieceTier3, tier3Option);
                    towerModalViewService.SetupButtonAppearance(PieceTier2, tier2Option);
                    towerModalViewService.SetupButtonAppearance(PieceTier1, tier1Option);

                    towerModalViewService.SetPieceEnabled(PieceTier3);
                    towerModalViewService.SetPieceOpacity(PieceTier2, tier2Option.Enabled);
                    towerModalViewService.SetPieceOpacity(PieceTier1, tier1Option.Enabled);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported Tower ModalType");
            }

            ActivateModal();
            isModalOpen.IsModalOpen = true;
        }

        private void ActivateModal()
        {
            //Overlay.SetActive(true);  //Activate the Panel; its default is "off" in the Inspector
            // Place Overlay, with Modal Panel, in view of camera
            RectTransform overlayRect = Overlay.GetComponent<RectTransform>();
            overlayRect.offsetMin = new Vector2(0, overlayRect.offsetMin.y);
            overlayRect.offsetMax = new Vector2(0, overlayRect.offsetMax.y);
        }

        private void ClosePanelVisibility()
        {
            /**
             * Dealing with problem of asynchronous events and overusing a single modal.
             * Closing this modal and then opening this same modal of the same/different type has no guarantee of when
             * this function is run; it could be run after the 2nd opening.  This is a simple work-around, but it
             * doesn't handle the case of closing the confirm modal.  That is handled in the answer engine
             */
            /**if ((Type == ModalType.CONFIRM && previousModalType == ModalType.CONFIRM && previous2ndModalType == ModalType.CAPTURE_STACK)
                || (Type == ModalType.TOWER_3RD_TIER && previousModalType == ModalType.TOWER_3RD_TIER && previous2ndModalType == ModalType.TIER_1_3_EXCHANGE_CLICK))
            {
                previousModalType = null;
            }
            else
            {
                visibility.IsVisible.value = false;
            }*/
        }

        private void ClosePanel()
        {
            //Overlay.SetActive(false); //Close the Modal Dialog
            // Place Overlay, with Modal Panel, away from view of camera
            RectTransform overlayRect = Overlay.GetComponent<RectTransform>();
            overlayRect.offsetMin = new Vector2(-400f, overlayRect.offsetMin.y);
            overlayRect.offsetMax = new Vector2(-400f, overlayRect.offsetMax.y);
            isModalOpen.IsModalOpen = false;
        }
    }
}
