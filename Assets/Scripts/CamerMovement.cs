using UnityEngine;

public class CamerMovement : MonoBehaviour
{
    private Transform worldTransform, playerTransform, cameraTransform;
    public float normalSpeed = 1, runSpeed = 2;
    private float movementSpeed;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GetComponent<Transform>();
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("left shift"))
        {
            movementSpeed = runSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }

        if (Input.GetKey("w"))
        {
            playerTransform.position = playerTransform.position + (cameraTransform.forward * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey("a"))
        {
            playerTransform.position = playerTransform.position - (cameraTransform.right * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey("s"))
        {
            playerTransform.position = playerTransform.position - (cameraTransform.forward * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey("d"))
        {
            playerTransform.position = playerTransform.position + (cameraTransform.right * movementSpeed * Time.deltaTime);
        }
    }
}
