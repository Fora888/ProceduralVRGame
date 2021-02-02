using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Makes the ingame player hand follow the controller tracking position by applying the velocity according to the remaining distance to the hand
public class TrackingLocationFollower : MonoBehaviour
{
    public GameObject handTrackingLocation;
    private Transform trackedHandTransform,inGameHandTransform;
    private Rigidbody inGameHandRigidbody;
    private Vector3 differenceVector,inGameHandPosition,trackedPosition;
    private List<Vector3> collisionNormals;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        collisionNormals = new List<Vector3>();
        trackedHandTransform = handTrackingLocation.GetComponent<Transform>();
        inGameHandTransform = GetComponent<Transform>();
        inGameHandRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        inGameHandPosition = inGameHandTransform.position;
        trackedPosition = trackedHandTransform.position;
        differenceVector.x = trackedPosition.x - inGameHandPosition.x;
        differenceVector.y = trackedPosition.y - inGameHandPosition.y;
        differenceVector.z = trackedPosition.z - inGameHandPosition.z;

        calculateAndApplySpeed();
        matchAngles();
    }

    void calculateAndApplySpeed()
    {
        speed = differenceVector.magnitude / Time.deltaTime;
        inGameHandRigidbody.velocity = differenceVector.normalized * speed;
    }

    void matchAngles()
    {
        inGameHandTransform.rotation = trackedHandTransform.rotation;
    }
}
