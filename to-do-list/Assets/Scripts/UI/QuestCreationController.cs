using System;
using System.Collections;
using TMPro;
using UnityEngine;

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
            UIController.OnRequestQuestCreationPanel += ShowQuestCreationPanel;
        }

        private void OnDestroy()
        {
            UIController.OnRequestQuestCreationPanel -= ShowQuestCreationPanel;
        }

        private void ShowQuestCreationPanel()
        {
            questCreationPanel.SetActive(true);
        }
        
        public void ConfirmQuestCreation()
        {
            if (string.IsNullOrEmpty(questNameInput.text))
            {
                StartCoroutine(RequestInputFieldFilling(questNameInput));
                return;
            }

            if (string.IsNullOrEmpty(questDescriptionInput.text))
            {
                StartCoroutine(RequestInputFieldFilling(questDescriptionInput));
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

        private IEnumerator RequestInputFieldFilling(TMP_InputField inputField)
        {
            float elapsedTime = 0f;
            Color originalColor = inputField.colors.normalColor;

            while (elapsedTime < warningTime / 2)
            {
                elapsedTime += Time.deltaTime;
                var inputFieldColors = inputField.colors;
                inputFieldColors.normalColor = Color.Lerp(originalColor, warningColor, elapsedTime / (warningTime / 2));
                inputField.colors = inputFieldColors;
                yield return null;
            }
            
            elapsedTime = 0f;
            
            while (elapsedTime < warningTime / 2)
            {
                elapsedTime += Time.deltaTime;
                var inputFieldColors = inputField.colors;
                inputFieldColors.normalColor = Color.Lerp(warningColor, originalColor, elapsedTime / (warningTime / 2));
                inputField.colors = inputFieldColors;
                yield return null;
            }
        }
    }
}