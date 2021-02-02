using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPositioning : MonoBehaviour
{
    private Transform thisTransform, cameraTransform;
    public float menuDistance = 1;


    //Positionier das Menu vor dem Spieler, sobald das Objekt aktiviert wird

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        thisTransform = GetComponent<Transform>();
        cameraTransform = FindNameFromTag.FindGameObjectFromTag(GameObject.FindGameObjectsWithTag("MainCamera"), "VRCamera").GetComponent<Transform>(); // nicht Optimal script reihenfolge muss verändert werden -> Menu Positioning vor Menu Caller
        thisTransform.position = new Vector3(cameraTransform.position.x + (cameraTransform.forward.x * menuDistance), cameraTransform.position.y, cameraTransform.position.z + (cameraTransform.forward.z * menuDistance));
        thisTransform.eulerAngles = new Vector3(transform.eulerAngles.x, cameraTransform.eulerAngles.y, transform.eulerAngles.z);
    }
}
