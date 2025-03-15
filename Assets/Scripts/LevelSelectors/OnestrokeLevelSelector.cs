using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnestrokeLevelSelector : BaseLevelSelector
{
    public override void OpenLevel(int levelNum)
    {
        audioManager.PlaySFX(audioManager.OnMenuClick);
        string sceneName = "Onestroke - Level " + levelNum;
        SceneManager.LoadScene(sceneName);
    }

    private void Awake()
    {
        audioManager = AudioManager.instance;
        audioManager.PlayMusic(audioManager.levelManagerBGM);

        lvlButtons.AddRange(GameObject.FindGameObjectsWithTag("levelBtn"));
        lvlButtons.Sort((s1, s2) => s1.name.CompareTo(s2.name));
        SetUnlockedLevels();
    }

    public override void SetUnlockedLevels()
    {
        for (int i = 0; i < lvlButtons.Count; i++)
        {
            if ((i + 1) > Save.instance.lastUnlockedOnestroke)
                lvlButtons[i].GetComponent<Button>().interactable = false;
        }
    }
}
