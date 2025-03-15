using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsController : MonoBehaviour
{
    private AudioManager audioManager;
    public GameObject settingsPanel, creditsPanel;

    private void Awake()
    {
        audioManager = AudioManager.instance;
    }

    private void Start()
    {
        settingsPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void GoToCredits()
    {
        if (audioManager != null)
            audioManager.PlaySFX(audioManager.OnMenuClick);

        ToggleViewCredits();
    }

    public void ReturnToSettings()
    {
        if (audioManager != null)
            audioManager.PlaySFX(audioManager.OnMenuClick);

        ToggleViewCredits();
    }

    private void ToggleViewCredits()
    {
        if (settingsPanel != null && creditsPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
            creditsPanel.SetActive(!creditsPanel.activeSelf);
        }
    }

    private void PrevScene()
    {
        if (audioManager != null)
            audioManager.PlaySFX(audioManager.OnMenuClick);

        if (creditsPanel.activeSelf)
        {
            ToggleViewCredits();
        }
    }

}
