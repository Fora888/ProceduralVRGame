using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCubeSpawn : MonoBehaviour , MenuButtonInterface
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void MenuAction()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 1f, 0);
    }
}
