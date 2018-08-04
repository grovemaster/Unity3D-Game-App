using Data.Enum.Modal;
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
        private Tier1OptionImpl tier1Option;
        private Tier2OptionImpl tier2Option;
        private Tier3OptionImpl tier3Option;
        private ModalQuestionImpl questionAnswer;
        private ModalVisibilityImpl visibility;

        // Services
        private ModalViewService modalViewService;

        public ModalType Type { get; set; }
        public DispatchOnSet<int> Answer { get; set; }

        void Awake()
        {
            modalViewService = new ModalViewService(ClosePanel);
            Answer = new DispatchOnSet<int>();
        }

        void Start()
        {
            isModalOpen = FindObjectOfType<TrackIsModalOpen>();

            tier1Option = gameObject.GetComponentInChildren<Tier1OptionImpl>();
            tier2Option = gameObject.GetComponentInChildren<Tier2OptionImpl>();
            tier3Option = gameObject.GetComponentInChildren<Tier3OptionImpl>();
            questionAnswer = gameObject.GetComponentInChildren<ModalQuestionImpl>();
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
                case ModalType.TOWER_2ND_TIER:
                    SetTitle();
                    SetQuestion();
                    modalViewService.DeactivateButton(Button1);
                    modalViewService.SetupButton(Button2, tier1Option, Answer);
                    modalViewService.SetupButton(Button3, tier2Option, Answer);
                    break;
                case ModalType.TOWER_3RD_TIER:
                    modalViewService.SetupButton(Button1, tier1Option, Answer);
                    modalViewService.SetupButton(Button2, tier2Option, Answer);
                    modalViewService.SetupButton(Button3, tier3Option, Answer);
                    break;
                case ModalType.CAPTURE_STACK:
                    SetTitle("Capture Or Stack");
                    SetQuestion("Capture piece or stack on top of it?");
                    modalViewService.DeactivateButton(Button1);
                    modalViewService.SetupButton(Button2, questionAnswer, ModalQuestionAnswer.CAPTURE);
                    modalViewService.SetupButton(Button3, questionAnswer, ModalQuestionAnswer.STACK);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported ModalType");
            }

            ActivateModal();
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
