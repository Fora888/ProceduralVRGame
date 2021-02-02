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

//This script Checks wether new Chunks need to be generated. It also Manages the jobs to generate those chunks aswell as the data of already generated chunks.
public class TerrainGenerator : MonoBehaviour
{
    public float noiseScale = 0.02f, noiseSize = 0.03f, heightOffsetNoiseSize = 0.003f;
    public int seed, renderDistance = 3;
    public Material terrainMaterial;
    public Material grassMaterial;
    [HideInInspector]
    public GameObject[,] generatedChunks = new GameObject[4688, 4688];
    [HideInInspector, ReadOnly]
    public Vector2 relativePlayerPosition;
    public int chunkDimensions = 32, verticesPerEdge = 3;
    public AnimationCurve strongDisplacementCurve;
    private Transform worldTransform;
    private int xseed, yseed;
    private float vertexIndexToPosition;
    private Transform playerTransform;
    private List<JobHandle> jobHandles;
    private List<TerrainGenerationJob> jobs;
    private int[] precaculatedTris;
    private SceneryPlacer sceneryPlacer;
    private SampledAnimationCurve strongDisplacementCurveLUT;
    private OriginShift originShift;
    void Start()
    {
        originShift = GameObject.FindGameObjectWithTag("World").GetComponent<OriginShift>();
        strongDisplacementCurveLUT = new SampledAnimationCurve(strongDisplacementCurve,256);
        worldTransform = GetComponent<Transform>();
        jobHandles = new List<JobHandle>();
        jobs = new List<TerrainGenerationJob>();
        playerTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        xseed = (int)(seed / 46340.95000105199);
        yseed = (int)(seed % 46340.95000105199);
        relativePlayerPosition = new Vector2((int)(playerTransform.position.x / chunkDimensions + 2344), (int)(playerTransform.position.z / chunkDimensions + 2344));
        precaculatedTris = PrecaculateTris();
        sceneryPlacer = GetComponent<SceneryPlacer>();
        vertexIndexToPosition = 1f / (verticesPerEdge - 1) * chunkDimensions;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckForEmptyChunks();
    }

    void Update()
    {
        SyncTerrainData();
    }

    //Ensures that there are no undisposed native collections from jobs
    private void OnApplicationQuit()
    {
        jobs.Clear();
        jobHandles.Clear();
        strongDisplacementCurveLUT.Dispose();
    }

    void CheckForEmptyChunks()
    {
        //player coordinates get converted into the chunk grid and checked, if there is a chunk already generated. If not the chunk gets scheduled to get generated
        relativePlayerPosition = new Vector2((int)((playerTransform.position.x + -originShift.offset.x) / chunkDimensions + 2344), (int)((playerTransform.position.z + -originShift.offset.y) / chunkDimensions + 2344));
        for (int x = (int)(relativePlayerPosition.x - renderDistance); x < relativePlayerPosition.x + renderDistance + 1; x++)
        {
            for (int y = (int)(relativePlayerPosition.y - renderDistance); y < relativePlayerPosition.y + renderDistance + 1; y++)
            {
                if (generatedChunks[x, y] == null)
                {
                    generatedChunks[x,y] = new GameObject("Chunk (" + x + "," + y + ")");
                    ScheduleJob(new Vector2(x, y));
                }
            }
        }
    }

    //Generates a new TerrainGenerationJob
    void ScheduleJob(Vector2 coordinates)
    {

        jobs.Add(new TerrainGenerationJob()
        {
            strongDisplacementCurveLUT = strongDisplacementCurveLUT,
            noiseSize = noiseSize,
            noiseScale = noiseScale,
            xseed = xseed,
            yseed = yseed,
            chunkDimensions = chunkDimensions,
            chunkWorldCoordinates = new Vector2((coordinates.x - 2344) * chunkDimensions, (coordinates.y - 2344) * chunkDimensions),
            chunkArrayCoordinates = coordinates,
            vertices = new NativeArray<Vector3>((int)(Mathf.Pow(verticesPerEdge, 2)), Allocator.Persistent),
            triangleVertices = new NativeArray<Vector3>(3, Allocator.Persistent),
            heightOffsetNoiseSize = heightOffsetNoiseSize,
            verticesPerEdge = verticesPerEdge,
            vertexIndexToPosition = vertexIndexToPosition,
            stoneQuantityDistributionCurveLUT = sceneryPlacer.stoneQuantityDistributionCurveLUT,
            stoneNoiseScale = sceneryPlacer.stoneNoiseScale,
            stoneData = new NativeList<GeneratedGameObjectData>(Allocator.Persistent),
            stoneSizeDistributuionCurveLUT = sceneryPlacer.stoneSizeDistributuionCurveLUT,
            stoneListSize = sceneryPlacer.stonePrefabs.Length - 1,
            stoneMaxAxisScaleDeviation = sceneryPlacer.stoneMaxAxisScaleDeviation,
        });
        jobHandles.Add(jobs[jobs.Count - 1].Schedule());

    }

    //Checks if a job is completet and applies the generated values to the Mesh and Hands them over to the SceneryPlacer
    void SyncTerrainData()
    {
        if(jobHandles.Count>0 && jobHandles[jobHandles.Count - 1].IsCompleted)
        {
            jobHandles[jobHandles.Count - 1].Complete();
            TerrainGenerationJob job = jobs[jobs.Count - 1];
            GameObject chunk = generatedChunks[(int)(job.chunkArrayCoordinates.x), (int)(job.chunkArrayCoordinates.y)];
            chunk.layer = 8;
            Transform chunkTransform = chunk.transform;
            chunkTransform.parent = worldTransform;
            originShift.offsetChunk(chunk, job.chunkArrayCoordinates);
            chunk.isStatic = true;
            MeshRenderer meshRenderer = chunk.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterials = new Material[2] { terrainMaterial, grassMaterial };
            MeshFilter meshFilter = chunk.AddComponent<MeshFilter>();
            meshFilter.mesh = GeneratePlane(job.vertices.ToArray());
            meshRenderer.shadowCastingMode = ShadowCastingMode.TwoSided;
            meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            MeshCollider chunkCollider = chunk.AddComponent<MeshCollider>();
            chunkCollider.sharedMesh = meshFilter.mesh;
            sceneryPlacer.PlaceScenery(chunk, job.stoneData.ToArray());
            job.Dispose();
            jobs.RemoveAt(jobs.Count - 1);
            jobHandles.RemoveAt(jobHandles.Count-1);
        } 
    }

    Mesh GeneratePlane(Vector3[] vertices)
    {
        Mesh plane = new Mesh
        {
            vertices = vertices,
            triangles = precaculatedTris
        };
        plane.RecalculateNormals();
        return plane;
    }

    //Precalculate the connections between the vertices to form a plane
    private int[] PrecaculateTris()
    {
        List<int> tris = new List<int>();
        for (int u = 0; u < verticesPerEdge - 1; u++)
        {
            for (int v = 0; v < verticesPerEdge - 1; v++)
            {
                tris.Add(u*(verticesPerEdge) +v);
                tris.Add(u * (verticesPerEdge) + v + 1);
                tris.Add((u+1) * (verticesPerEdge) + v);
                
                tris.Add((u + 1) * (verticesPerEdge) + v);
                tris.Add(u  * (verticesPerEdge) + v + 1);
                tris.Add((u + 1) * (verticesPerEdge) + v + 1);
            }
        }
        return tris.ToArray();
    }
}
