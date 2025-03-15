using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    // minigame progress
    public enum Minigame
    {
        FILL,
        ONESTROKE,
        PIPES,
        SLIDINGTILE
    }
    public int[] lastUnlockedMinigame;

    // rpg progress
    public int lastUnlockedWorld;
    public int lastUnlockedCombatLevel;

    public SaveData(Save save)
    {
        lastUnlockedWorld = save.lastUnlockedWorld;
        lastUnlockedCombatLevel = save.lastUnlockedCombatLevel;

        lastUnlockedMinigame = new int[Enum.GetNames(typeof(Minigame)).Length];
        lastUnlockedMinigame[(int)Minigame.FILL] = save.lastUnlockedFill;
        lastUnlockedMinigame[(int)Minigame.ONESTROKE] = save.lastUnlockedOnestroke;
        lastUnlockedMinigame[(int)Minigame.PIPES] = save.lastUnlockedPipes;
        lastUnlockedMinigame[(int)Minigame.SLIDINGTILE] = save.lastUnlockedSlidingTile;
    }
}
