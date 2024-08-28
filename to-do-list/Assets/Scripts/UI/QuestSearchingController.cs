using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class QuestSearchingController : MonoBehaviour
    {
        #region Variables

        [SerializeField] private GameObject questSearchPanel;
        [SerializeField] private TMP_InputField questSearchText;
        [SerializeField] private Toggle includeQuestDescInSearching;

        #endregion

        #region Events

        public static event Action OnRequestQuestList;
        public static event Action<List<int>> OnPassSearchedQuestIDsList;
        public static event Action OnSearchingCleared;

        #endregion

        private void Start()
        {
            questSearchPanel.SetActive(false);
        }

        public void OpenQuestSearchPanel()
        {
            questSearchPanel.SetActive(true);
        }

        public void CloseQuestSearchPanel()
        {
            questSearchPanel.SetActive(false);
            OnSearchingCleared?.Invoke();
        }

        public void StartSearch()
        {
            StartCoroutine(SearchQuests());
        }
        private IEnumerator SearchQuests()
        {
            var questList = new List<Quest>();

            void OnQuestListReceived(List<Quest> list) => questList = list;
            UIController.OnPassQuestList += OnQuestListReceived;
            OnRequestQuestList?.Invoke();

            yield return new WaitUntil(() => questList.Count > 0);

            UIController.OnPassQuestList -= OnQuestListReceived;

            var searchQuery = questSearchText.text.ToLower();
            var questIDsList = new List<int>();

            if (string.IsNullOrEmpty(searchQuery))
            {
                questIDsList.AddRange(questList.Select(quest => quest.QuestID));
            }
            else
            {
                var queryPredicate = new Func<Quest, bool>(quest =>
                    quest.QuestTitle.ToLower().Contains(searchQuery) ||
                    (includeQuestDescInSearching.isOn && quest.QuestDescription.ToLower().Contains(searchQuery))
                );

                questIDsList.AddRange(questList.Where(queryPredicate).Select(quest => quest.QuestID));
            }

            OnPassSearchedQuestIDsList?.Invoke(questIDsList);
        }
    }
}
