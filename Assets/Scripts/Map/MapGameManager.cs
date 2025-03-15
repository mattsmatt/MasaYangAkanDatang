using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGameManager : BaseGameManager
{
    private void Awake()
    {
        audioManager = AudioManager.instance;
        audioManager.PlayMusic(audioManager.mapBGM);
    }

    // Start is called before the first frame update
    void Start()
    {
        pauseCanvas.SetActive(false);
    }
}
