using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingChange : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject npc;
    public GameObject robot;
    public QuestManager questManager;
    public Light pointLight;
    public void ChangeMap() {
        npc.SetActive(false);
        robot.SetActive(false);
        pointLight.color = Color.green;
    }
}
