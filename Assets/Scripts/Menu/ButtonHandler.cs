using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHandler : MonoBehaviour
{

    public void OnPointerDown(BaseEventData data)
    {
        // Debug.Log("on pointer down");
        data.selectedObject.transform.localScale += new Vector3(0.1f, 0.1f, 0);
    }

    public void OnPointerUp(BaseEventData data)
    {
        // Debug.Log("on pointer up");
        data.selectedObject.transform.localScale -= new Vector3(0.1f, 0.1f, 0);
    }
}
