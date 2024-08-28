using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

namespace Quests
{
    public class QuestManager : MonoBehaviour
    {
        #region Variables

        [SerializeField] private int maxQuests;
        private readonly List<Quest> _toDoQuestList = new();
        private readonly List<Quest> _completedQuestList = new();
        private readonly HashSet<int> _questIdsInUse = new();
        private readonly Queue<int> _questIdsAvailable = new();

        #endregion

        #region Events

        public static event Action<Quest> OnQuestAdded;
        public static event Action<Quest> OnQuestRemoved;
        public static event Action<List<Quest>> OnPassQuestList;

        #endregion

        private void Start()
        {
            InitializeQuestIds();
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void InitializeQuestIds()
        {
            for (int i = 0; i < maxQuests; i++)
            {
                _questIdsAvailable.Enqueue(i);
            }
        }

        private int GetNextQuestId()
        {
            if (_questIdsAvailable.Count == 0) return -1;
            var id = _questIdsAvailable.Dequeue();
            _questIdsInUse.Add(id);
            return id;
        }

        private void ReleaseQuestId(int id)
        {
            if (_questIdsInUse.Remove(id))
            {
                _questIdsAvailable.Enqueue(id);
            }
        }

        private void AddQuest(string title, string description)
        {
            var quest = new Quest(GetNextQuestId(), title, description);
            if (quest.QuestID == -1 || _toDoQuestList.Any(q => q.QuestID == quest.QuestID))
            {
                return;
            }

            _toDoQuestList.Add(quest);
            OnQuestAdded?.Invoke(quest);
        }

        private void PassQuestList()
        {
            var questList = _toDoQuestList.Concat(_completedQuestList).ToList();
            OnPassQuestList?.Invoke(questList);
        }

        private void MarkQuestAsToDo(Quest quest)
        {
            if (!_completedQuestList.Remove(quest)) return;
            quest.IsQuestCompleted = false;
            _toDoQuestList.Add(quest);
        }

        private void MarkQuestAsCompleted(Quest quest)
        {
            if (!_toDoQuestList.Remove(quest)) return;
            quest.IsQuestCompleted = true;
            _completedQuestList.Add(quest);
        }

        private void RemoveQuest(Quest quest)
        {
            if (!_toDoQuestList.Remove(quest) && !_completedQuestList.Remove(quest)) return;
            ReleaseQuestId(quest.QuestID);
            OnQuestRemoved?.Invoke(quest);
        }

        private void SubscribeToEvents()
        {
            UIController.OnConfirmedQuestCreation += AddQuest;
            UIController.OnMarkQuestAsToDo += MarkQuestAsToDo;
            UIController.OnMarkQuestAsCompleted += MarkQuestAsCompleted;
            UIController.OnMarkQuestAsToBeRemoved += RemoveQuest;
            UIController.OnRequestQuestList += PassQuestList;
        }

        private void UnsubscribeFromEvents()
        {
            UIController.OnConfirmedQuestCreation -= AddQuest;
            UIController.OnMarkQuestAsToDo -= MarkQuestAsToDo;
            UIController.OnMarkQuestAsCompleted -= MarkQuestAsCompleted;
            UIController.OnMarkQuestAsToBeRemoved -= RemoveQuest;
            UIController.OnRequestQuestList -= PassQuestList;
        }
    }
}
