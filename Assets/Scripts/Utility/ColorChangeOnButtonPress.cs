using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class ColorChangeOnButtonPress : MonoBehaviour
{
    private GameObject vrRig;
    private Transform thisTransform;
    private InputHandler inputHandler;
    private Material material;
    private new Renderer renderer;
    public SteamVR_Action_Boolean Grip;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        material = renderer.material;
        thisTransform = GetComponent<Transform>();
        vrRig = thisTransform.root.gameObject;
        inputHandler = vrRig.GetComponent<InputHandler>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Grip.GetState(SteamVR_Input_Sources.Any) == true)
        {
            material.color = Color.green;
        }
        else
        {
            material.color = Color.white;
        }
    }
}
