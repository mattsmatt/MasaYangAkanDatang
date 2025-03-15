using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensitivityController : MonoBehaviour
{
    [SerializeField] private Slider sensSlider;
    public GameObject touchZone;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("sensitivity"))
        {
            LoadSensitivity();
        }
        else
        {
            SetSensitivity();
        }
    }

    public void SetSensitivity()
    {
        Debug.Log("Sensitivity: " + sensSlider.value);
        float sensValue = sensSlider.value;
        PlayerPrefs.SetFloat("sensitivity", sensValue);
        PlayerPrefs.Save();

        if (touchZone != null)
        {
            touchZone.GetComponent<UIVirtualTouchZone>().SetSensitivity();
        }
    }

    private void LoadSensitivity()
    {
        Debug.Log("getting sensitivity: " + PlayerPrefs.GetFloat("sensitivity"));
        sensSlider.value = PlayerPrefs.GetFloat("sensitivity");
        SetSensitivity();
    }
}
