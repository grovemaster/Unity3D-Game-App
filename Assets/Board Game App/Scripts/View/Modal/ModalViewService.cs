using Data.Enum.Modal;
using ECS.Component.Modal;
using Svelto.ECS;
using UnityEngine.Events;
using UnityEngine.UI;

namespace View.Modal
{
    public class ModalViewService
    {
        private UnityAction closePanel;

        public ModalViewService(UnityAction closePanel)
        {
            this.closePanel = closePanel;
        }

        public void SetupButton(Button button, ITowerOptionBase config, DispatchOnSet<int> answer, bool addClosePanel)
        {
            DeactivateButton(button); // Reset button first

            button.onClick.AddListener(delegate()
            {
                // Trigger confirmation that clicked item was referenced piece id (engine will pick it up)
                answer.value = config.ReferencedPieceId;
            });

            if (addClosePanel)
            {
                button.onClick.AddListener(closePanel);
            }

            button.GetComponentInChildren<Text>().text = config.Name;

            button.gameObject.SetActive(true);
            button.interactable = config.Enabled;
        }

        public void SetupButton(
            Button button, ICaptureOrStackComponent answerContainer, ModalQuestionAnswer answer)
        {
            DeactivateButton(button); // Reset button first

            button.onClick.AddListener(delegate ()
            {
                // Trigger clicked answer for listening engine to pick up
                answerContainer.Answer.value = answer;
            });
            button.onClick.AddListener(closePanel);
            button.GetComponentInChildren<Text>().text = answer.ToString();

            button.gameObject.SetActive(true);
            button.interactable = true;
        }

        public void SetupButton(Button button, IConfirmComponent confimContainer, string buttonText, bool answer)
        {
            DeactivateButton(button);

            button.onClick.AddListener(delegate ()
            {
                // Trigger clicked answer for listening engine to pick up
                confimContainer.Answer.value = answer;
            });
            button.onClick.AddListener(closePanel);
            button.GetComponentInChildren<Text>().text = buttonText;

            button.gameObject.SetActive(true);
            button.interactable = true;
        }

        public void DeactivateButton(Button button)
        {
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        }
    }
}
