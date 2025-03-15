using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save : MonoBehaviour
{
    public int lastUnlockedFill = 1;
    public int lastUnlockedSlidingTile = 1;
    public int lastUnlockedOnestroke = 1;
    public int lastUnlockedPipes = 1;

    public int lastUnlockedWorld = 1;
    public int lastUnlockedCombatLevel = 1;

    public static Save instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            LoadPlayer();
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SavePlayer()
    {
        SaveController.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        SaveData data = SaveController.LoadPlayer();

        if (data == null)
        {
            SavePlayer();
        }
        else
        {
            lastUnlockedFill = data.lastUnlockedMinigame[(int)SaveData.Minigame.FILL];
            lastUnlockedOnestroke = data.lastUnlockedMinigame[(int)SaveData.Minigame.ONESTROKE];
            lastUnlockedPipes = data.lastUnlockedMinigame[(int)SaveData.Minigame.PIPES];
            lastUnlockedSlidingTile = data.lastUnlockedMinigame[(int)SaveData.Minigame.SLIDINGTILE];

            lastUnlockedWorld = data.lastUnlockedWorld;
            lastUnlockedCombatLevel = data.lastUnlockedCombatLevel;
        }
    }
}
