using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerWorld : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("curr_scene")) {
            PlayerPrefs.SetInt("curr_scene", 28);
            PlayerPrefs.Save();
        }
    }

}
