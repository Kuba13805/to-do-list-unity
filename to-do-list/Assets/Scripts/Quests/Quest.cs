namespace Quests
{
    [System.Serializable]
    public class Quest
    {
        public string QuestTitle;
        public string QuestDescription;
        public bool IsQuestCompleted;
        public int QuestID;

        public Quest(int id, string title, string description)
        {
            QuestID = id;
            QuestTitle = title;
            QuestDescription = description;
            IsQuestCompleted = false;
        }
    }
}