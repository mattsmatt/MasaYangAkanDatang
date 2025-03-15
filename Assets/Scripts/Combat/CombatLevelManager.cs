using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombatLevelManager : BaseLevelSelector
{
    public static CombatLevelManager instance;

    public int worldNum;

    [System.Serializable]
    public class LevelData
    {
        public int levelNum;
        public List<EnemyData> enemies = new List<EnemyData>();
    }

    [System.Serializable]
    public class EnemyData
    {
        public string enemyName;
        public GameObject enemyObject;
    }

    public List<LevelData> levelData = new List<LevelData>();
    public LevelData currentLevel;

    public List<GameObject> heroList = new List<GameObject>();

    // for initialization
    private void Awake()
    {
        // check if instance exists
        if (instance == null)
        {
            // if not exist, set instance to this
            instance = this;
        }
        // check if instance is not this game object
        else if (instance != this)
        {
            // destroy this 
            Destroy(gameObject);
        }

        audioManager = AudioManager.instance;
        audioManager.PlayMusic(audioManager.levelManagerBGM);

        lvlButtons.AddRange(GameObject.FindGameObjectsWithTag("levelBtn"));
        lvlButtons.Sort((s1, s2) => s1.name.CompareTo(s2.name));
        SetUnlockedLevels();

        // game object won't be destroyed when switching scenes
        DontDestroyOnLoad(gameObject);
    }

    public void DestroyCurrentInstance()
    {
        Destroy(gameObject);
        instance = null;
        if (instance == null)
        {
            Debug.Log("instance is null");
        }
    }

    public override void OpenLevel(int levelNum)
    {
        audioManager.PlaySFX(audioManager.OnMenuClick);

        // set current level
        SetCurrentLevel(levelNum);

        // open combat scene
        SceneManager.LoadScene("Combat");
    }

    public void SetCurrentLevel(int levelNum)
    {
        currentLevel = levelData.Find(
                    delegate (LevelData lvl)
                    {
                        return lvl.levelNum == levelNum;
                    }
                );
    }

    public override void Back()
    {
        DestroyCurrentInstance();
        audioManager.PlaySFX(audioManager.OnMenuClick);
        SceneManager.LoadScene(PlayerPrefs.GetInt("curr_scene"));
    }

    public override void SetUnlockedLevels()
    {
        if (worldNum == Save.instance.lastUnlockedWorld)
        {
            for (int i = 0; i < lvlButtons.Count; i++)
            {
                if ((i + 1) > Save.instance.lastUnlockedCombatLevel)
                    lvlButtons[i].GetComponent<Button>().interactable = false;
            }
        }
        else if (worldNum < Save.instance.lastUnlockedWorld)
        {
            foreach (GameObject btn in lvlButtons)
            {
                btn.GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            foreach (GameObject btn in lvlButtons)
            {
                btn.GetComponent<Button>().interactable = false;
            }
        }

    }
}
