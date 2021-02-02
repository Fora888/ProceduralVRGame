using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

public class MultithreadedTerrainGenerator : MonoBehaviour
{
    public float noiseScale = 0.02f, noiseSize = 0.03f, heightOffsetNoiseSize = 0.003f;
    public int seed, renderDistance = 3;
    public Material terrainMaterial;
    [HideInInspector]
    public GameObject[,] generatedChunks = new GameObject[4688,4688];
    [HideInInspector, ReadOnly]
    public Vector2 relativePlayerPosition;
    public int chunkDimensions = 32, texelPerMeter = 1;
    private Transform worldTransform;
    private int xseed, yseed;
    private Transform playerTransform;  
    private List<JobHandle> jobHandles;
    private List<GenerationJob> jobs;

    // Start is called before the first frame update
    void Start()
    {
        worldTransform = GetComponent<Transform>();
        jobHandles = new List<JobHandle>();
        jobs = new List<GenerationJob>();
        playerTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        xseed = (int)(seed / 46340.95000105199);
        yseed = (int)(seed % 46340.95000105199);
        relativePlayerPosition = new Vector2((int)(playerTransform.position.x / chunkDimensions + 2344), (int)(playerTransform.position.z / chunkDimensions + 2344));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckForEmptyChunks();
        SyncTerrainData();
    }

    private void OnApplicationQuit()
    {
        jobs.Clear();
        jobHandles.Clear();
    }

    void CheckForEmptyChunks()
    {
        relativePlayerPosition = new Vector2((int)(playerTransform.position.x/chunkDimensions + 2344), (int)(playerTransform.position.z/chunkDimensions + 2344));
        for(int x = (int)(relativePlayerPosition.x - renderDistance); x<relativePlayerPosition.x + renderDistance +1; x++)
        {
            for(int y = (int)(relativePlayerPosition.y -renderDistance); y<relativePlayerPosition.y +renderDistance +1; y++)
            {
                if(generatedChunks[x,y] == null)
                {
                    GenerateChunk(new Vector2(x, y));
                }

            }
        }
    }

    void GenerateChunk(Vector2 coordinates)
    {
        jobs.Add(new GenerationJob()
        {
            noiseSize = noiseSize,
            noiseScale = noiseScale,
            xseed = xseed,
            yseed = yseed,
            chunkDimensions = chunkDimensions,
            texelPerMeter = texelPerMeter,
            chunkWorldCoordinates = new Vector2((coordinates.x - 2344) * chunkDimensions, (coordinates.y - 2344) * chunkDimensions),
            chunkArrayCoordinates = coordinates,
            heights = new NativeArray<float>((int)(Mathf.Pow((chunkDimensions * texelPerMeter) + 1, 2)), Allocator.Persistent),
            heightOffsetNoiseSize = heightOffsetNoiseSize
        });
        jobHandles.Add(jobs[jobs.Count-1].Schedule());
        
    }

    void SyncTerrainData()
    {
        Profiler.BeginSample("Job time");
        foreach(JobHandle jobHandle in jobHandles)
        {
            jobHandle.Complete();
        }
        Profiler.EndSample();
        foreach(GenerationJob job in jobs)
        {
            generatedChunks[(int)(job.chunkArrayCoordinates.x), (int)(job.chunkArrayCoordinates.y)] = Terrain.CreateTerrainGameObject(new TerrainData());
            Terrain terrain = generatedChunks[(int)(job.chunkArrayCoordinates.x), (int)(job.chunkArrayCoordinates.y)].GetComponent<Terrain>();
            terrain.transform.parent = worldTransform;
            terrain.transform.localPosition = new Vector3(job.chunkWorldCoordinates.x,0, job.chunkWorldCoordinates.y);
            terrain.heightmapPixelError = 1;
            terrain.materialTemplate = terrainMaterial;
            terrain.shadowCastingMode = ShadowCastingMode.Off;
            TerrainData terrainData = terrain.terrainData;
            terrainData.heightmapResolution = chunkDimensions * texelPerMeter + 1;
            terrainData.size = new Vector3(chunkDimensions, 256, chunkDimensions);
            terrainData.SetHeights(0, 0, ConvertArray(job.heights));
        }
        jobs.Clear();
        jobHandles.Clear();
    }

    float[,] ConvertArray(NativeArray<float> heights)
    {
        float[,] convertedHeights = new float[chunkDimensions * texelPerMeter + 1, chunkDimensions * texelPerMeter + 1];
        for(int x = 0; x<heights.Length; x++)
        {
            convertedHeights[x % convertedHeights.GetLength(1), x / convertedHeights.GetLength(0)] = heights[x];

        }
        heights.Dispose();
        return convertedHeights;
    }

    struct GenerationJob : IJob
    {
        public float noiseSize, noiseScale, heightOffsetNoiseSize;
        public int xseed, yseed,chunkDimensions,texelPerMeter, heightmapResolution;
        public Vector2 chunkWorldCoordinates,chunkArrayCoordinates;
        public Vector3 size;
        public NativeArray<float> heights;

        public void Execute()
        {
            heightmapResolution = chunkDimensions * texelPerMeter + 1;
            size = new Vector3(chunkDimensions, 256, chunkDimensions);
            calculateHeight();
        }

        void calculateHeight()
        {
            float heightOffset = 0;
            for (int x = 0; x < chunkDimensions * texelPerMeter + 1; x++)
            {
                for (int y = 0; y < chunkDimensions * texelPerMeter + 1; y++)
                {
                    heightOffset = Mathf.Pow((Mathf.PerlinNoise((x + ((chunkWorldCoordinates.x + (150000 + 300000 * xseed)) * texelPerMeter)) * heightOffsetNoiseSize, (y + ((chunkWorldCoordinates.y + (150000 + 300000 * yseed)) * texelPerMeter)) * heightOffsetNoiseSize)-0.5f)*2,3)+0.05f;
                    heights[(x * (chunkDimensions * texelPerMeter + 1))+y] = Mathf.PerlinNoise((x + ((chunkWorldCoordinates.x + (150000 + 300000 * xseed)) * texelPerMeter)) * noiseSize, (y + ((chunkWorldCoordinates.y + (150000 + 300000 * yseed)) * texelPerMeter)) * noiseSize) * noiseScale + heightOffset;
                }
            }
        }
    }
}
