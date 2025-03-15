using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckComponent : MonoBehaviour
{
    // Start is called before the first frame update
    public QuestManager questManager;
    void Start()
    {
        if (PlayerPrefs.GetInt("Arcade") == 1) {
            // InventoryManager.Instance.AddItem(3,1);
            questManager.CompleteInteractQuest(10);
            questManager.HandReward(3);
            questManager.GiveQuest(13);
            PlayerPrefs.SetInt("Arcade", -1);
            PlayerPrefs.Save();
        }
    }
}
