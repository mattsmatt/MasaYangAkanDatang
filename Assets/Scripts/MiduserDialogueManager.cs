using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGameFree;

public class MiduserDialogueManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Dictionary<string, int> npcprogress;
    void Start()
    {
        CheckNPCProgress();
    }

    public void CheckNPCProgress() {
        Dictionary<string, int> npcprogress = SaveGame.Load<Dictionary<string, int>>("NPCProgress");
        // locking combat
        if (PlayerPrefs.GetInt("LockCombat") == 1) {
            npcprogress["Miduser"] = 16;
            SaveGame.Save("NPCProgress", npcprogress);
        } else if (PlayerPrefs.GetInt("LockCombat") == 2) {
            npcprogress["Miduser"] = 4;
            SaveGame.Save("NPCProgress", npcprogress);
            PlayerPrefs.SetInt("LockCombat", 1);

            PlayerPrefs.SetInt("LockCombat", 1);
        } else {
            int x = PlayerPrefs.GetInt("Component");
            int currworld = Save.instance.lastUnlockedWorld;

            if (x == 1) {
                npcprogress["Miduser"] = 26;
            } else if (x == 2) {
                if (currworld == 1) {
                    npcprogress["Miduser"] = 26;
                } else {
                    npcprogress["Miduser"] = 28;
                }
            } else if (x == 3) {
                if (currworld == 1) {
                    npcprogress["Miduser"] = 26;
                } else if (currworld == 2) {
                    npcprogress["Miduser"] = 28;
                } else {
                    npcprogress["Miduser"] = 30;
                }
            } else if (x == 8) {
                npcprogress["Miduser"] = 15;
            }
            SaveGame.Save("NPCProgress", npcprogress);

            PlayerPrefs.SetInt("UnlockCombat", 0);
            PlayerPrefs.SetInt("LockCombat", 0);
            PlayerPrefs.Save();
        }

        // Unlock combat
        // if (PlayerPrefs.GetInt("UnlockCombat") == 1) {
        //     int x = PlayerPrefs.GetInt("Component");

        //     if (x == 1) {
        //         npcprogress["Miduser"] = 18;
        //     } else if (x == 2) {
        //         npcprogress["Miduser"] = 19;
        //     } else if (x == 3) {
        //         if (Save.instance.lastUnlockedWorld == 2) {
        //             npcprogress["Miduser"] = 19;
        //         } else {
        //             npcprogress["Miduser"] = 20;
        //         }
        //     } else if (x == 8) {
        //         npcprogress["Miduser"] = 15;
        //     }
        //     SaveNPCProgress();

        //     PlayerPrefs.SetInt("UnlockCombat", 0);
        //     PlayerPrefs.SetInt("LockCombat", 0);
        //     PlayerPrefs.Save();
        // }
    }
}
