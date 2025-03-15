using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAutoScroll : MonoBehaviour
{
    public float cameraScrollSpeed = 0.2f;
    public Vector3 startPosition;


    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(cameraScrollSpeed, 0.1f, 0f);
    }
}
