using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameMenuController : MonoBehaviour
{
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = AudioManager.instance;
    }

    public void OnExit()
    {
        PlaySFX();
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.Contains("Fill"))
        {
            SceneManager.LoadScene("Fill - Level Manager");
        }
        else if (sceneName.Contains("Onestroke"))
        {
            SceneManager.LoadScene("Onestroke - Level Manager");
        }
        else if (sceneName.Contains("Pipes"))
        {
            SceneManager.LoadScene("Pipes - Level Manager");
        }
    }

    public void OnRetry()
    {
        PlaySFX();
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(buildIndex);
        Debug.Log("Game resetted!");
    }

    public void PlaySFX()
    {
        if (audioManager != null)
            audioManager.PlaySFX(audioManager.OnMenuClick);
    }
}
