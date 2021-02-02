using UnityEngine;
using Valve.VR;

//Provides a non physically based character controller optimized for vr usage and based on Raycasts
public class VrMovement : MonoBehaviour
{
    public float movementSpeed = 10f, minPlayerHeight = 0.5f, stickDeadzone = 15f, stepHeight = 0.1f, minWalkAngle = 0.1f, fallSpeed = 5f;
    private Transform camTransform;
    private CapsuleCollider characterCollider;
    public SteamVR_Action_Vector2 joyStick;
    private int layerMask;
    public Vector2 test;
    // Start is called before the first frame update
    void Start()
    {
        camTransform = transform.Find("VRCamera").GetComponent<Transform>();
        characterCollider = GetComponent<CapsuleCollider>();
        layerMask = 1 << 8 | 1;
    }

    // Update is called once per frame
    void Update()
    {
        AdjustCollider();
        Gravity();
        MovePlayer();
    }

    //Adjust the ingame collider to the current real world standing heightof the player
    private void AdjustCollider()
    {
        characterCollider.height = camTransform.localPosition.y;
        if (characterCollider.height < minPlayerHeight)
        {
            characterCollider.height = minPlayerHeight;
        }
        characterCollider.center = new Vector3(camTransform.localPosition.x, characterCollider.height * 0.5f, camTransform.localPosition.z);
    }

    private void MovePlayer()
    {
        Vector2 direction;
        direction = joyStick.axis;
        //Elimination of stick drift
        if (direction.x <= stickDeadzone && direction.x >= -stickDeadzone)
        {
            direction.x = 0;
        }

        if (direction.y <= stickDeadzone && direction.y >= -stickDeadzone)
        {
            direction.y = 0;
        }

        //prevent the player from moving faster than he should
        if (direction.magnitude > 1)
        {
            direction = direction.normalized;
        }

        if (direction.magnitude > 0)
        {
            Vector2 normalizedForward = new Vector2(camTransform.forward.x, camTransform.forward.z).normalized;
            Vector2 normalizedRight = new Vector2(camTransform.right.x, camTransform.right.z).normalized;
            //How far a Raycast needs to go for that Frame
            float stepMagnitude = movementSpeed * Time.deltaTime * direction.magnitude + characterCollider.radius;

            //Casts a Ray horizontaly in the direction the Player wants to go to check wether the path is free of obstacles
            RaycastHit horizontalHit;
            Ray horizontalRay = new Ray(new Vector3(transform.position.x + characterCollider.center.x, transform.position.y - characterCollider.height * 0.5f + stepHeight, transform.position.z + characterCollider.center.z), new Vector3(normalizedForward.x * direction.y + normalizedRight.x * direction.x, 0, normalizedForward.y * direction.y + normalizedRight.y * direction.x));
            Physics.Raycast(horizontalRay, out horizontalHit, stepMagnitude, layerMask);
            //Debug.DrawLine(horizontalRay.origin, horizontalRay.origin + horizontalRay.direction * stepMagnitude);
            horizontalHit.distance = horizontalHit.collider == null ? stepMagnitude : horizontalHit.distance;
            if (horizontalHit.distance > characterCollider.radius)
            {
                //cast a vertical ray to check if there is a floor in reach and on which height it is
                RaycastHit verticalHit;
                Ray verticalRay = new Ray(horizontalRay.origin + (horizontalRay.direction.normalized * (horizontalHit.distance - characterCollider.radius)), Vector3.down);
                Physics.Raycast(verticalRay, out verticalHit, stepHeight, layerMask);
                //Debug.DrawLine(verticalRay.origin, verticalRay.origin + verticalRay.direction * stepHeight);
                verticalHit.point = verticalHit.collider == null ? transform.position + horizontalRay.direction.normalized * (stepMagnitude - characterCollider.radius) : new Vector3(verticalHit.point.x, verticalHit.point.y + characterCollider.height * 0.5f, verticalHit.point.z);
                if (verticalHit.normal.y > minWalkAngle || horizontalHit.collider == null)
                {
                    transform.position = verticalHit.point;
                }

            }
        }
    }

    //Casts a ray down from the center of the Collider to provide falling when the player isnt standing on anything, aswell as Correcting clipping issues with the player collider
    private void Gravity()
    {
        RaycastHit gravityHit;
        if (!Physics.Raycast(transform.position + characterCollider.center, Vector3.down, out gravityHit, fallSpeed * Time.deltaTime + characterCollider.height * 0.5f, layerMask))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - fallSpeed * Time.deltaTime, transform.position.z);
        }
        else
        {
            if(gravityHit.distance != characterCollider.height * 0.5f)
            {
                transform.position = new Vector3(transform.position.x, gravityHit.point.y, transform.position.z);
            }
        }

        if(transform.position.y < -100)
        {
            RaycastHit hit;
            Physics.Raycast(new Vector3(transform.position.x, 1000, transform.position.z), Vector3.down, out hit, Mathf.Infinity, layerMask);
            transform.position = new Vector3(hit.point.x, hit.point.y + 3, hit.point.z);
        }
    }
}
