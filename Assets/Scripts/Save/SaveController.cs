using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Serializers;
using Unity.VisualScripting;
using UnityEngine;

public class SaveController
{
    public static void SavePlayer(Save save)
    {
        SaveData data = new SaveData(save);
        SaveGame.Save<SaveData>("SaveData.dat", data);
    }

    public static SaveData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/SaveData.dat";
        if (File.Exists(path))
        {
            SaveData data = SaveGame.Load<SaveData>("SaveData.dat");
            return data;
        }
        else
        {
            Debug.Log("no save file");
            return null;
        }
    }
}
