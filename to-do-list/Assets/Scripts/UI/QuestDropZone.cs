using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class QuestDropZone : MonoBehaviour, IDropHandler
    {
        private Transform _content;
        [SerializeField] private bool setQuestAsCompleted;

        private void Start()
        {
            _content = GetComponentsInChildren<HorizontalOrVerticalLayoutGroup>()[0].transform;
        }

        public void OnDrop(PointerEventData eventData)
        {
            var questPrompt = eventData.pointerDrag.GetComponent<QuestPrompt>();
            if (questPrompt == null) return;
            
            questPrompt.GetComponent<Transform>().SetParent(_content);
            
            switch (setQuestAsCompleted)
            {
                case true:
                    questPrompt.MarkQuestAsCompleted();
                    break;
                case false:
                    questPrompt.MarkQuestAsToDo();
                    break;
            }
        }
    }
}