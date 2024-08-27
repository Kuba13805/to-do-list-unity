using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class QuestDropZone : MonoBehaviour, IDropHandler
    {
        private Transform _content;

        private void Start()
        {
            _content = GetComponentsInChildren<HorizontalOrVerticalLayoutGroup>()[0].transform;
        }

        public void OnDrop(PointerEventData eventData)
        {
            var questPrompt = eventData.pointerDrag.GetComponent<QuestPrompt>();
            if (questPrompt == null) return;
            
            questPrompt.GetComponent<Transform>().SetParent(_content);
            
            switch (questPrompt.quest.IsQuestCompleted)
            {
                case false:
                    questPrompt.MarkQuestAsCompleted();
                    break;
                case true:
                    questPrompt.MarkQuestAsToDo();
                    break;
            }
        }
    }
}