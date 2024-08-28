using System;
using System.Collections.Generic;
using System.Linq;
using Quests;
using UnityEngine;

namespace UI
{
    public class QuestInspectorController : MonoBehaviour
    {
        #region Variables
    
        [SerializeField] private Transform toDoQuestsPanel;
        [SerializeField] private Transform completedQuestsPanel;
    
        [SerializeField] private GameObject questPrefab;
        
        private readonly List<QuestPrompt> _toDoQuestList = new();
        private readonly List<QuestPrompt> _completedQuestList = new();

        #endregion

        #region Events
        
        public static event Action<Quest> OnQuestRemovedFromPrompt; 
        public static event Action<Quest> OnQuestMarkedAsCompletedFromPrompt;
        public static event Action<Quest> OnQuestMarkedAsToDoFromPrompt;

        #endregion

        private void Start()
        {
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            UIController.OnQuestAdded += AddQuest;
            UIController.OnQuestRemoved += RemoveQuest;
            UIController.OnPassSearchedQuestIDsList += DisplaySearchedQuests;
            UIController.OnSearchQuestCleared += DisplayAllQuests;
            QuestPrompt.OnRemoveQuestButtonClicked += RemoveQuestFromPrompt;
            QuestPrompt.OnQuestMarkedAsCompleted += CompleteQuestFromPrompt;
            QuestPrompt.OnQuestMarkedAsToDo += MarkQuestAsToDoFromPrompt;
        }

        private void UnsubscribeFromEvents()
        {
            UIController.OnQuestAdded -= AddQuest;
            UIController.OnQuestRemoved -= RemoveQuest;
            UIController.OnPassSearchedQuestIDsList -= DisplaySearchedQuests;
            UIController.OnSearchQuestCleared -= DisplayAllQuests;
            QuestPrompt.OnRemoveQuestButtonClicked -= RemoveQuestFromPrompt;
            QuestPrompt.OnQuestMarkedAsCompleted -= CompleteQuestFromPrompt;
            QuestPrompt.OnQuestMarkedAsToDo -= MarkQuestAsToDoFromPrompt;
        }

        private void AddQuest(Quest quest)
        {
            var questObject = Instantiate(questPrefab, toDoQuestsPanel);
            var questPrompt = questObject.GetComponent<QuestPrompt>();
            questPrompt.SetQuest(quest);
            _toDoQuestList.Add(questPrompt);
        }

        private void RemoveQuest(Quest quest)
        {
            RemoveQuestFromList(quest, _toDoQuestList);
            RemoveQuestFromList(quest, _completedQuestList);
        }

        private static void RemoveQuestFromList(Quest quest, List<QuestPrompt> questList)
        {
            var questPrompt = questList.FirstOrDefault(q => q.quest.QuestID == quest.QuestID);
            if (questPrompt == null) return;
            questList.Remove(questPrompt);
            Destroy(questPrompt.gameObject);
        }

        private void DisplayAllQuests()
        {
            SetQuestPromptsActive(_toDoQuestList, true);
            SetQuestPromptsActive(_completedQuestList, true);
        }

        private void DisplaySearchedQuests(List<int> questIDsList)
        {
            DisplayAllQuests();
            SetQuestPromptsActive(_toDoQuestList, questIDsList);
            SetQuestPromptsActive(_completedQuestList, questIDsList);
        }

        private static void SetQuestPromptsActive(List<QuestPrompt> questList, bool isActive)
        {
            foreach (var questPrompt in questList)
            {
                questPrompt.gameObject.SetActive(isActive);
            }
        }

        private static void SetQuestPromptsActive(List<QuestPrompt> questList, List<int> questIDsList)
        {
            foreach (var questPrompt in questList)
            {
                questPrompt.gameObject.SetActive(questIDsList.Contains(questPrompt.quest.QuestID));
            }
        }

        private static void MarkQuestAsToDoFromPrompt(Quest quest) => OnQuestMarkedAsToDoFromPrompt?.Invoke(quest);

        private static void CompleteQuestFromPrompt(Quest quest) => OnQuestMarkedAsCompletedFromPrompt?.Invoke(quest);

        private static void RemoveQuestFromPrompt(Quest quest) => OnQuestRemovedFromPrompt?.Invoke(quest);
    }
}
