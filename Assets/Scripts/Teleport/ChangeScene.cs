using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] GameObject Object;

    public void getDropDown()
    {
        if (Object.activeSelf == true)
        {
            Object.SetActive(false);
        }
        else
        {
            Object.SetActive(true);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Object.SetActive(true);
    }

    public void OnTriggerExit(Collider other)
    {
        Object.SetActive(false);
    }

}
