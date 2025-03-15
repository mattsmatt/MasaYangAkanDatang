using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    private CombatGameManager gameManager;
    private int worldNum;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("CombatGameManager").GetComponent<CombatGameManager>();
        if (CombatLevelManager.instance != null)
        {
            worldNum = CombatLevelManager.instance.worldNum;
        }
    }

    private void DestroyLevelManagerInstance()
    {
        if (CombatLevelManager.instance != null)
        {
            CombatLevelManager.instance.DestroyCurrentInstance();
        }
    }

    public void OpenSettings()
    {
        gameManager.OpenSettings();
    }

    public void GoToLevelSelect()
    {
        gameManager.PlayMenuSFX();
        Time.timeScale = 1f;

        DestroyLevelManagerInstance();

        if (worldNum == 1)
        {
            SceneManager.LoadScene("World 1 - Level Manager");
        }
        else if (worldNum == 2)
        {
            SceneManager.LoadScene("World 2 - Level Manager");
        }
        else if (worldNum == 3)
        {
            SceneManager.LoadScene("World 3 - Level Manager");
        }
    }

    public void ResetGame()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(buildIndex);
        Time.timeScale = 1f;
        gameManager.PlayMenuSFX();
        Debug.Log("Game resetted!");
    }

    public void ResumeGame()
    {
        gameManager.ResumeGame();
    }
}
