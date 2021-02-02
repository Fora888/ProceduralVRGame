using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosDrawVertices : MonoBehaviour
{
    Color color = Color.white;
    float radius = .05f;


    void OnDrawGizmos()
    {
        Gizmos.color = color;
        var transform = this.transform;
        MeshFilter meshFilter = GetComponent<MeshFilter>();
    foreach (var vert in meshFilter.sharedMesh.vertices)
            Gizmos.DrawWireSphere(transform.TransformPoint(vert), radius);
    }
}
