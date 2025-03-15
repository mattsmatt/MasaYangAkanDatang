using UnityEngine;
using UnityEngine.Playables;

public class CutsceneFirstRun : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayableDirector timeline;
    void Start()
    {
        if (!PlayerPrefs.HasKey("Cutscene"))
        {
            PlayerPrefs.SetInt("Cutscene", 1);
            PlayerPrefs.Save(); // Save changes to PlayerPrefs
            timeline.Play();
        } else if (PlayerPrefs.GetInt("Cutscene") == 2 && PlayerPrefs.GetInt("curr_scene") == 29) {
            PlayerPrefs.SetInt("Cutscene", 3);
            PlayerPrefs.Save(); // Save changes to PlayerPrefs
            timeline.Play();
        }
    }

    public void EndCutscene()
    {
        timeline.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerPrefs.GetInt("Cutscene") == 1)
            {
                PlayerPrefs.SetInt("Cutscene", 2);
                PlayerPrefs.Save(); // Save changes to PlayerPrefs
                timeline.Play();
            }
        }
    }
}
