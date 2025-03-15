using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGameManager : MonoBehaviour
{
    public AudioManager audioManager;

    public GameObject pauseCanvas;
    public GameObject pauseMenu;
    public GameObject settingsCanvas;

    public void OpenPauseMenu()
    {
        PlayMenuSFX();

        pauseMenu.SetActive(true);
        settingsCanvas.SetActive(false);
    }

    public void OpenSettings()
    {
        PlayMenuSFX();

        pauseMenu.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void PauseGame()
    {
        OpenPauseMenu();
        pauseCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        PlayMenuSFX();

        pauseCanvas.SetActive(false);
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void PlayMenuSFX()
    {
        if (audioManager != null)
            audioManager.PlaySFX(audioManager.OnMenuClick);
    }
}
