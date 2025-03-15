using System;
using System.Collections;
using BayatGames.SaveGameFree;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SceneChangeHandler : MonoBehaviour
{
    // Reference to the player's Transform
    public Transform player;
    public int sid;
    public GameObject warning;
    

    void Start()
    {
        PlayerPrefs.SetInt("curr_scene", sid);
        PlayerPrefs.Save();
        LoadPosition();
        StartCoroutine(Teleporting());
    }

    private IEnumerator Teleporting() {
        if (sid == 28) {
            warning.GetComponentInChildren<TMP_Text>().text = "LABORATORIUM AIRA";
        } else if (sid == 29) {
            warning.GetComponentInChildren<TMP_Text>().text = "KOTA GAGUNA";
        }
        
        warning.SetActive(true);
        yield return new WaitForSeconds(2f);
        warning.SetActive(false);

        if (sid == 28) {
            warning.GetComponentInChildren<TMP_Text>().text = "Kamu tidak bisa teleport sekarang";
        } else if (sid == 29) {
            warning.GetComponentInChildren<TMP_Text>().text = "Memulai permainan Arcade...";
        }
    }

    void OnApplicationQuit()
    {
        SavePosition();
    }

    void OnDisable()
    {
        SavePosition();
    }

    public void SavePosition()
    {
        if (player != null)
        {
            Vector3 playerPosition = player.position;
            SaveGame.Save("PlayerPosition" + sid.ToString() , playerPosition);
            Debug.Log("Player position saved!");
        }
    }

    public void LoadPosition()
    {
        if (SaveGame.Exists("PlayerPosition"+ sid.ToString() ))
        {
            Vector3 savedPosition = SaveGame.Load<Vector3>("PlayerPosition"+ sid.ToString());
            player.position = savedPosition;
            Debug.Log("Player position loaded using SaveGame!");
        } else {
            SavePosition();
        }
    }
}
