using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HandItemInteractions : MonoBehaviour
{
    public SteamVR_Action_Boolean grabButton;
    public SteamVR_Input_Sources handType;
    private int handDirection = 1, itemLayerMask, inventoryLayerMask;
    public float grabDistance;
    private GameObject grabbedObject;
    private Rigidbody grabbedObjectRigidbody;
    private TerrainGenerator terrainGenerator;
    // Start is called before the first frame update
    void Start()
    {
        grabButton.AddOnStateDownListener(Grab, handType);
        grabButton.AddOnStateUpListener(LetGo, handType);
        if (handType == SteamVR_Input_Sources.RightHand)
        {
            handDirection = -1;
        }
        itemLayerMask = 1 << 11;
        inventoryLayerMask = 1 << 12;
        terrainGenerator = GameObject.FindWithTag("Terrain").GetComponent<TerrainGenerator>();
    }

    public void Grab(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (grabbedObject == null)
        {
            Debug.DrawLine(transform.position, transform.position + transform.right * handDirection * grabDistance);
            if (Physics.Raycast(transform.position, transform.right * handDirection, out RaycastHit grabHit, grabDistance, itemLayerMask))
            {
                grabbedObject = grabHit.transform.gameObject;
                grabbedObjectRigidbody = grabbedObject.GetComponent<Rigidbody>();
                if (grabbedObjectRigidbody == true)
                {
                    Destroy(grabbedObjectRigidbody);
                }
                grabbedObject.transform.parent = transform;
            }
        }
    }

    public void LetGo(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (grabbedObject == true)
        {
            Collider[] inventoryCollider = Physics.OverlapSphere(transform.position, 0.2f, inventoryLayerMask);
            if (inventoryCollider.Length > 0)
            {
                if(inventoryCollider[0].gameObject.GetComponent<IInventory>().AddItem(grabbedObject))
                {
                    Destroy(grabbedObject);
                }
            }
            else
            {
                grabbedObject.AddComponent<Rigidbody>();
                grabbedObject.transform.parent = terrainGenerator.generatedChunks[(int)terrainGenerator.relativePlayerPosition.x, (int)terrainGenerator.relativePlayerPosition.y].transform;
                grabbedObject = null;
            }
        }
    }
}
