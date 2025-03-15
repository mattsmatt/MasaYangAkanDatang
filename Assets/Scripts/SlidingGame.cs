using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SlidingGame : MonoBehaviour
{
    private bool isPlayerNearby = false;
    private bool isInteracting = false;
    public Button interactButton;
    public SceneChangeHandler sceneChangeHandler;
    public TestNav testNav;
    public GameObject warning;
    public QuestManager questManager;
    void Start()
    {
        interactButton.gameObject.SetActive(false);
        interactButton.onClick.AddListener(OnInteractButtonPressed);
    }

    public void OnInteractButtonPressed()
    {
        if (isPlayerNearby && !isInteracting) {
            if (questManager.GetQuestById(10).isAvailable == true) {
                isInteracting = true;
                StartCoroutine(DisplayWarning());
                sceneChangeHandler.SavePosition();
                SceneManager.LoadScene("OnePuzzle");
                isInteracting = false;
            } else {
                StartCoroutine(CantGo());
            }
        } 
    }

    private IEnumerator CantGo() {
        warning.GetComponentInChildren<TMP_Text>().text = "Belum bisa berinteraksi!";
        warning.SetActive(true);
        yield return new WaitForSeconds(2f);
        warning.SetActive(false);
        warning.GetComponentInChildren<TMP_Text>().text = "Memulai permainan Arcade...";
        
    } 

    private IEnumerator DisplayWarning() {
        warning.SetActive(true);
        yield return new WaitForSeconds(4f);
        warning.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNearby && !isInteracting)
        {
            interactButton.GetComponentInChildren<TMP_Text>().text = "Play";
            interactButton.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            interactButton.gameObject.SetActive(false);
            interactButton.GetComponentInChildren<TMP_Text>().text = "Berinteraksi";
        }
    }
}
