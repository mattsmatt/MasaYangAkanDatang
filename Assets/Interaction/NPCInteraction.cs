using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    public Button interactButton;            // The virtual button, assign in the Inspector
    private bool isPlayerNearby = false;
    private bool isInteracting = false;
    public GameObject NPC;
    public string dialogCondition = "start";
    private Renderer npcRenderer;

    public DialogueManager dialogueManager;

    void Start()
    {
        interactButton.gameObject.SetActive(false);    // Hide button initially

        npcRenderer = NPC.GetComponent<Renderer>();

        // Link the button's OnClick event to the interaction method
        interactButton.onClick.AddListener(OnInteractButtonPressed);
    }

    public void OnInteractButtonPressed()
    {
        // Trigger interaction when button is pressed
        if (isPlayerNearby && !isInteracting)
        {
            isInteracting = true;
            interactButton.gameObject.SetActive(false);    // Hide button during interaction
            StartInteraction();
        }
    }

    void Update()
    {
        // Show prompt and button when player is nearby and not currently interacting
        if (isPlayerNearby && !isInteracting)
        {
            interactButton.gameObject.SetActive(true);
        }
        else if (!isPlayerNearby || isInteracting)
        {
            interactButton.gameObject.SetActive(false);    // Hide button when player leaves or interacting
        }
    }

    void StartInteraction()
    {
        // Change NPC color to green as a placeholder for interaction
        npcRenderer.material.color = Color.green;

        // Trigger the dialogue system
        dialogueManager.StartDialogue("bebek", dialogCondition);

        // Listen for when the dialogue ends to reset interaction
        dialogueManager.OnDialogueEnd += ResetInteraction; // Subscribe to event
    }

    void EndInteraction()
    {
        // Reset NPC color to white (idle state)
        npcRenderer.material.color = Color.white;

        // Reset interaction state
        isInteracting = false;

        // Allow re-interaction if the player is still nearby
        if (isPlayerNearby)
        {
            interactButton.gameObject.SetActive(true);
        }
    }

    void ResetInteraction()
    {
        // Unsubscribe from the dialogue end event to avoid repeated calls
        dialogueManager.OnDialogueEnd -= ResetInteraction;

        // End interaction to allow re-interaction
        EndInteraction();
    }

    // Detect player entering interaction range
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    // Detect player leaving interaction range
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            // If the player leaves during interaction, end it
            if (isInteracting)
            {
                EndInteraction();
            }
        }
    }
}
