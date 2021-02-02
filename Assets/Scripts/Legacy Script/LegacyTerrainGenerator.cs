using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Profiling;

public class LegacyTerrainGenerator : MonoBehaviour
{

    public float noiseScale = 0.02f,noiseSize = 0.03f;
    private List<GameObject> chunks;
    private static int chunkDimensions = 64,texelPerMeter = 2;
    private Transform worldTransform;
    public Material terrainMaterial;
    public int seed;
    private int xseed, yseed;
    // Start is called before the first frame update
    void Start()
    {        
        xseed = (int)(seed / 46340.95000105199);
        yseed = (int)(seed % 46340.95000105199);        
        Debug.Log(xseed);
        Debug.Log(yseed);
        AssetDatabase.ImportAsset("Assets/Materials/TerrainMaterial.mat");
        chunks = new List<GameObject>();
        worldTransform = GetComponent<Transform>();
        for(int n = -5; n<5;n++)
        {
            for (int m = -5; m < 5; m++)
            {
                createNewChunk(new Vector2(n, m));
            }
        }
    }

    // Update is called once per frame
    void OnValidate()
    {
        if (Application.isPlaying)
        {
            foreach(GameObject go in chunks)
            {
                go.GetComponent<Terrain>().terrainData.SetHeights(0, 0, GenerateHeights(new Vector2(go.transform.position.x,go.transform.position.z)));
            }
        }
    }
    float[,] GenerateHeights (Vector2 currentChunkCoordinates)
    {
        float[,] heights = new float[chunkDimensions * texelPerMeter + 1, chunkDimensions * texelPerMeter + 1];
        for(int x = 0; x< chunkDimensions * texelPerMeter + 1; x++)
        {
            for (int y = 0; y < chunkDimensions * texelPerMeter + 1; y++)
            {
                heights[y, x] = Mathf.PerlinNoise((x + ((currentChunkCoordinates.x + (150000 + 300000 * xseed)) * texelPerMeter))  * noiseSize, (y + ((currentChunkCoordinates.y + (150000 + 300000 * yseed)) * texelPerMeter)) * noiseSize) * noiseScale;
            }        
        }        
        return heights;
    }

    void createNewChunk(Vector2 currentChunk)
    {
        chunks.Add(Terrain.CreateTerrainGameObject(new TerrainData()));        
        Terrain terrain = chunks[chunks.Count - 1].GetComponent<Terrain>();
        terrain.transform.parent = worldTransform;
        terrain.transform.localPosition = new Vector3(currentChunk.x * chunkDimensions, 0, currentChunk.y * chunkDimensions);
        TerrainData terrainData = terrain.terrainData;
        terrainData.heightmapResolution = chunkDimensions * texelPerMeter + 1;
        terrainData.size = new Vector3(chunkDimensions, 256, chunkDimensions);
        terrainData.SetHeights(0, 0, GenerateHeights(currentChunk * chunkDimensions));
        terrain.materialTemplate = terrainMaterial;
        terrain.heightmapPixelError = 1;  
        
    }
}
