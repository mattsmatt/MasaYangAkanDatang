using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using VIDE_Data; // Import VIDE for dialogue management
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class CutsceneDialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogueContainer; // Parent container for dialogue UI
    public TextMeshProUGUI npcText; // NPC's dialogue text
    public TextMeshProUGUI npcLabel; // NPC's name label
    private CanvasGroup dialogueCanvasGroup; // CanvasGroup for fading effect

    public GameObject interactbutton;

    [Header("Cutscene Settings")]
    public PlayableDirector timelineDirector; // Timeline to control cutscene flow
    public VIDE_Assign dialogueAssign; // The VIDE dialogue to play
    public float fadeDuration = 1f; // Duration of fade-in effect

    private bool isFading = false;
    public Material nightSkybox;

    void Start()
    {
        // Initialize dialogue UI
        dialogueContainer.SetActive(false);

        // Add CanvasGroup for fade effect if not already present
        dialogueCanvasGroup = dialogueContainer.GetComponent<CanvasGroup>();
        if (dialogueCanvasGroup == null)
        {
            dialogueCanvasGroup = dialogueContainer.AddComponent<CanvasGroup>();
        }
        dialogueCanvasGroup.alpha = 1f; // Start invisible
    }

    public void HideDialogue(int n) {
        if (n == 0) {
            // Debug.Log("fading out");
            StopAllCoroutines();
            StartCoroutine(FadeOutDialogue());
            dialogueContainer.SetActive(false);
        } else if (n == 1) {
            StopAllCoroutines();
            StartCoroutine(FadeInDialogue());
            dialogueContainer.SetActive(true);
        } else {
            OnDisable();
        }
    }

    public void BacktoGame()
    {
        Debug.Log("Loading Cut Scene...");
        PlayerPrefs.SetInt("FirstRun", 0);
        AudioManager.instance.PlayMusic(AudioManager.instance.mapBGM);
        SceneManager.LoadScene("aira");
    }

    public void StopCutscene()
    {
        timelineDirector.Stop();
    }

    public void SetNightSkybox()
    {
        RenderSettings.skybox = nightSkybox;
        DynamicGI.UpdateEnvironment(); // Updates global illumination
    }

    public void StartCutsceneDialogue()
    {
        // Begin cutscene dialogue
        Debug.Log("Starting cutscene dialogue...");
        dialogueContainer.SetActive(true);
        interactbutton.SetActive(false);

        // Subscribe to VIDE events
        VD.OnNodeChange += UpdateUI;
        VD.OnEnd += EndCutsceneDialogue;

        // Begin the VIDE dialogue
        VD.BeginDialogue(dialogueAssign);

        // Pause the timeline while the dialogue runs
        // if (timelineDirector != null) timelineDirector.Pause();
    }

    private void UpdateUI(VD.NodeData data)
    {
        // Reset UI states
        npcText.text = "";
        npcLabel.text = "";

        if (!data.isPlayer) // Handle NPC nodes
        {
            npcLabel.text = string.IsNullOrEmpty(data.tag) ? dialogueAssign.alias : data.tag;
            npcText.text = data.comments[data.commentIndex];

            // Start fade-in effect
            StopAllCoroutines();
            StartCoroutine(FadeInDialogue());
        }
    }

    private IEnumerator FadeInDialogue()
    {
        isFading = true;

        dialogueCanvasGroup.alpha = 0f; // Start fully transparent
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            dialogueCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        dialogueCanvasGroup.alpha = 1f; // Ensure full opacity
        isFading = false;
    }

    public void SkipOrAdvance()
    {
        if (isFading)
        {
            // Skip fade-in and set full opacity
            StopAllCoroutines();
            dialogueCanvasGroup.alpha = 1f;
            isFading = false;
        }
        else
        {
            // Advance to the next dialogue node
            VD.Next();
        }
    }

    public void EndCutsceneDialogue(VD.NodeData data)
    {
        // Unsubscribe from VIDE events
        VD.OnNodeChange -= UpdateUI;
        VD.OnEnd -= EndCutsceneDialogue;

        // End the dialogue session
        VD.EndDialogue();

        // Start fade-out effect
        StartCoroutine(FadeOutDialogue());
    }

    private IEnumerator FadeOutDialogue()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            dialogueCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        dialogueCanvasGroup.alpha = 0f; // Ensure fully transparent
        dialogueContainer.SetActive(false);

        // Resume the timeline
        // if (timelineDirector != null) timelineDirector.Resume();

        // Debug.Log("Cutscene dialogue ended.");
    }

    public void OnDisable()
    {
        // Ensure clean-up to avoid errors
        interactbutton.SetActive(true);
        dialogueCanvasGroup.alpha = 1f;
        dialogueContainer.SetActive(false);
        VD.OnNodeChange -= UpdateUI;
        VD.OnEnd -= EndCutsceneDialogue;
        VD.EndDialogue();

        if(PlayerPrefs.GetInt("Component") == 8) {
            PlayerPrefs.DeleteKey("FirstRun");
            PlayerPrefs.DeleteKey("Cutscene");
            PlayerPrefs.Save();
            SceneManager.LoadScene("MainMenu");
        }
    }
}
