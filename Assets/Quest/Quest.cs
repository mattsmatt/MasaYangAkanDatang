using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public int id;
    public string title;
    public string description;
    [SerializeField] public Dictionary<int, int> rewards; // Key: Item ID, Value: Quantity
    public List<QuestGoal> questGoals;
    public bool isComplete = false;
    public bool isAvailable = false; // Track if the quest is available
    public List<int> nextQuestIds; // Store IDs of the next quests

    public Quest(int id, string title, string description, Dictionary<int, int> rewards, List<QuestGoal> goals, List<int> nextQuestIds)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.rewards = rewards;
        this.questGoals = goals;
        this.isAvailable = false; // Default to unavailable
        this.nextQuestIds = nextQuestIds;
    }
}

[System.Serializable]
public class QuestGoal
{
    public string goalDescription;

    public string goalCategory;
    public int requiredItemId;
    public int requiredAmount;
    public int currentAmount;

    public QuestGoal(string category, string description, int requiredamnt, int requiredItemId)
    {
        this.goalCategory = category;
        this.goalDescription = description;
        this.requiredAmount = requiredamnt;
        this.requiredItemId = requiredItemId;
        this.currentAmount = 0;
    }

    public bool IsReached()
    {
        return currentAmount >= requiredAmount;
    }

    public void IncrementProgress(int amount = 1)
    {
        currentAmount += amount;
    }
}
