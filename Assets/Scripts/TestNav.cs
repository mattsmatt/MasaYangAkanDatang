using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestNav : MonoBehaviour
{
    public SceneChangeHandler sceneChangeHandler;
    public void GoToFill()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.OnMenuClick);
        sceneChangeHandler.SavePosition();
        SceneManager.LoadScene("Fill - Level Manager");
    }
    public void GoToOneStroke()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.OnMenuClick);
        sceneChangeHandler.SavePosition();
        SceneManager.LoadScene("Onestroke - Level Manager");
    }
    public void GoToPipes()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.OnMenuClick);
        sceneChangeHandler.SavePosition();
        SceneManager.LoadScene("Pipes - Level Manager");
    }
    public void GoToCombatWorld1()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.OnMenuClick);
        sceneChangeHandler.SavePosition();
        SceneManager.LoadScene("World 1 - Level Manager");
    }
    public void GoToCombatWorld2()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.OnMenuClick);
        sceneChangeHandler.SavePosition();
        SceneManager.LoadScene("World 2 - Level Manager");
    }
    public void GoToCombatWorld3()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.OnMenuClick);
        sceneChangeHandler.SavePosition();
        SceneManager.LoadScene("World 3 - Level Manager");
    }

    public void GoToMenu()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.OnMenuClick);
        sceneChangeHandler.SavePosition();
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToSlidingTile()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.OnMenuClick);
        sceneChangeHandler.SavePosition();
        SceneManager.LoadScene("SlidingTilePuzzle");
    }
    public void GoToMap()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.OnMenuClick);
        sceneChangeHandler.SavePosition();
        SceneManager.LoadScene(28);
    }

    private void Awake()
    {
        AudioSource musicSource = AudioManager.instance.gameObject.transform.Find("Music").GetComponent<AudioSource>();
        if (musicSource.clip.name.CompareTo("MainMenu") != 0)
        {
            AudioManager.instance.PlayMusic(AudioManager.instance.menuBGM);
        }
        else if (!musicSource.isPlaying)
        {
            AudioManager.instance.PlayMusic(AudioManager.instance.menuBGM);
        }

    }
}
