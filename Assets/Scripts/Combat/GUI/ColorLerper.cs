using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ColorLerper : MonoBehaviour
{
    private MeshRenderer cubeMeshRenderer;
    [SerializeField]
    public GameObject avatar;

    private Color startColor;
    public Color endColor;

    // Use this for initialization
    void Start()
    {
        // cubeMeshRenderer = this.GetComponent<MeshRenderer>();
        // startColor = cubeMeshRenderer.material.color;
    }

    public void ChangeColor()
    {
        avatar.SetActive(false);
        // cubeMeshRenderer.material.color = Color.Lerp(startColor, endColor, 2.5f);
    }

    public void ReverseColor()
    {
        avatar.SetActive(true);
        // cubeMeshRenderer.material.color = Color.Lerp(endColor, startColor, 2.5f);
    }
}
