//Handles the Interaction between Menus and controller. Draws the visualRay from the controller to the menu, aswell as Calling the Script from the Selected button

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class controllerMenuInteraction : MonoBehaviour
{
    private Transform visualRayTransform;
    private RaycastHit raycastHit;
    private GameObject visualRay;
    private int layerMask;
    private Material MenuRayCast;
    private bool isRightHand;
    public SteamVR_Action_Boolean trigger;
    // Start is called before the first frame update
    void Start()
    {
        MenuRayCast = Resources.Load("Materials/MenuRayCast", typeof(Material)) as Material;
        layerMask = LayerMask.GetMask("UI");
        visualRay = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        visualRay.GetComponent<MeshRenderer>().material = MenuRayCast;
        visualRayTransform = visualRay.GetComponent<Transform>();
        visualRay.GetComponent<Collider>().enabled = false;
        visualRay.SetActive(false);
        DeterminHand();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, transform.forward, out raycastHit, Mathf.Infinity,layerMask))
        {
            drawRaycast();
            if (isRightHand == true)
            {
                if (trigger.GetStateDown(SteamVR_Input_Sources.RightHand) == true)
                {
                    activateButtonAction();
                }
            }
            else
            {
                if (trigger.GetStateDown(SteamVR_Input_Sources.LeftHand) == true)
                {
                    activateButtonAction();
                }
            }

        }
        else
        {
            visualRay.SetActive(false);           
        }
    }

    void drawRaycast()
    {
        visualRay.SetActive(true);
        visualRayTransform.position = new Vector3((transform.position.x + raycastHit.point.x) / 2, (transform.position.y + raycastHit.point.y) / 2, (transform.position.z + raycastHit.point.z) / 2);
        visualRayTransform.localScale = new Vector3(0.01f, raycastHit.distance / 2, 0.01f);
        visualRayTransform.eulerAngles = new Vector3(-Mathf.Asin((raycastHit.point.y - transform.position.y) / raycastHit.distance) * Mathf.Rad2Deg - 90,  transform.eulerAngles.y, visualRayTransform.eulerAngles.z);
    }

    void activateButtonAction()
    {
        Transform hitTransform = raycastHit.transform;
        MenuButtonInterface buttonAction = hitTransform.GetComponent<MenuButtonInterface>();
        Debug.Log(buttonAction);
    }

    void DeterminHand()
    {
        Transform parentTransform = transform.parent;
        if(parentTransform.gameObject.name.Equals("right hand", StringComparison.OrdinalIgnoreCase))
        {
            isRightHand = true;
        }
    }
}
