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

        [SerializeField] private int maxQuests = 10;
        [SerializeField] private List<Quest> _toDoQuestList = new();
        [SerializeField] private List<Quest> _completedQuestList = new();
        private HashSet<int> _questIdsInUse = new();
        private Queue<int> _questIdsAvailable = new();

        #endregion

        #region Events

        public static event Action<Quest> OnQuestAdded;
        public static event Action<Quest> OnQuestRemoved;
        public static event Action<Quest> OnQuestCompleted;
        public static event Action<List<Quest>> OnPassQuestList; 

        #endregion

        private void Start()
        {
            InitializeQuestIds();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            UIController.OnConfirmedQuestCreation += AddQuest;
            UIController.OnQuestCompleted += CompleteQuest;
            UIController.OnMarkQuestAsToDo += MarkQuestAsToDo;
            UIController.OnMarkQuestAsCompleted += MarkQuestAsCompleted;
            UIController.OnMarkQuestAsToBeRemoved += RemoveQuest;
            UIController.OnRequestQuestList += PassQuestList;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void UnsubscribeFromEvents()
        {
            UIController.OnConfirmedQuestCreation -= AddQuest;
            UIController.OnQuestCompleted -= CompleteQuest;
            UIController.OnMarkQuestAsToDo -= MarkQuestAsToDo;
            UIController.OnMarkQuestAsCompleted -= MarkQuestAsCompleted;
            UIController.OnMarkQuestAsToBeRemoved -= RemoveQuest;
            UIController.OnRequestQuestList -= PassQuestList;
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
            if (_questIdsAvailable.Count <= 0 ) return -1;
            var id = _questIdsAvailable.Dequeue();
            _questIdsInUse.Add(id);
            return id;
        }

        private void ReleaseQuestId(int id)
        {
            if(!_questIdsInUse.Contains(id)) return;
            _questIdsInUse.Remove(id);
            _questIdsAvailable.Enqueue(id);
        }
        private void AddQuest(string title, string description)
        {
            var quest = new Quest(GetNextQuestId(),title, description);

            if (_toDoQuestList.Any(questToDo => quest.QuestID == questToDo.QuestID) || quest.QuestID == -1)
            {
                return;
            }
            
            _toDoQuestList.Add(quest);
            OnQuestAdded?.Invoke(quest);
        }
        private void PassQuestList()
        {
            var questList = new List<Quest>(_toDoQuestList);
            questList.AddRange(_completedQuestList);

            OnPassQuestList?.Invoke(questList);
        }
        private void MarkQuestAsToDo(Quest quest)
        {
            _toDoQuestList.Add(quest);
            _completedQuestList.Remove(quest);
            quest.IsQuestCompleted = false;
        }
        private void MarkQuestAsCompleted(Quest quest)
        {
            _toDoQuestList.Remove(quest);
            _completedQuestList.Add(quest);
            quest.IsQuestCompleted = true;
        }
        private void RemoveQuest(Quest quest)
        {
            SearchForQuestToRemove(quest, _toDoQuestList);
            SearchForQuestToRemove(quest, _completedQuestList);
            OnQuestRemoved?.Invoke(quest);
        }

        private void SearchForQuestToRemove(Quest questToRemove, ICollection<Quest> listOfQuests)
        {
            foreach (var quest in listOfQuests)
            {
                if (questToRemove.QuestID != quest.QuestID) continue;
                
                listOfQuests.Remove(quest);
                ReleaseQuestId(quest.QuestID);
                break;
            }
        }

        private void CompleteQuest(Quest quest)
        {
            _completedQuestList.Add(quest);
            _toDoQuestList.Remove(quest);
            quest.IsQuestCompleted = true;
            OnQuestCompleted?.Invoke(quest);
        }
    }
}
