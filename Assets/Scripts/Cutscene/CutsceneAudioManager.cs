using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneAudioManager : MonoBehaviour
{
    [Header("----------Audio Source----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    private void Awake()
    {
        musicSource.Stop();
    }
}
