using System;
using BayatGames.SaveGameFree;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteController : MonoBehaviour
{
    private AudioManager audioManager;

    private int worldNum, levelNum;

    [SerializeField]
    public bool isGameOver = false;

    private void Awake()
    {
        audioManager = AudioManager.instance;
    }

    void Start()
    {
        if (CombatLevelManager.instance != null)
        {
            worldNum = CombatLevelManager.instance.worldNum;
            levelNum = CombatLevelManager.instance.currentLevel.levelNum;
        }

        if (!isGameOver)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (!sceneName.Contains("SlidingTilePuzzle"))
            {
                audioManager.PlaySFX(audioManager.OnLevelComplete);
                SaveGameData();
            }

        }
        else
        {
            audioManager.PlaySFX(audioManager.OnGameOver);
        }
    }

    public void CheckItem() {
        // SaveGame.Save("");
    }

    private void SaveGameData()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.Contains("Fill"))
        {
            if (Save.instance.lastUnlockedFill < 5 && Int32.Parse(sceneName.Substring(sceneName.Length - 1)) == Save.instance.lastUnlockedFill)
            {
                Save.instance.lastUnlockedFill += 1;
            }
        }
        else if (sceneName.Contains("Onestroke"))
        {
            if (Save.instance.lastUnlockedOnestroke < 5 && Int32.Parse(sceneName.Substring(sceneName.Length - 1)) == Save.instance.lastUnlockedOnestroke)
            {
                Save.instance.lastUnlockedOnestroke += 1;
            }
        }
        else if (sceneName.Contains("Pipes"))
        {
            if (Save.instance.lastUnlockedPipes < 5 && Int32.Parse(sceneName.Substring(sceneName.Length - 1)) == Save.instance.lastUnlockedPipes)
            {
                Save.instance.lastUnlockedPipes += 1;
            }
        }
        else if (sceneName.Contains("Combat"))
        {

            if (levelNum == Save.instance.lastUnlockedCombatLevel && worldNum == Save.instance.lastUnlockedWorld)
            {
                if (Save.instance.lastUnlockedCombatLevel == 1 && Save.instance.lastUnlockedWorld == 1) {
                    // if no item
                    Save.instance.lastUnlockedCombatLevel += 1;
                    if (PlayerPrefs.GetInt("Component") == 0) {
                        PlayerPrefs.SetInt("LockCombat", 2);
                        PlayerPrefs.Save();
                    }
                }
                else if (Save.instance.lastUnlockedCombatLevel < 5)
                    Save.instance.lastUnlockedCombatLevel += 1;
                else
                {
                    if (Save.instance.lastUnlockedWorld < 3)
                    {
                        if (Save.instance.lastUnlockedWorld >= PlayerPrefs.GetInt("Component")) {
                            // update questmanager
                            PlayerPrefs.SetInt("LockCombat", 1);
                            PlayerPrefs.Save();
                        } else {
                            // PlayerPrefs.SetInt("UnlockCombat", 1);
                            // PlayerPrefs.Save();
                        }

                        Save.instance.lastUnlockedWorld += 1;
                        Save.instance.lastUnlockedCombatLevel = 1;
                    } else {
                        PlayerPrefs.SetInt("Component", 8);
                        PlayerPrefs.Save();
                    }
                }
            }


        }

        SaveController.SavePlayer(Save.instance);
    }

    public void GoToLevelSelect()
    {
        PlaySFX();

        string sceneName = SceneManager.GetActiveScene().name;

        DestroyLevelManagerInstance();

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
        else if (sceneName.Contains("Combat"))
        {
            GoToCombatLevelManager();
        }
        else if (sceneName.Contains("SlidingTilePuzzle"))
        {
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(buildIndex);
        }
    }

    public void GameFinished() {
        PlaySFX();

        string sceneName = SceneManager.GetActiveScene().name;

        DestroyLevelManagerInstance();

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
        else if (sceneName.Contains("Combat"))
        {
            GoToCombatLevelManager();
        }
        else if (sceneName.Contains("SlidingTilePuzzle"))
        {
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(buildIndex);
        }
    }

    private void GoToCombatLevelManager()
    {
        if (worldNum == 1)
        {
            // Debug.Log("here after first firght " + Save.instance.lastUnlockedCombatLevel);
            if (Save.instance.lastUnlockedCombatLevel == 2) {
                SceneManager.LoadScene("aira");
            } else {
                SceneManager.LoadScene("World 1 - Level Manager");
            }
        }
        else if (worldNum == 2)
        {
            SceneManager.LoadScene("World 2 - Level Manager");
        }
        else if (worldNum == 3)
        {
            SceneManager.LoadScene("World 3 - Level Manager");
        }
        else
        {
            Debug.Log("Go to level select");
        }
    }

    private void DestroyLevelManagerInstance()
    {
        if (CombatLevelManager.instance != null)
        {
            CombatLevelManager.instance.DestroyCurrentInstance();
        }

    }

    public void OnRetry()
    {
        PlaySFX();
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(buildIndex);
        Debug.Log("Game resetted!");
    }

    // public void GameFinished()
    // {
    //     PlaySFX();
    //     string sceneName = SceneManager.GetActiveScene().name;
    //     Debug.Log("Current scene: " + sceneName);

    //     if (sceneName.Contains("Level 5"))
    //     {
    //         DestroyLevelManagerInstance();
    //         if (sceneName.Contains("Fill"))
    //         {
    //             SceneManager.LoadScene("Fill - Level Manager");
    //         }
    //         else if (sceneName.Contains("Onestroke"))
    //         {
    //             SceneManager.LoadScene("Onestroke - Level Manager");
    //         }
    //         else if (sceneName.Contains("Pipes"))
    //         {
    //             SceneManager.LoadScene("Pipes - Level Manager");
    //         }
    //     }
    //     else
    //     {
    //         if (sceneName.Contains("Fill"))
    //         {
    //             SceneManager.LoadScene("Fill - Level " + (Int32.Parse(sceneName.Substring(sceneName.Length - 1)) + 1));
    //         }
    //         else if (sceneName.Contains("Onestroke"))
    //         {
    //             SceneManager.LoadScene("Onestroke - Level " + (Int32.Parse(sceneName.Substring(sceneName.Length - 1)) + 1));
    //         }
    //         else if (sceneName.Contains("Pipes"))
    //         {
    //             SceneManager.LoadScene("Pipes - Level " + (Int32.Parse(sceneName.Substring(sceneName.Length - 1)) + 1));
    //         }
    //         else if (sceneName.Contains("Combat"))
    //         {
    //             // if combat is at level 5
    //             if (levelNum == 5)
    //             {
    //                 DestroyLevelManagerInstance();

    //                 // go to level manager
    //                 GoToCombatLevelManager();
    //             }
    //             else
    //             {
    //                 // insert code to set next level enemies and settings
    //                 CombatLevelManager.instance.SetCurrentLevel(levelNum + 1);
    //                 int buildIndex = SceneManager.GetActiveScene().buildIndex;
    //                 SceneManager.LoadScene(buildIndex);

    //                 Debug.Log("Go to combat next level");
    //             }
    //         }
    //     }
    // }

    public void PlaySFX()
    {
        if (audioManager != null)
            audioManager.PlaySFX(audioManager.OnMenuClick);
    }
}
