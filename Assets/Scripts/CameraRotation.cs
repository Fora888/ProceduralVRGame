using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    private Transform transformCamera;
    public float mouseSensitivity = 1000.0f;
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        transformCamera = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        rotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        Quaternion localRotation = Quaternion.Euler(rotationY, rotationX, 0.0f);
        transformCamera.rotation = localRotation;
    }
}
