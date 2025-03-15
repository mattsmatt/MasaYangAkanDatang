using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using VIDE_Data;
using UnityEngine.UI;
using TMPro;

public class UIManagera : MonoBehaviour
{
    public GameObject container_NPC;
    public GameObject container_Player;
    public TextMeshProUGUI text_NPC;
    public TextMeshProUGUI name_NPC;
    public TextMeshProUGUI[] text_Choices;

    // button logic
    public Button interactButton;
    private bool isPlayerNearby = false;
    private bool isInteracting = false;
    private bool isChoiceActive = false;
    public GameObject WorldObject;
    private Renderer ObjectRenderer;

    void Start()
    {
        container_NPC.SetActive(false);
        container_Player.SetActive(false);

        interactButton.gameObject.SetActive(false);
        ObjectRenderer = WorldObject.GetComponent<Renderer>();

        interactButton.onClick.AddListener(OnInteractButtonPressed);
    }

    void Update()
    {
        if (isPlayerNearby && !isInteracting)
        {
            interactButton.gameObject.SetActive(true);
        }

        
        // Check for advancing dialogue only if choices are NOT active
        
        if (VD.isActive && !isChoiceActive && isInteracting && (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0)))
        {
            VD.Next();
        }
        
    }

      void ActionHandler(int actionNodeID)
    {
        Debug.Log("ACTION TRIGGERED: " + actionNodeID.ToString());
        switch (actionNodeID) {
            case 1:
                Debug.Log("action 1 rawr");
                break;

            case 5:
                Debug.Log("action 5 rawr");
                break;

            default :
                Debug.Log("idk");
                break;
        }
    }

    void Begin()
    {
        // Subscribe to events
        VD.OnNodeChange += UpdateUI;
        VD.OnActionNode += ActionHandler;
        VD.OnEnd += End;
        

        ObjectRenderer.material.color = Color.green;

        // Start the dialogue
        VD.BeginDialogue(GetComponent<VIDE_Assign>());

        // Hide the interact button during interaction
        interactButton.gameObject.SetActive(false);
    }

    void UpdateUI(VD.NodeData data)
    {
        container_NPC.SetActive(false);
        container_Player.SetActive(false);
        isChoiceActive = false; // Reset choice flag

        if (data.isPlayer)
        {
            container_Player.SetActive(true);
            isChoiceActive = true; // Set flag since choices are active

            for (int i = 0; i < text_Choices.Length; i++)
            {
                if (i < data.comments.Length)
                {
                    text_Choices[i].transform.parent.gameObject.SetActive(true);
                    text_Choices[i].text = data.comments[i];
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
            name_NPC.text = data.tag.Length > 0 ? data.tag : VD.assigned.alias;
            text_NPC.text = data.comments[data.commentIndex];
        }
    }

    void End(VD.NodeData data)
    {
        // Ensure proper cleanup
        VD.OnNodeChange -= UpdateUI;
        VD.OnActionNode -= ActionHandler;
        VD.OnEnd -= End;

        VD.EndDialogue();

        isInteracting = false;
        ObjectRenderer.material.color = Color.white;

        if (isPlayerNearby)
        {
            interactButton.gameObject.SetActive(true);
        }

        container_NPC.SetActive(false);
        container_Player.SetActive(false);
    }

    public void SetPlayerChoice(int choice)
    {
        VD.nodeData.commentIndex = choice;

        VD.Next();
        isChoiceActive = false;

    }

    public void OnInteractButtonPressed()
    {
        if (isPlayerNearby && !isInteracting)
        {
            isInteracting = true;
            Begin();
        }
    }

    void OnDisable()
    {
        VD.OnNodeChange -= UpdateUI;
        VD.OnEnd -= End;
        VD.OnActionNode -= ActionHandler;

        if (container_NPC!= null)
        container_NPC.SetActive(false);
        if (container_Player!= null)
        container_Player.SetActive(false);
        VD.EndDialogue();

        if (VD.isActive)
        {
            End(null);
        }
    }

    public void AccessPuzzle() {
        Debug.Log("Loading Puzzle Game Scene...");
        SceneManager.LoadScene("SlidingTilePuzzle");
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
