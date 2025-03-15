using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    // Start is called before the first frame update
    void Start()
    {

        if (PlayerPrefs.HasKey("musicVolume"))
        {
            // Debug.Log("music: " + PlayerPrefs.GetFloat("musicVolume"));
            LoadMusic();
        }
        else
        {
            SetMusicVolume();
        }

        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            // Debug.Log("sfx: " + PlayerPrefs.GetFloat("sfxVolume"));
            LoadSfx();
        }
        else
        {
            SetSfxVolume();
        }
    }

    public void SetSfxVolume()
    {
        Debug.Log("Sfx volume: " + sfxSlider.value);
        float sfxVolume = sfxSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(sfxVolume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume()
    {
        Debug.Log("Music volume: " + musicSlider.value);
        float musicVolume = musicSlider.value;
        myMixer.SetFloat("Music", Mathf.Log10(musicVolume) * 20);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    private void LoadSfx()
    {
        Debug.Log("getting sfx vol: " + PlayerPrefs.GetFloat("sfxVolume"));
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        SetSfxVolume();
    }

    private void LoadMusic()
    {
        Debug.Log("getting music vol: " + PlayerPrefs.GetFloat("musicVolume"));
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SetMusicVolume();
    }
}
