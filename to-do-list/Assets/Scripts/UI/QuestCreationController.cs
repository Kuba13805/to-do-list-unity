using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class QuestCreationController : MonoBehaviour
    {
        #region Variables

        [SerializeField] private GameObject questCreationPanel;
        [SerializeField] private TMP_InputField questNameInput;
        [SerializeField] private TMP_InputField questDescriptionInput;

        [SerializeField] private float warningTime = 2f;
        [SerializeField] private Color warningColor;

        #endregion

        #region Events

        public static event Action<string, string> OnConfirmedQuestCreation;

        #endregion
        
        private void Start()
        {
            questCreationPanel.SetActive(false);
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            UIController.OnRequestQuestCreationPanel += ShowQuestCreationPanel;
        }

        private void UnsubscribeFromEvents()
        {
            UIController.OnRequestQuestCreationPanel -= ShowQuestCreationPanel;
        }

        private void ShowQuestCreationPanel()
        {
            questCreationPanel.SetActive(true);
        }

        public void ConfirmQuestCreation()
        {
            if (string.IsNullOrEmpty(questNameInput.text) || string.IsNullOrWhiteSpace(questNameInput.text))
            {
                StartCoroutine(FlashInputField(questNameInput));
                return;
            }

            if (string.IsNullOrEmpty(questDescriptionInput.text) || string.IsNullOrWhiteSpace(questDescriptionInput.text))
            {
                StartCoroutine(FlashInputField(questDescriptionInput));
                return;
            }

            OnConfirmedQuestCreation?.Invoke(questNameInput.text, questDescriptionInput.text);
            ClearQuestCreation();
        }

        public void CancelQuestCreation()
        {
            ClearQuestCreation();
        }

        private void ClearQuestCreation()
        {
            questCreationPanel.SetActive(false);
            questNameInput.text = "";
            questDescriptionInput.text = "";
        }

        private IEnumerator FlashInputField(Selectable inputField)
        {
            var halfWarningTime = warningTime / 2f;
            var originalColor = inputField.colors.normalColor;

            yield return LerpInputFieldColor(inputField, originalColor, warningColor, halfWarningTime);
            yield return LerpInputFieldColor(inputField, warningColor, originalColor, halfWarningTime);
        }

        private IEnumerator LerpInputFieldColor(Selectable inputField, Color fromColor, Color toColor, float duration)
        {
            var elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var inputFieldColors = inputField.colors;
                inputFieldColors.normalColor = Color.Lerp(fromColor, toColor, elapsedTime / duration);
                inputField.colors = inputFieldColors;
                yield return null;
            }
        }
    }
}