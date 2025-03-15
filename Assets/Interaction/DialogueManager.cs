using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialog;
    public TextMeshProUGUI dialog_name;
    public GameObject interaction_panel;

    public float typingSpeed = 0.05f;
    public float dialogueSpeed = 1.5f;

    private int curr = 0;               // Current dialogue index
    private bool isTyping = false;      // Is the text being typed?
    private bool lineComplete = false;  // Has the current line finished typing?

    // Toggle for auto mode
    public Toggle autoToggle;
    public Image toggleBackground;
    private bool isAutoMode = false;

    // JSON Data Classes
    [System.Serializable]
    public class DialogueSet
    {
        public string condition;          // The condition key (e.g., "start", "forest", etc.)
        public List<string> dialogues;    // List of dialogues for this condition
    }

    [System.Serializable]
    public class NPCDialogue
    {
        public string name;                  // NPC name
        public List<DialogueSet> dialogueSets; // Dialogue sets for different conditions
    }

    [System.Serializable]
    public class DialogueData
    {
        public List<NPCDialogue> NPCs;      // All NPCs in the game
    }

    public DialogueData dialogueData;          // Store loaded JSON dialogue data
    public string jsonFileName = "DialogueData.json"; // Default JSON file name

    private NPCDialogue currentNPC;         // The currently active NPC
    private string currentCondition = "start"; // Default condition key

    // Start is called before the first frame update
    void Start()
    {
        interaction_panel.SetActive(false); // Hide interaction panel initially
        autoToggle.onValueChanged.AddListener(ToggleAutoMode);
        autoToggle.isOn = isAutoMode;

        LoadDialogueData(); // Load dialogues from JSON
    }

    // Load dialogue data from a JSON file
    private void LoadDialogueData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            dialogueData = JsonUtility.FromJson<DialogueData>(jsonData);
        }
        else
        {
            Debug.LogError("Dialogue JSON file not found at: " + filePath);
        }
    }

    // Start dialogue for a specific NPC
    public void StartDialogue(string npcName, string condition = "start")
    {
        currentNPC = dialogueData.NPCs.Find(npc => npc.name == npcName);
        if (currentNPC != null)
        {
            currentCondition = condition;

            // Find the dialogue set for the current condition
            DialogueSet dialogueSet = currentNPC.dialogueSets.Find(ds => ds.condition == currentCondition);
            if (dialogueSet != null && dialogueSet.dialogues.Count > 0)
            {
                interaction_panel.SetActive(true);
                curr = 0; // Reset dialogue index
                dialoguelines = dialogueSet.dialogues.ToArray();
                dialog_name.text = npcName; // Set NPC name in UI
                StartCoroutine(TypeDialogue());
            }
            else
            {
                Debug.LogWarning($"No dialogue found for condition '{currentCondition}' in NPC '{npcName}'.");
            }
        }
        else
        {
            Debug.LogError("NPC not found: " + npcName);
        }
    }

    // Coroutine to type out the dialogue
    private IEnumerator TypeDialogue()
    {
        isTyping = true;  // Typing in progress
        lineComplete = false; // Line is not complete
        dialog.text = ""; // Clear the text

        foreach (char letter in dialoguelines[curr].ToCharArray())
        {
            dialog.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;  // Typing finished
        lineComplete = true; // Line is fully displayed

        if (isAutoMode)
        {
            yield return new WaitForSeconds(dialogueSpeed);
            if (!isTyping && lineComplete)
            {
                NextLine();
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Check for player input to speed up or skip
        if ((Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0)) && interaction_panel.activeSelf)
        {
            if (isTyping)
            {
                // Skip to full line if still typing
                StopAllCoroutines();
                dialog.text = dialoguelines[curr];
                isTyping = false;
                lineComplete = true;
            }
            else if (lineComplete)
            {
                NextLine();
            }
        }
    }

    // Move to the next line in the dialogue
    private void NextLine()
    {
        curr++; // Move to the next line

        if (curr < dialoguelines.Length)
        {
            StartCoroutine(TypeDialogue()); // Start typing the next line
        }
        else
        {
            interaction_panel.SetActive(false); // End the dialogue when done
            EndDialogue();
        }
    }

    // Toggle Auto Mode
    public void ToggleAutoMode(bool auto)
    {
        isAutoMode = auto;
        if (ColorUtility.TryParseHtmlString(auto ? "#E4E2B3" : "#FFFFFF", out Color newColor))
        {
            toggleBackground.color = newColor;
        }
        else
        {
            Debug.LogError("Invalid hex color code.");
        }
    }

    // Event triggered when the dialogue ends
    public delegate void DialogueEndHandler();
    public event DialogueEndHandler OnDialogueEnd;

    private void EndDialogue()
    {
        interaction_panel.SetActive(false); // Hide dialogue panel

        // Trigger the event when dialogue ends
        OnDialogueEnd?.Invoke();
    }

    // Dialogue lines for current NPC and condition
    private string[] dialoguelines;
}
