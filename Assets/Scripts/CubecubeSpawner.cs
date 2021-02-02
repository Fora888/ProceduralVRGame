using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubecubeSpawner : MonoBehaviour
{
    private List<GameObject> cubes;
    public Vector3 startCorner, endCorner, currentSpawningPosition;
    public int cubeCount = 0;
    public float cubeSize,offset;
    // Start is called before the first frame update
    void Start()
    {
        currentSpawningPosition = startCorner;
        cubes = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("c"))
        {
            cubes.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
            cubes[cubes.Count-1].transform.position = currentSpawningPosition;
            cubes[cubes.Count-1].transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
            currentSpawningPosition.x = currentSpawningPosition.x + cubeSize-offset;
            cubeCount ++;
            if (currentSpawningPosition.x > endCorner.x)
            {
                currentSpawningPosition.x = startCorner.x;
                currentSpawningPosition.z = currentSpawningPosition.z + cubeSize;
            }
        }
    }
}
