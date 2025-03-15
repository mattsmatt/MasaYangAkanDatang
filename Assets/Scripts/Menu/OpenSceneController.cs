using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenSceneController : MonoBehaviour
{
    private AudioManager audioManager;
    public int sceneNumber;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("audioManager").GetComponent<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnButtonClick()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("previousScene" + sceneNumber, currentScene);
        SceneManager.LoadScene(sceneNumber);
    }
}
