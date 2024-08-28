using System;
using System.Collections.Generic;
using Quests;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        #region Events

        public static event Action OnRequestQuestCreationPanel;
        public static event Action<string, string> OnConfirmedQuestCreation;
        public static event Action<Quest> OnQuestAdded;
        public static event Action<Quest> OnQuestRemoved;
        public static event Action<Quest> OnMarkQuestAsToDo;
        public static event Action<Quest> OnMarkQuestAsCompleted;
        public static event Action<Quest> OnMarkQuestAsToBeRemoved;
        public static event Action OnRequestQuestList;
        public static event Action<List<Quest>> OnPassQuestList; 
        public static event Action<List<int>> OnPassSearchedQuestIDsList;
        public static event Action OnSearchQuestCleared;
        
        #endregion
        private void Start()
        {
            SubscribeToEvents();
        }
        private static void SubscribeToEvents()
        {
            QuestCreationController.OnConfirmedQuestCreation += AddQuestFromCreationPanel;
            QuestManager.OnQuestAdded += AddQuest;
            QuestManager.OnQuestRemoved += RemoveQuestFromInspector;
            QuestManager.OnPassQuestList += PassQuestList;
            QuestInspectorController.OnQuestRemovedFromPrompt += MarkQuestAsToBeRemovedFromInspector;
            QuestInspectorController.OnQuestMarkedAsCompletedFromPrompt += MarkQuestAsCompletedFromInspector;
            QuestInspectorController.OnQuestMarkedAsToDoFromPrompt += MarkQuestAsToDoFromInspector;
            QuestSearchingController.OnRequestQuestList += RequestQuestList;
            QuestSearchingController.OnPassSearchedQuestIDsList += PassSearchedQuestIDsList;
            QuestSearchingController.OnSearchingCleared += ClearSearching;
        }
        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
        private static void UnsubscribeFromEvents()
        {
            QuestCreationController.OnConfirmedQuestCreation -= AddQuestFromCreationPanel;
            QuestManager.OnQuestAdded -= AddQuest;
            QuestManager.OnQuestRemoved -= RemoveQuestFromInspector;
            QuestManager.OnPassQuestList -= PassQuestList;
            QuestInspectorController.OnQuestRemovedFromPrompt -= MarkQuestAsToBeRemovedFromInspector;
            QuestInspectorController.OnQuestMarkedAsCompletedFromPrompt -= MarkQuestAsCompletedFromInspector;
            QuestInspectorController.OnQuestMarkedAsToDoFromPrompt -= MarkQuestAsToDoFromInspector;
            QuestSearchingController.OnRequestQuestList -= RequestQuestList;
            QuestSearchingController.OnPassSearchedQuestIDsList -= PassSearchedQuestIDsList;
            QuestSearchingController.OnSearchingCleared -= ClearSearching;
        }

        public void RequestQuestCreationPanel() => OnRequestQuestCreationPanel?.Invoke();
        private static void MarkQuestAsToBeRemovedFromInspector(Quest quest) => OnMarkQuestAsToBeRemoved?.Invoke(quest);
        private static void MarkQuestAsToDoFromInspector(Quest quest) => OnMarkQuestAsToDo?.Invoke(quest);
        private static void MarkQuestAsCompletedFromInspector(Quest quest) => OnMarkQuestAsCompleted?.Invoke(quest);
        private static void AddQuest(Quest quest) => OnQuestAdded?.Invoke(quest);
        private static void RemoveQuestFromInspector(Quest quest) => OnQuestRemoved?.Invoke(quest);
        private static void AddQuestFromCreationPanel(string title, string desc) => OnConfirmedQuestCreation?.Invoke(title, desc);
        private static void RequestQuestList() => OnRequestQuestList?.Invoke();
        private static void PassQuestList(List<Quest> quests) => OnPassQuestList?.Invoke(quests);
        private static void PassSearchedQuestIDsList(List<int> questIDsList) => OnPassSearchedQuestIDsList?.Invoke(questIDsList);
        private static void ClearSearching() => OnSearchQuestCleared?.Invoke();
    }
}
