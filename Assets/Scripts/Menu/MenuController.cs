using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
    private AudioManager audioManager;
    public GameObject settingsPanel, mainMenuPanel;
    public SettingsController settingsController;
    // public LoadSceneManager loadSceneManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("audioManager").GetComponent<AudioManager>();
        audioManager.PlayMusic(audioManager.menuBGM);
        // loadSceneManager = GameObject.FindFirstObjectByType<LoadSceneManager>();
    }

    public void PrevScene()
    {
        audioManager.PlaySFX(audioManager.OnMenuClick);

        if (settingsController.creditsPanel.activeSelf)
        {
            settingsController.ReturnToSettings();
        }
        else if (settingsPanel.activeSelf)
        {
            toggleViewSettings();
        }
    }

    public void NewGame()
    {
        audioManager.PlaySFX(audioManager.OnMenuClick);

        // Load game on map
        if (PlayerPrefs.GetInt("FirstRun") == 0)
        {
            SceneManager.LoadScene(PlayerPrefs.GetInt("curr_scene"));
        }
        else
        {
            audioManager.StopMusic();
            PlayerPrefs.SetInt("FirstRun", 0);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Cutscene awal");

        }

        Debug.Log("New Game");
    }

    public void ViewSettings()
    {
        audioManager.PlaySFX(audioManager.OnMenuClick);
        // Debug.Log("Opening Settings");
        toggleViewSettings();
    }

    public void ExitGame()
    {
        audioManager.PlaySFX(audioManager.OnMenuClick);
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    void toggleViewSettings()
    {
        if (settingsPanel != null && mainMenuPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
            mainMenuPanel.SetActive(!mainMenuPanel.activeSelf);
        }
    }
}
