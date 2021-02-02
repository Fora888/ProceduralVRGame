using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPlacer : MonoBehaviour
{
    public Material waterMaterial;
    public GameObject player;
    private TerrainGenerator terrainGenerator;
    private GameObject water;
    private Transform  waterTransform;
    private OriginShift originShift;
    void Start()
    {
        terrainGenerator = GetComponent<TerrainGenerator>();
        water = GameObject.CreatePrimitive(PrimitiveType.Plane);
        water.name = "Water";
        water.layer = 4;
        waterTransform = water.transform;
        waterTransform.localScale = new Vector3(terrainGenerator.chunkDimensions * (terrainGenerator.renderDistance * 2f + 1f)/10f, 1, terrainGenerator.chunkDimensions * (terrainGenerator.renderDistance * 2f + 1f)/10f);
        waterTransform.parent = gameObject.transform.parent;
        water.GetComponent<MeshRenderer>().sharedMaterial = waterMaterial;
        originShift = gameObject.GetComponentInParent<OriginShift>();
    }

    private void Update()
    {
        waterTransform.localPosition = new Vector3(player.transform.position.x, 0, player.transform.position.z);
        waterMaterial.SetFloat("Vector1_C0A5B226", -originShift.offset.x);
        waterMaterial.SetFloat("Vector1_8CAD2C00", -originShift.offset.y);
    }


}
