using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BayatGames.SaveGameFree;
using TMPro;

public class QuestManager : MonoBehaviour
{
    [SerializeField] public List<Quest> allQuests = new List<Quest>();
    public GameObject questPrefab;
    public GameObject questPanel;
    public GameObject menuUI;
    public GameObject questDesc;

    public GameObject questmark;

    public InventoryManager inventoryManager;
    public GameObject interactcanvas;

    public string saveKey;

    public void CheckQuestProgress()
    {
        foreach (var quest in allQuests.Where(q => q.isAvailable && !q.isComplete))
        {
            bool allGoalsMet = true; // Assume all goals are met at the start

            foreach (var goal in quest.questGoals)
            {
                // Check inventory for required item and amount
                if (goal.goalCategory == "collecting")
                {
                    goal.currentAmount = inventoryManager.GetQuantity(goal.requiredItemId);

                    // If the current amount is less than the required amount, this goal is not met
                    if (goal.currentAmount < goal.requiredAmount)
                    {
                        allGoalsMet = false; // Mark as not all goals met
                    }
                } else {
                    allGoalsMet = false;
                }
                // Add other goal categories if needed (e.g., interacting, defeating enemies)
                // else if (goal.goalCategory == "interacting") { ... }
            }

            if (allGoalsMet)
            {
                quest.isComplete = true;
                CompleteQuest(quest);
                CreateQuestDisplay();
                Debug.Log($"Quest '{quest.title}' is now complete! Distributing rewards.");

                if (quest.id == 8) {
                    var npcprogress = SaveGame.Load<Dictionary<string, int>>("NPCProgress");
                    npcprogress["Budiman"] = 14;
                    SaveGame.Save("NPCProgress", npcprogress);
                }
            }
        }
    }

    IEnumerator CheckQuestProgressRoutine()
    {
        while (true)
        {
            CheckQuestProgress(); // Call your quest checking logic here
            yield return new WaitForSeconds(1f); // Check every second
        }
    }

    void Start()
    {
        // regularly check quest
        StartCoroutine(CheckQuestProgressRoutine());


        menuUI.SetActive(true);
        questPanel.transform.parent.gameObject.SetActive(false);
        LoadQuests();
        CreateQuestDisplay();
    }

    public void LoadQuests()
    {
        allQuests = SaveGame.Load<List<Quest>>(saveKey) ?? new List<Quest>();
        Debug.Log("Quests loaded");
        CreateQuestDisplay();
    }

    public void SaveQuests()
    {
        SaveGame.Save(saveKey, allQuests);
        Debug.Log("Quests saved");
    }

    public void RemoveBurgers() {
        InventoryManager.Instance.RemoveItem(7,1);
        InventoryManager.Instance.RemoveItem(8,1);
        InventoryManager.Instance.RemoveItem(9,1);
    }

    public void RemoveBackQuest() {
        for(int i=10; i<13; i++) {
            var quest = GetQuestById(i);
            if (quest.isAvailable == true && quest.isComplete == false) {
                CompleteInteractQuest(i);
            }
        }
    }

    public void AddQuest(Quest newQuest)
    {
        if (!allQuests.Contains(newQuest))
        {
            allQuests.Add(newQuest);
        }
    }

    public void CompleteQuest(Quest quest)
    {
        if (quest.isComplete == true)
        {
            // Reward logic
            foreach (var reward in quest.rewards)
            {
                int itemId = reward.Key;
                int quantity = reward.Value;
                InventoryManager.Instance.AddItem(itemId, quantity);
            }

            // Unlock next quests
            foreach (int nextQuestId in quest.nextQuestIds)
            {
                var nextQuest = GetQuestById(nextQuestId);
                if (nextQuest != null)
                {
                    nextQuest.isAvailable = true;
                }
            }

            SaveQuests();
            CreateQuestDisplay(); // Refresh UI
        }
    }

    public void CompleteInteractQuest(int id)
    {
        // Find the quest by its ID
        var quest = GetQuestById(id);

        if (quest != null && !quest.isComplete)
        {
            // Update all goals related to interacting
            foreach (var goal in quest.questGoals)
            {
                if (goal.goalCategory == "interaction")
                {
                    goal.currentAmount = goal.requiredAmount; // Mark the goal as completed
                }
            }

            foreach (var reward in quest.rewards)
            {
                int itemId = reward.Key;
                int quantity = reward.Value;
                InventoryManager.Instance.AddItem(itemId, quantity);
            }

            // Unlock next quests
            foreach (int nextQuestId in quest.nextQuestIds)
            {
                var nextQuest = GetQuestById(nextQuestId);
                if (nextQuest != null)
                {
                    nextQuest.isAvailable = true;
                }
            }

            // Mark the quest as complete
            quest.isComplete = true;

            // Save the updated quest list
            SaveQuests();

            // Refresh the quest UI
            CreateQuestDisplay();

            Debug.Log($"Quest '{quest.title}' marked as completed through interaction.");
        }
    }

