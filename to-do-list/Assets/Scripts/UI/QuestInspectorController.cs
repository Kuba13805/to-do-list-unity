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
        
        private List<QuestPrompt> _toDoQuestList = new();
        private List<QuestPrompt> _completedQuestList = new();

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

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void UnsubscribeFromEvents()
        {
            UIController.OnQuestAdded -= AddQuest;
            UIController.OnQuestRemoved -= RemoveQuest;
            UIController.OnPassSearchedQuestIDsList += DisplaySearchedQuests;
            UIController.OnSearchQuestCleared -= DisplayAllQuests;
            QuestPrompt.OnRemoveQuestButtonClicked -= RemoveQuestFromPrompt;
            QuestPrompt.OnQuestMarkedAsCompleted -= CompleteQuestFromPrompt;
            QuestPrompt.OnQuestMarkedAsToDo -= MarkQuestAsToDoFromPrompt;
        }


        private void AddQuest(Quest quest)
        {
            var questObject = Instantiate(questPrefab, toDoQuestsPanel);

            questObject.GetComponent<QuestPrompt>().SetQuest(quest);

            _toDoQuestList.Add(questObject.GetComponent<QuestPrompt>());
        }

        private void RemoveQuest(Quest quest)
        {
            SearchForQuestAndRemoveFromInspector(quest);
        }

        private void SearchForQuestAndRemoveFromInspector(Quest quest)
        {
            _toDoQuestList.Remove(SearchForQuestInInspector(quest, _toDoQuestList));
            _completedQuestList.Remove(SearchForQuestInInspector(quest, _completedQuestList));
        }
        private void DisplayAllQuests()
        {
            foreach (var questPrompt in _toDoQuestList) questPrompt.gameObject.SetActive(true);
            foreach (var questPrompt in _completedQuestList) questPrompt.gameObject.SetActive(true);
        }
        private void DisplaySearchedQuests(List<int> iDsList)
        {
            DisplayAllQuests();
            foreach (var questPrompt in _toDoQuestList.Where(questPrompt => !iDsList.Contains(questPrompt.quest.QuestID))) questPrompt.gameObject.SetActive(false);
            foreach (var questPrompt in _completedQuestList.Where(questPrompt => !iDsList.Contains(questPrompt.quest.QuestID))) questPrompt.gameObject.SetActive(false);
        }
        private static void MarkQuestAsToDoFromPrompt(Quest quest) => OnQuestMarkedAsToDoFromPrompt?.Invoke(quest);

        private static void CompleteQuestFromPrompt(Quest quest) => OnQuestMarkedAsCompletedFromPrompt?.Invoke(quest);

        private static void RemoveQuestFromPrompt(Quest quest) => OnQuestRemovedFromPrompt?.Invoke(quest);

        private static QuestPrompt SearchForQuestInInspector(Quest quest, IEnumerable<QuestPrompt> listOfPrompts)
        {
            return listOfPrompts.FirstOrDefault(questPrompt => questPrompt.quest.QuestID == quest.QuestID);
        }
    }
}
