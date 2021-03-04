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
    public GameObject tablet;
    public Vector3 tabletHoldingPosition, tabletHoldingRotation;
    private GameObject grabbedObject;
    private Rigidbody grabbedObjectRigidbody;
    private TerrainGenerator terrainGenerator;
    private new Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
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
            else
            {
                Collider[] inventoryCollider = Physics.OverlapSphere(transform.position, 0.2f, inventoryLayerMask);
                if (inventoryCollider.Length > 0)
                {
                    foreach (Collider collider in inventoryCollider)
                    {
                        if(collider.gameObject.name == "PlayerInventory")
                        {
                            tablet.SetActive(true);
                            grabbedObject = tablet;
                            grabbedObject.transform.parent = transform;
                            grabbedObject.transform.localPosition = tabletHoldingPosition * handDirection;
                            grabbedObject.transform.localEulerAngles = tabletHoldingRotation * handDirection;
                        }
                    }
                }
            }
        }
    }

    public void LetGo(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (grabbedObject == true)
        {
            if (grabbedObject != tablet)
            {
                Collider[] inventoryCollider = Physics.OverlapSphere(transform.position, 0.2f, inventoryLayerMask);
                if (inventoryCollider.Length > 0)
                {
                    if (inventoryCollider[0].gameObject.GetComponent<IInventory>().AddItem(grabbedObject))
                    {
                        Destroy(grabbedObject);
                    }
                }
                else
                {
                    Rigidbody grabbedObjectRigidbody = grabbedObject.AddComponent<Rigidbody>();
                    grabbedObjectRigidbody.velocity = rigidbody.velocity;
                    grabbedObjectRigidbody.angularVelocity = rigidbody.angularVelocity;
                    grabbedObject.transform.parent = terrainGenerator.generatedChunks[(int)terrainGenerator.relativePlayerPosition.x, (int)terrainGenerator.relativePlayerPosition.y].transform;
                    grabbedObject = null;
                    ;
                }
            }
            else
            {
                grabbedObject.transform.parent = terrainGenerator.generatedChunks[(int)terrainGenerator.relativePlayerPosition.x, (int)terrainGenerator.relativePlayerPosition.y].transform;
                grabbedObject = null;
            }
        }
    }
}