    public void HandReward(int id) {
        if (id < 4) {
            int x = PlayerPrefs.GetInt("Component");
            x += 1;
            PlayerPrefs.SetInt("Component", x);

            if (PlayerPrefs.GetInt("LockCombat") == 1) {
                PlayerPrefs.SetInt("LockCombat", 0);
                // PlayerPrefs.SetInt("UnlockCombat", 1);
            }
        }

        InventoryManager.Instance.AddItem(id, 1);
    }


    public Quest GetQuestById(int id)
    {
        return allQuests.FirstOrDefault(q => q.id == id);
    }

    public void OpenQuestMenu()
    {
        interactcanvas.SetActive(false);
        questmark.SetActive(false);
        CreateQuestDisplay();
        questPanel.transform.parent.gameObject.SetActive(true);
        menuUI.SetActive(false);
    }

    public void CloseQuestMenu()
    {
        questPanel.transform.parent.gameObject.SetActive(false);
        menuUI.SetActive(true);
        interactcanvas.SetActive(true);
    }

    public void CreateQuestDisplay()
    {
        // Clear existing quest UI elements
        foreach (Transform child in questPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Display available quests (not completed)
        foreach (var quest in allQuests.Where(q => q.isAvailable && !q.isComplete))
        {
            var questUI = Instantiate(questPrefab, questPanel.transform);

            var textComponent = questUI.GetComponentInChildren<TMP_Text>();
            if (textComponent != null)
            {
                textComponent.text = "  -\t" + quest.title;
            }

            var buttonComponent = questUI.GetComponent<UnityEngine.UI.Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(() => OnQuestButtonClicked(quest));
            }
        }

        // Display completed quests
        // foreach (var quest in allQuests.Where(q => q.isComplete))
        // {
        //     var questUI = Instantiate(questPrefab, questPanel.transform);

        //     var textComponent = questUI.GetComponentInChildren<TMP_Text>();
        //     if (textComponent != null)
        //     {
        //         textComponent.text = "  -\t" + quest.title;
        //     }

        //     // Change the color of the prefab to gray for completed quests
        //     var background = questUI.GetComponent<UnityEngine.UI.Image>();
        //     if (background != null)
        //     {
        //         background.color = Color.gray; // Adjust the shade as needed
        //     }

        //     var buttonComponent = questUI.GetComponent<UnityEngine.UI.Button>();
        //     if (buttonComponent != null)
        //     {
        //         buttonComponent.onClick.AddListener(() => OnQuestButtonClicked(quest));
        //     }
        // }
    }

    public void AddSaveInstance() {
        if(Save.instance.lastUnlockedCombatLevel < 5) {
            Save.instance.lastUnlockedCombatLevel += 1;
        } else {
            if (Save.instance.lastUnlockedWorld < 3)
                {
                    Save.instance.lastUnlockedWorld += 1;
                    Save.instance.lastUnlockedCombatLevel = 1;
                }
        }
    }


    public void OnQuestButtonClicked(Quest quest)
    {
        questDesc.transform.GetChild(0).GetComponent<TMP_Text>().text = quest.title;

        string description = quest.description + "\n\nRequirement:\n";

        foreach (var goal in quest.questGoals)
        {
            if (goal.goalCategory == "hunting" || goal.goalCategory == "collecting")
            {
                description += $"{goal.goalDescription}: ({goal.currentAmount}/{goal.requiredAmount})\n";
            }
            else
            {
                description += $"{goal.goalDescription}\n";
            }
        }

        questDesc.transform.GetChild(1).GetComponent<TMP_Text>().text = description;

        var buttoncomponent = questDesc.transform.parent.GetChild(1).GetComponent<UnityEngine.UI.Button>();
        if (buttoncomponent != null)
            {
                buttoncomponent.onClick.RemoveAllListeners();
                buttoncomponent.onClick.AddListener(() => OnTrackButtonClicked(quest));
            }
    }

    public void OnTrackButtonClicked(Quest quest) {
        foreach (var goal in quest.questGoals)
        {
            // Simulate fulfilling the requirements
            // goal.currentAmount += 1;
            inventoryManager.AddItem(goal.requiredItemId, 1);
            // OnQuestButtonClicked(quest);
            // Debug.Log($"Goal '{goal.goalDescription}' completed. ({goal.currentAmount}/{goal.requiredAmount})");
        }
    }

    public void GiveQuest(int questId)
    {
        // Find the quest by ID
        Quest questToGive = GetQuestById(questId);

        if (questToGive != null && questToGive.isAvailable == false)
        {
            // Set the quest as available
            questToGive.isAvailable = true;

            // Save the updated quests to persist changes
            SaveQuests();

            questmark.SetActive(true);

            // Refresh the quest display to show the updated quest list
            CreateQuestDisplay();

            
        }
    }

    

}
