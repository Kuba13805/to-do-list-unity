using System;
using Quests;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class QuestPrompt : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        #region Variables

        [SerializeField] private Sprite toDoQuestIcon;
        [SerializeField] private Sprite completedQuestIcon;
        
        [SerializeField] private TextMeshProUGUI questTitleBox;
        [SerializeField] private Image questIcon;
        [SerializeField] private GameObject removeQuestButton;
        [SerializeField] private GameObject questInfoBox;
        [SerializeField] private TextMeshProUGUI questDescriptionBox;

        [SerializeField] private Color normalTextColor;
        [SerializeField] private Color completedTextColor;
        
        public Quest quest;

        private int _clickCount = 0;
        private float _lastClickTime = 0f;
        private const float DoubleClickThreshold = 0.3f;

        #endregion

        #region Events

        public static event Action<Quest> OnQuestMarkedAsToDo; 
        public static event Action<Quest> OnRemoveQuestButtonClicked;
        public static event Action<Quest> OnQuestMarkedAsCompleted; 

        #endregion

        public void SetQuest(Quest newQuest)
        {
            removeQuestButton.SetActive(false);
            quest = newQuest;
            questTitleBox.text = newQuest.QuestTitle;
            questIcon.sprite = toDoQuestIcon;
            RebuildLayout();
        }
        public void MarkQuestAsCompleted()
        {
            questIcon.sprite = completedQuestIcon;
            questTitleBox.fontStyle = FontStyles.Strikethrough;
            questTitleBox.color = completedTextColor;
            OnQuestMarkedAsCompleted?.Invoke(quest);
        }

        public void MarkQuestAsToDo()
        {
            questIcon.sprite = toDoQuestIcon;
            questTitleBox.fontStyle = FontStyles.Normal;
            questTitleBox.color = normalTextColor;
            OnQuestMarkedAsToDo?.Invoke(quest);
        }

        public void RemoveQuest()
        {
            OnRemoveQuestButtonClicked?.Invoke(quest);
            Destroy(gameObject);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            removeQuestButton.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            removeQuestButton.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Time.time - _lastClickTime < DoubleClickThreshold)
            {
                _clickCount++;
            }
            else
            {
                _clickCount = 1;
            }

            _lastClickTime = Time.time;

            if (_clickCount != 2) return;
            
            DisplayQuestInfo();
            _clickCount = 0;
        }

        private void DisplayQuestInfo()
        {
            if (!questInfoBox.activeSelf)
            {
                questInfoBox.SetActive(true);
                questDescriptionBox.text = quest.QuestDescription;
            }
            else
            {
                questInfoBox.SetActive(false);
            }
            RebuildLayout();
        }

        private void RebuildLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(questInfoBox.transform as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }
    }
}
