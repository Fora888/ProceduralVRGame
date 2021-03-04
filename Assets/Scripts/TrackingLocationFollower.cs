using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Makes the ingame player hand follow the controller tracking position by applying the velocity according to the remaining distance to the hand
public class TrackingLocationFollower : MonoBehaviour
{
    public GameObject handTrackingLocation;
    public float maxHandVelocity = 10, maxHandAcceleration = 15;
    private Transform trackedHandTransform,inGameHandTransform;
    private Rigidbody inGameHandRigidbody;
    private Vector3 differenceVector,inGameHandPosition,trackedPosition;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
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
        speed = speed > maxHandVelocity ? maxHandVelocity : speed;
        inGameHandRigidbody.velocity = differenceVector.normalized * speed;
    }

    void matchAngles()
    {
        inGameHandTransform.rotation = trackedHandTransform.rotation;
    }
}
