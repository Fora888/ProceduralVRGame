using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;

public class SceneryPlacer : MonoBehaviour
{
    public float stoneNoiseScale = 0.05f, stoneMaxAxisScaleDeviation = 0.2f;
    public AnimationCurve stoneQuantityDistributionCurve, stoneSizeDistributuionCurve;
    public bool ReloadStones;
    public Object[] stonePrefabs;
    [HideInInspector]
    public SampledAnimationCurve stoneQuantityDistributionCurveLUT, stoneSizeDistributuionCurveLUT;

    //This script is a dependency of TerrainGenerator. It servers the purpose to Seperate the scenery object instanciation and variables from the chunk mesh generation and job management. 
    //Essentialy it supplies the terrain Generator with the Necessary variables and Applies the variables it gets from it to new Game Objects
    private void Start()
    {
        stoneQuantityDistributionCurveLUT = new SampledAnimationCurve(stoneQuantityDistributionCurve, 256);
        stoneSizeDistributuionCurveLUT = new SampledAnimationCurve(stoneSizeDistributuionCurve, 256);
    }

    private void OnApplicationQuit()
    {
        stoneQuantityDistributionCurveLUT.Dispose();
        stoneSizeDistributuionCurveLUT.Dispose();
    }
    public void PlaceScenery(GameObject chunk, GeneratedGameObjectData[] stoneData)
    {
        foreach (GeneratedGameObjectData stone in stoneData)
        { 
            GameObject newStone = Instantiate((GameObject)stonePrefabs[stone.modellIndex], chunk.transform);
            newStone.transform.localPosition = stone.midpoint;
            newStone.transform.eulerAngles = stone.rotation;
            newStone.transform.localScale = stone.size;
            newStone.GetComponent<MeshCollider>().convex = true;
        }
        /*
        RaycastHit raycastHit;
        List<GameObject> stone = new List<GameObject>();
        for(int x = 0; x<placingGridSize; x++)
        {
            for (int y = 0; y < placingGridSize; y++)
            {
                if(Random.value<Mathf.PerlinNoise(chunkArrayCoordinateX / stoneNoiseScale, chunkArrayCoordinateY / stoneNoiseScale))
                {
                    stone.Add(Instantiate((GameObject)stonePrefabs[(int)Random.Range(0, stonePrefabs.Length-1)], chunk.transform));
                    GameObject newStone = stone[stone.Count - 1];
                    Transform newStoneTransform = newStone.transform;
                    newStoneTransform.localPosition = new Vector3((x + Random.Range(0,maxStoneGridDiviation)) * chunkDimension / placingGridSize , 0, (y + Random.Range(0, maxStoneGridDiviation)) * chunkDimension / placingGridSize);

                    Physics.Raycast(new Vector3(newStoneTransform.position.x, 1000, newStoneTransform.position.z), -Vector3.up, out raycastHit, Mathf.Infinity, layerMask);
                    newStoneTransform.position = new Vector3(newStoneTransform.position.x, raycastHit.point.y, newStoneTransform.position.z);
                    newStoneTransform.rotation = Random.rotation;
                    //float scale = 34 * Mathf.Pow(Random.value-0.25f, 6) + 0.25f;
                    float scale = stoneDistributionCurve.Evaluate(Random.value);
                    newStoneTransform.localScale = new Vector3(100 * scale, 65 * scale, 100 * scale);
                }
            }
        }
        */
    }

    //Loads all Prefabs in a Folder
    public void OnValidate()
    {
        if (ReloadStones == true)
        {
            string[] files = Directory.GetFiles("Assets/Models/Stones", "*.fbx");
            stonePrefabs = new Object[files.Length - 1];
            for(int i = 0; i < files.Length-1; i++)
            {
                stonePrefabs[i] = AssetDatabase.LoadAssetAtPath(files[i], typeof (Object));
            }
        }
        ReloadStones = false;
    }
}



