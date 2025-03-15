using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Teleport : MonoBehaviour
{
    public Transform hospital;
    public Transform police_station;
    public Transform city_entrance;
    public Transform time_machine;
    public Transform future_room;
    public Transform game_room;
    public GameObject player;
    public SceneChangeHandler sceneChangeHandler;
    [SerializeField] TMP_Dropdown dropDown;
    [SerializeField] GameObject Object;

    public void MoveToPoint()
    {
        //city area teleport point
        Debug.Log(dropDown.options[dropDown.value].text);
        if (dropDown.options[dropDown.value].text == "Rumah Sakit")
        {
            transform.position = hospital.position;
            dropDown.value = 0;
            Object.SetActive(false);
        }

        else if (dropDown.options[dropDown.value].text == "Kantor Polisi")
        {
            transform.position = police_station.position;
            dropDown.value = 0;
            Object.SetActive(false);
        }

        else if (dropDown.options[dropDown.value].text == "Pinggir Kota")
        {
            transform.position = city_entrance.position;
            dropDown.value = 0;
            Object.SetActive(false);
        }

        else if (dropDown.options[dropDown.value].text == "Lab AIRA")
        {
            sceneChangeHandler.SavePosition();
            SceneManager.LoadScene("aira");
            dropDown.value = 0;
            Object.SetActive(false);
        }

        //aira labs teleport point

        // else if (dropDown.options[dropDown.value].text == "Time Machine")
        // {
        //     transform.position = time_machine.position;
        //     dropDown.value = 0;
        //     Object.SetActive(false);
        // }

        // else if (dropDown.options[dropDown.value].text == "Lobby Room")
        // {
        //     transform.position = future_room.position;
        //     dropDown.value = 0;
        //     Object.SetActive(false);
        // }

        // else if (dropDown.options[dropDown.value].text == "Simulated Combat")
        // {
        //     transform.position = game_room.position;
        //     dropDown.value = 0;
        //     Object.SetActive(false);
        // }

        // else if (dropDown.options[dropDown.value].text == "City")
        // {
        //     sceneChangeHandler.SavePosition();
        //     SceneManager.LoadScene("city");
        //     dropDown.value = 0;
        //     Object.SetActive(false);
        // }

    }


}
