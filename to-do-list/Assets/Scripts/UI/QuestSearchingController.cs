using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Quests;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

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
        StartCoroutine(Searching());
    }
    private IEnumerator Searching()
    {
        var questList = new List<Quest>();
        
        Action<List<Quest>> onQuestListReceived = (list) => questList = list;
        UIController.OnPassQuestList += onQuestListReceived;
        OnRequestQuestList?.Invoke();
        
        yield return new WaitUntil(() => questList != null);
        
        UIController.OnPassQuestList -= onQuestListReceived;
        var questIDsList = new List<int>();

        var searchedText = questSearchText.text.ToLower();
        if (string.IsNullOrEmpty(questSearchText.text))
        {
            questIDsList.AddRange(questList.Select(quest => quest.QuestID));
            Debug.Log($"No filter: {questIDsList.Count} quests found");
        }
        else if (includeQuestDescInSearching.isOn)
        {
            questIDsList.AddRange(from quest in questList where quest.QuestTitle.ToLower().Contains(searchedText) || quest.QuestDescription.ToLower().Contains(searchedText) select quest.QuestID);
            Debug.Log($"With description included: {questIDsList.Count} quests found");
        }
        else
        {
            questIDsList.AddRange(from quest in questList where quest.QuestTitle.ToLower().Contains(searchedText) select quest.QuestID);
            Debug.Log($"Without description included: {questIDsList.Count} quests found");
        }
        OnPassSearchedQuestIDsList?.Invoke(questIDsList);
        yield return null;
    }
}
