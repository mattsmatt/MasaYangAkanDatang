using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapPauseController : MonoBehaviour
{
    private MapGameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("MapGameManager").GetComponent<MapGameManager>();
    }

    public void OpenSettings()
    {
        gameManager.OpenSettings();
    }

    public void GoToMenu()
    {
        gameManager.PlayMenuSFX();
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }

    public void ResumeGame()
    {
        gameManager.ResumeGame();
    }
}
