using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatGameManager : BaseGameManager
{
    public GameObject levelCompletePanel;
    public GameObject gameOverPanel;

    private void Awake()
    {
        audioManager = AudioManager.instance;

        if (CombatLevelManager.instance.currentLevel.levelNum == 5)
        {
            audioManager.PlayMusic(audioManager.combatBossBGM);
        }
        else
        {
            audioManager.PlayMusic(audioManager.combatBGM);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        pauseCanvas.SetActive(false);
        levelCompletePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void LevelComplete()
    {
        levelCompletePanel.SetActive(true);
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }


}
