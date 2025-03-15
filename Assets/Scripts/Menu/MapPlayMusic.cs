using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayMusic : MonoBehaviour
{
    public AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("audioManager").GetComponent<AudioManager>();
        audioManager.isMapPlaying = AudioManager.Status.MAP;
    }
}
