using UnityEngine;
using UnityEngine.SceneManagement;
using VIDE_Data;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public GameObject container_NPC;
    public GameObject container_Player;
    public GameObject gameui;
    public TextMeshProUGUI text_NPC;
    public TextMeshProUGUI name_NPC;
    public TextMeshProUGUI[] text_Choices;

    public Button interactButton;
    private bool isPlayerNearby = false;
    private bool isInteracting = false;
    private bool isChoiceActive = false;

    public string saveKey = "NPCProgress";
    public Dictionary<string, int> npcprogress;

    // look at player
    // private Animator npcAnimator;
    public Transform playerTransform;
    public float rotationSpeed = 5f;
    public AudioManager audioManager;

    void Start()
    {
        Debug.Log("ui manager is starting help");
        audioManager = AudioManager.instance;
        container_NPC.SetActive(false);
        container_Player.SetActive(false);

        interactButton.gameObject.SetActive(false);
        // ObjectRenderer = WorldObject.GetComponent<Renderer>();

        interactButton.onClick.AddListener(OnInteractButtonPressed);

        // npcAnimator = GetComponentInParent<Animator>(); // Access the Animator from the parent
        // if (npcAnimator == null)
        // {
        //     Debug.LogError("Animator component missing on NPC parent.");
        // }

        LoadNPCProgress();
        SaveNPCProgress();
    }
    
    public void LoadNPCProgress() {
        npcprogress = SaveGame.Load<Dictionary<string, int>>(saveKey);
    }

    public void SaveNPCProgress() {
        SaveGame.Save(saveKey, npcprogress);
    }

    void Update()
    {
        if (isPlayerNearby && !isInteracting)
        {
            interactButton.gameObject.SetActive(true);
        }

        // Allow advancing dialogue only if choices are NOT active
        if (VD.isActive && !isChoiceActive && isInteracting && (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0)))
        {
            VD.Next();
        }

        // Make NPC look at player while interacting
        if (isInteracting)
        {
            LookAtPlayer();
        }
    }

    void LookAtPlayer()
    {
        if (playerTransform == null) return;

        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0; // Ignore vertical rotation
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    void Begin()
    {
        
        VD.OnNodeChange += UpdateUI;
        VD.OnActionNode += ActionHandler;
        VD.OnEnd += End;

        // ObjectRenderer.material.color = Color.green;

        // Set override start node based on dialogue progress

        VD.BeginDialogue(GetComponent<VIDE_Assign>());
        interactButton.gameObject.SetActive(false);
        gameui.SetActive(false);
        SetDialogueProgress();
    }

    void UpdateUI(VD.NodeData data)
    {
        container_NPC.SetActive(false);
        container_Player.SetActive(false);
        isChoiceActive = false;

        if (data.isPlayer)
        {
            container_Player.SetActive(true);
            isChoiceActive = true;

            for (int i = 0; i < text_Choices.Length; i++)
            {
                if (i < data.comments.Length)
                {
                    text_Choices[i].transform.parent.gameObject.SetActive(true);
                    text_Choices[i].text = data.comments[i];
                    text_Choices[i].transform.parent.GetComponent<Button>().onClick.RemoveAllListeners();
                    int choice = i;
                    text_Choices[i].transform.parent.GetComponent<Button>().onClick.AddListener(() => SetPlayerChoice(choice));
                }
                else
                {
                    text_Choices[i].transform.parent.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            container_NPC.SetActive(true);
            name_NPC.text = data.tag;
            text_NPC.text = data.comments[data.commentIndex];
            
            // Randomize the OnTalk audio
            AudioClip[] onTalkOptions = { audioManager.OnTalk, audioManager.OnTalk2, audioManager.OnTalk3, audioManager.OnTalk4, audioManager.OnTalk5, audioManager.OnTalk6  };
            AudioClip randomOnTalk = onTalkOptions[Random.Range(0, onTalkOptions.Length)];
            
            audioManager.PlaySFX(randomOnTalk);
        }
    }

    void End(VD.NodeData data)
    {
        VD.OnNodeChange -= UpdateUI;
        VD.OnActionNode -= ActionHandler;
        VD.OnEnd -= End;

        VD.EndDialogue();

        isInteracting = false;
        // ObjectRenderer.material.color = Color.white;

        if (isPlayerNearby)
        {
            interactButton.gameObject.SetActive(true);
        }

        gameui.SetActive(true);
        container_NPC.SetActive(false);
        container_Player.SetActive(false);

        // Save the game state
        // UpdateDialogueProgress();
    }

    void ActionHandler(int actionNodeID)
    {
        // Debug.Log("ACTION TRIGGERED: " + actionNodeID);

        // switch (actionNodeID)
        // {
        //     case 5:
        //         Debug.Log("Transitioning to Puzzle");
        //         // AccessPuzzle();
        //         break;

        //     case 7:
        //         Debug.Log("Transitioning to Cutscene");
        //         break;


        //     default:
        //         // Debug.Log("Unhandled action ID");
        //         break;
        // }
    }

    public void UpdateDialogueProgress(int nodeid)
    {
        LoadNPCProgress();
        string npcKey = GetComponent<VIDE_Assign>().assignedDialogue;
        if (npcprogress.ContainsKey(npcKey))
        {
            npcprogress[npcKey] = nodeid;
        } else {
            npcprogress.Add(npcKey, nodeid);
            Debug.Log($"NPC '{npcKey}' progress updated to node {nodeid}");
        }

        SaveNPCProgress(); // Save after updating
        // Debug.Log($"NPC '{npcKey}' progress updated to node {nodeid}");
    }



    void SetDialogueProgress()
    {
        LoadNPCProgress();
        string npcKey = GetComponent<VIDE_Assign>().assignedDialogue;

        if (npcprogress.ContainsKey(npcKey))
        {
            int savedProgress = npcprogress[npcKey];
            VD.SetNode(savedProgress); // Set dialogue start node
            Debug.Log($"Set dialogue for NPC '{npcKey}' to start at node {savedProgress}");
        }
        else
        {
            Debug.Log($"No saved progress for NPC '{npcKey}'. Starting from the beginning.");
        }
    }

    public void SetPlayerChoice(int choice)
    {
        // Debug.Log("inside setting " + choice);
        VD.nodeData.commentIndex = choice;
        VD.Next();
        isChoiceActive = false;
    }

    public void OnInteractButtonPressed()
    {
        if (isPlayerNearby && !isInteracting)
        {
            isInteracting = true;

            // if (npcAnimator != null)
            // {
            //     npcAnimator.SetTrigger("Interact"); // Ensure the trigger exists in the Animator
            // }

            Begin();

            // StartCoroutine(ResetTrigger("Interact"));
        }
    }

    // private IEnumerator ResetTrigger(string triggerName)
    // {
    //     yield return null; // Wait for one frame
    //     npcAnimator.ResetTrigger(triggerName);
    // }

    void OnDisable()
    {
        VD.OnNodeChange -= UpdateUI;
        VD.OnEnd -= End;
        VD.OnActionNode -= ActionHandler;

        if (container_NPC != null) container_NPC.SetActive(false);
        if (container_Player != null) container_Player.SetActive(false);
        VD.EndDialogue();
    }

    public void AccessPuzzle()
    {

        Debug.Log("Loading Puzzle Game Scene...");
        PlayerPrefs.SetInt("PuzzleCompleted", 0); // Set as incomplete
        SceneManager.LoadScene("SlidingTilePuzzle");
    }

    public void AccessCutscene()
    {

        Debug.Log("Loading Cut Scene...");
        SceneManager.LoadScene("Cutscene awal");
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

            if (isInteracting)
            {
                End(null);
            }

            interactButton.gameObject.SetActive(false);
        }
    }
}
