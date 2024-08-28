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

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            QuestCreationController.OnConfirmedQuestCreation += HandleQuestCreation;
            QuestManager.OnQuestAdded += HandleQuestAdded;
            QuestManager.OnQuestRemoved += HandleQuestRemoved;
            QuestManager.OnPassQuestList += HandlePassQuestList;
            QuestInspectorController.OnQuestRemovedFromPrompt += HandleQuestMarkedAsToBeRemoved;
            QuestInspectorController.OnQuestMarkedAsCompletedFromPrompt += HandleQuestMarkedAsCompleted;
            QuestInspectorController.OnQuestMarkedAsToDoFromPrompt += HandleQuestMarkedAsToDo;
            QuestSearchingController.OnRequestQuestList += HandleRequestQuestList;
            QuestSearchingController.OnPassSearchedQuestIDsList += HandlePassSearchedQuestIDsList;
            QuestSearchingController.OnSearchingCleared += HandleSearchCleared;
        }

        private void UnsubscribeFromEvents()
        {
            QuestCreationController.OnConfirmedQuestCreation -= HandleQuestCreation;
            QuestManager.OnQuestAdded -= HandleQuestAdded;
            QuestManager.OnQuestRemoved -= HandleQuestRemoved;
            QuestManager.OnPassQuestList -= HandlePassQuestList;
            QuestInspectorController.OnQuestRemovedFromPrompt -= HandleQuestMarkedAsToBeRemoved;
            QuestInspectorController.OnQuestMarkedAsCompletedFromPrompt -= HandleQuestMarkedAsCompleted;
            QuestInspectorController.OnQuestMarkedAsToDoFromPrompt -= HandleQuestMarkedAsToDo;
            QuestSearchingController.OnRequestQuestList -= HandleRequestQuestList;
            QuestSearchingController.OnPassSearchedQuestIDsList -= HandlePassSearchedQuestIDsList;
            QuestSearchingController.OnSearchingCleared -= HandleSearchCleared;
        }

        public void RequestQuestCreationPanel() => OnRequestQuestCreationPanel?.Invoke();

        private static void HandleQuestCreation(string title, string desc) => OnConfirmedQuestCreation?.Invoke(title, desc);

        private static void HandleQuestAdded(Quest quest) => OnQuestAdded?.Invoke(quest);

        private static void HandleQuestRemoved(Quest quest) => OnQuestRemoved?.Invoke(quest);

        private static void HandleQuestMarkedAsToBeRemoved(Quest quest) => OnMarkQuestAsToBeRemoved?.Invoke(quest);

        private static void HandleQuestMarkedAsToDo(Quest quest) => OnMarkQuestAsToDo?.Invoke(quest);

        private static void HandleQuestMarkedAsCompleted(Quest quest) => OnMarkQuestAsCompleted?.Invoke(quest);

        private static void HandleRequestQuestList() => OnRequestQuestList?.Invoke();

        private static void HandlePassQuestList(List<Quest> quests) => OnPassQuestList?.Invoke(quests);

        private static void HandlePassSearchedQuestIDsList(List<int> questIDsList) => OnPassSearchedQuestIDsList?.Invoke(questIDsList);

        private static void HandleSearchCleared() => OnSearchQuestCleared?.Invoke();
    }
}
