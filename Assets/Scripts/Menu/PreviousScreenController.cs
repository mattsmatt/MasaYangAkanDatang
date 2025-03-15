using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreviousScreenController : MonoBehaviour
{
    private AudioManager audioManager;
    private int sceneIndex;
    private int sceneToOpen;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("audioManager").GetComponent<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (!PlayerPrefs.HasKey("previousScene" + sceneIndex))
        {
            PlayerPrefs.SetInt("previousScene" + sceneIndex, sceneIndex);
        }
        sceneToOpen = PlayerPrefs.GetInt("previousScene" + sceneIndex);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
