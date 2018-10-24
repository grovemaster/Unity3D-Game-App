using Data.Enums.Modal;
using Data.Enums.Piece.Side;
using ECS.Component.Modal;
using Svelto.ECS;
using System;
using UI.Modal;
using UnityEngine;
using UnityEngine.UI;
using View.Modal;

namespace ECS.Implementor.Modal
{
    class ModalImpl : MonoBehaviour, IImplementor, IModalTypeComponent, IAnswerComponent
    {
        public GameObject Overlay;                // Need overlay to hide/show modal, since it contains modal
        private TrackIsModalOpen isModalOpen;

        // Modal elements
        public Text Title;     // The Modal Window Title
        public Text Question;  // The Modal Window Question (or statement)
        public Button Button1;   // The first button
        public Button Button2;   // The second button
        public Button Button3;   // The third button
        public Button CancelButton; // The cancel modal button
        public Image IconImage; // The Icon Image, if any

        // Other Impls contained here so values may be used in logic
        // Value set through Unity3D Editor
        private ModalQuestionImpl questionAnswer;
        private ModalDropFrontBackImpl dropFrontBack;
        private ModalConfirmImpl confirm;
        private IModalVictoriousPlayerImpl victoriousPlayer;
        private ModalVisibilityImpl visibility;

        // Services
        private ModalViewService modalViewService;

        private ModalType? previousModalType = null;
        private ModalType? previous2ndModalType = null;

        public ModalType Type { get; set; }
        public DispatchOnSet<int> Answer { get; set; }

        void Awake()
        {
            modalViewService = new ModalViewService(ClosePanelVisibility);
            Answer = new DispatchOnSet<int>();
        }

        void Start()
        {
            isModalOpen = FindObjectOfType<TrackIsModalOpen>();

            questionAnswer = gameObject.GetComponentInChildren<ModalQuestionImpl>();
            dropFrontBack = gameObject.GetComponentInChildren<ModalDropFrontBackImpl>();
            confirm = gameObject.GetComponentInChildren<ModalConfirmImpl>();
            victoriousPlayer = gameObject.GetComponentInChildren<IModalVictoriousPlayerImpl>();
            visibility = gameObject.GetComponentInChildren<ModalVisibilityImpl>();

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
            switch(Type)
            {
                case ModalType.CAPTURE_STACK:
                    SetTitle("Capture Or Stack");
                    SetQuestion("Capture piece or stack on top of it?");
                    modalViewService.DeactivateButton(Button1);
                    modalViewService.SetupButton(Button2, questionAnswer, ModalQuestionAnswer.CAPTURE);
                    modalViewService.SetupButton(Button3, questionAnswer, ModalQuestionAnswer.STACK);
                    break;
                case ModalType.CONFIRM:
                    SetTitle("Forced Recovery");
                    SetQuestion("Perform Forced Recovery?");
                    modalViewService.DeactivateButton(Button1);
                    modalViewService.SetupButton(Button2, confirm, "Yes", true);
                    modalViewService.SetupButton(Button3, confirm, "No", false);
                    break;
                case ModalType.FRONT_BACK:
                    SetTitle("Drop");
                    SetQuestion("Drop front or back side?");
                    modalViewService.DeactivateButton(Button1);
                    modalViewService.SetupButton(Button2, dropFrontBack, dropFrontBack.Front.ToString(), PieceSide.FRONT);
                    modalViewService.SetupButton(Button3, dropFrontBack, dropFrontBack.Back.ToString(), PieceSide.BACK);
                    break;
                case ModalType.SUBSTITUTION_CLICK:
                    SetTitle("Substitution or Click");
                    SetQuestion("Substitution ability or click highlight?");
                    modalViewService.DeactivateButton(Button1);
                    modalViewService.SetupButton(Button2, questionAnswer, ModalQuestionAnswer.SUBSTITUTION);
                    modalViewService.SetupButton(Button3, questionAnswer, ModalQuestionAnswer.CLICK);
                    break;
                case ModalType.TIER_1_3_EXCHANGE_CLICK:
                    SetTitle("1-3 Tier Exchange or Click");
                    SetQuestion("1-3 Tier Exchange ability or click highlight?");
                    modalViewService.DeactivateButton(Button1);
                    modalViewService.SetupButton(Button2, questionAnswer, ModalQuestionAnswer.TIER_1_3_EXCHANGE);
                    modalViewService.SetupButton(Button3, questionAnswer, ModalQuestionAnswer.CLICK);
                    break;
                case ModalType.CLICK_IMMOBILE_CAPTURE:
                    SetTitle("Click or Immobile Capture");
                    SetQuestion("Click highlight or immobile capture?");
                    modalViewService.DeactivateButton(Button1);
                    modalViewService.SetupButton(Button2, questionAnswer, ModalQuestionAnswer.CLICK);
                    modalViewService.SetupButton(Button3, questionAnswer, ModalQuestionAnswer.IMMOBILE_CAPTURE);
                    break;
                case ModalType.CHECKMATE:
                    SetTitle("Game Over");
                    SetQuestion("Winner: " + victoriousPlayer.PlayerColor.ToString());
                    modalViewService.DeactivateButton(Button1);
                    modalViewService.SetupReloadSceneButton(Button2);
                    modalViewService.SetupLoadTitleSceneButton(Button3);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported ModalType");
            }

            ActivateModal();
            previous2ndModalType = previousModalType;
            previousModalType = Type;
            isModalOpen.IsModalOpen = true;
        }

        private void SetTitle(string title = "Select Tower Piece")
        {
            Title.text = title;           //Fill in the Title part of the Message Box
        }

        private void SetQuestion(string question = "Select Tower Piece")
        {
            Question.text = question;     //Fill in the Question/statement part of the Messsage Box
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
            if ((Type == ModalType.CONFIRM && previousModalType == ModalType.CONFIRM && previous2ndModalType == ModalType.CAPTURE_STACK)
                || (Type == ModalType.TOWER_3RD_TIER && previousModalType == ModalType.TOWER_3RD_TIER && previous2ndModalType == ModalType.TIER_1_3_EXCHANGE_CLICK))
            {
                previousModalType = null;
            }
            else
            {
                visibility.IsVisible.value = false;
            }
        }

        private void ClosePanel()
        {
            //Overlay.SetActive(false); //Close the Modal Dialog
            // Place Overlay, with Modal Panel, away from view of camera
            RectTransform overlayRect = Overlay.GetComponent<RectTransform>();
            overlayRect.offsetMin = new Vector2(-400f, overlayRect.offsetMin.y);
            overlayRect.offsetMax = new Vector2(-400f, overlayRect.offsetMax.y);
            isModalOpen.IsModalOpen = false;
            previousModalType = null;
            previous2ndModalType = null;
        }
    }
}
