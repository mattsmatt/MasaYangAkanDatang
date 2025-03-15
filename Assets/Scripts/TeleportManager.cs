using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeleportManager : MonoBehaviour
{
    private bool isPlayerNearby = false;
    private bool isInteracting = false;
    public Button interactButton;
    // Start is called before the first frame update

    public QuestManager questManager;

    public GameObject warning;
    public SceneChangeHandler sceneChangeHandler;
    void Start()
    {
        interactButton.gameObject.SetActive(false);
        interactButton.onClick.AddListener(OnInteractButtonPressed);
    }

    public void OnInteractButtonPressed()
    {
        if (isPlayerNearby && !isInteracting)
        {
            isInteracting = true;
            if (questManager.GetQuestById(5).isAvailable == true) {
                questManager.CompleteInteractQuest(5);
                sceneChangeHandler.SavePosition();
                StartCoroutine(Teleporting());
                SceneManager.LoadScene(29);
            } else {
                StartCoroutine(DisplayWarning());
            }
            isInteracting = false;
            
        }
    }

    private IEnumerator DisplayWarning() {
        warning.SetActive(true);
        yield return new WaitForSeconds(2f);
        warning.SetActive(false);
    }

    private IEnumerator Teleporting() {
        warning.GetComponentInChildren<TMP_Text>().text = "Teleporting...";
        warning.SetActive(true);
        yield return new WaitForSeconds(2f);
        warning.SetActive(false);
        warning.GetComponentInChildren<TMP_Text>().text = "Kamu tidak bisa teleport sekarang";
    }

    void Update()
    {
        if (isPlayerNearby && !isInteracting)
        {
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
        }
    }
}
