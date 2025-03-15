using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class BaseLevelSelector : MonoBehaviour
{
    public AudioManager audioManager;
    public List<GameObject> lvlButtons = new List<GameObject>();

    public abstract void OpenLevel(int levelNum);

    public virtual void Back()
    {
        audioManager.PlaySFX(audioManager.OnMenuClick);
        SceneManager.LoadScene(PlayerPrefs.GetInt("curr_scene"));
    }

    public abstract void SetUnlockedLevels();
}
