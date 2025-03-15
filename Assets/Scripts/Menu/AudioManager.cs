using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    [Header("----------Audio Source----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("----------Music Clip----------")]
    public AudioClip menuBGM;
    public AudioClip combatBGM;
    public AudioClip combatBossBGM;
    public AudioClip puzzleBGM;
    public AudioClip levelManagerBGM;
    public AudioClip mapBGM;

    [Header("----------SFX Clip----------")]
    public AudioClip OnMenuClick;
    public AudioClip OnHeal;
    public AudioClip OnMeleeAtk;
    public AudioClip OnSkillAtk;
    public AudioClip OnGameOver;
    public AudioClip OnLevelComplete;
    public AudioClip OnInteract;
    public AudioClip OnAutoBattle;
    public AudioClip OnClickCombatMenu;
    public AudioClip OnDead;
    public AudioClip OnTalk;
    public AudioClip OnTalk2;
    public AudioClip OnTalk3;
    public AudioClip OnTalk4;
    public AudioClip OnTalk5;
    public AudioClip OnTalk6;

    public enum Status
    {
        MAP,
        IDLE
    };

    public Status isMapPlaying;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "aira" || SceneManager.GetActiveScene().name == "city")
        {
            switch (isMapPlaying)
            {
                case Status.MAP:
                    {
                        PlayMusic(mapBGM);
                        isMapPlaying = Status.IDLE;
                        break;
                    }
                case Status.IDLE:
                    {

                        break;
                    }
            }
        }
    }
}
