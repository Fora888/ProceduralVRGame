using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AutoTiling : MonoBehaviour
{
    [SerializeField]
    private bool UpdateTile;
    public float scaleToTiles = 0.667f;
    private Renderer localrenderer;
    void OnValidate()
    {
        localrenderer = GetComponent<Renderer>();
        localrenderer.sharedMaterial.mainTextureScale = new Vector2(transform.lossyScale.x * scaleToTiles, transform.lossyScale.z * scaleToTiles);   
    }
}
