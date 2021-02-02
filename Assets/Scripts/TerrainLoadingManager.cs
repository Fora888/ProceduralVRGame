using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainLoadingManager : MonoBehaviour
{
    private TerrainGenerator Generator;
    private Vector2 previousPosition,difference;
    private int maxDifference, renderDistance, differenceSignX, differenceSignY;
    private WaterPlacer waterPlacer;
    private OriginShift originShift;
    // Start is called before the first frame update
    void Start()
    {
        Generator = GetComponent<TerrainGenerator>();
        waterPlacer = GetComponent<WaterPlacer>();
        originShift = GameObject.FindGameObjectWithTag("World").GetComponent<OriginShift>();
        renderDistance = Generator.renderDistance;
        maxDifference = renderDistance * 2 + 1;
        previousPosition = Generator.relativePlayerPosition;
        Debug.Log(previousPosition.x + ", " + Generator.relativePlayerPosition.x);
        Debug.Log(previousPosition.y + ", " + Generator.relativePlayerPosition.y);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(previousPosition != Generator.relativePlayerPosition)
        {
            difference.x = Mathf.Clamp(Generator.relativePlayerPosition.x - previousPosition.x, -maxDifference, maxDifference);
            difference.y = Mathf.Clamp(Generator.relativePlayerPosition.y - previousPosition.y, -maxDifference, maxDifference);      
            differenceSignX = (int)Mathf.Sign(difference.x);
            differenceSignY = (int)Mathf.Sign(difference.y);
            Vector2 chunkCoordinate = new Vector2();
            for (int x = 0; x < Mathf.Abs(difference.x); x++)
            {
                for(int y = -renderDistance; y < renderDistance +1 ; y++)
                {
                    try
                    {
                        Generator.generatedChunks[(int)(previousPosition.x + (x * differenceSignX) - (renderDistance * differenceSignX)), (int)(previousPosition.y + y)].SetActive(false);
                        chunkCoordinate = new Vector2(previousPosition.x - (x * differenceSignX) + (renderDistance + 1) * differenceSignX, previousPosition.y + y);
                        originShift.offsetChunk(Generator.generatedChunks[(int)chunkCoordinate.x, (int)chunkCoordinate.y], chunkCoordinate);
                        Generator.generatedChunks[(int)chunkCoordinate.x, (int)chunkCoordinate.y].SetActive(true);
                    }
                    catch(NullReferenceException)
                    {
                        Debug.Log("Activating Error");
                    }
                }
            }

            for (int x = 0; x < Mathf.Abs(difference.y); x++)
            {
                for (int y = -renderDistance; y < renderDistance+1; y++)
                {
                    try
                    {
                        Generator.generatedChunks[(int)(previousPosition.x + y), (int)(previousPosition.y + (x * differenceSignY) - renderDistance * differenceSignY)].SetActive(false);
                        chunkCoordinate = new Vector2(previousPosition.x + y, previousPosition.y - (x * differenceSignY) + (renderDistance + 1) * differenceSignY);
                        originShift.offsetChunk(Generator.generatedChunks[(int)chunkCoordinate.x, (int)chunkCoordinate.y], chunkCoordinate);
                        Generator.generatedChunks[(int)chunkCoordinate.x, (int)chunkCoordinate.y].SetActive(true);
                    }
                    catch (NullReferenceException)
                    {
                        Debug.Log("Activating Error");
                    }
                }
            }
            previousPosition = Generator.relativePlayerPosition;
        }
        else
        {
            previousPosition = Generator.relativePlayerPosition;
        }
    }
}
