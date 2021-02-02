using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class OriginShift : MonoBehaviour
{
    public int maxDistanceFromOrigin = 500;
    //[HideInInspector]
    public Vector2 offset;
    private Transform cameraTransform, playerTransform;
    private TerrainGenerator terrainGenerator;
    //Checks wether the Player is a certain distance away from 0,0. When this value is exceeded by the player it moves the Player and the Chunks that are in render distance of the Player back by the set limit, so they are esentialy back to 0,0. This is done to preserve floating point precision
    //It also provides the applied offset and A method to shift newly loaded or generated chunks by this offset
    void Start()
    {
        offset = new Vector2(0, 0);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        terrainGenerator = GameObject.FindGameObjectWithTag("Terrain").GetComponent<TerrainGenerator>();
    }

    void FixedUpdate()
    {
        if(Mathf.Abs(cameraTransform.position.x) > maxDistanceFromOrigin || Mathf.Abs(cameraTransform.position.z) > maxDistanceFromOrigin)
        {
            Vector2 playerOffset = new Vector2();
            if(Mathf.Abs(cameraTransform.position.x) > maxDistanceFromOrigin)
            {
                offset.x += -maxDistanceFromOrigin * Mathf.Sign(cameraTransform.position.x);
                playerOffset.x = -maxDistanceFromOrigin * Mathf.Sign(cameraTransform.position.x);
            }

            if (Mathf.Abs(cameraTransform.position.z) > maxDistanceFromOrigin)
            {
                offset.y += -maxDistanceFromOrigin * Mathf.Sign(cameraTransform.position.z);
                playerOffset.y = -maxDistanceFromOrigin * Mathf.Sign(cameraTransform.position.z);
            }

            playerTransform.position = new Vector3(playerTransform.position.x + playerOffset.x, playerTransform.position.y, playerTransform.position.z + playerOffset.y);

            for (int x = (int)(terrainGenerator.relativePlayerPosition.x - terrainGenerator.renderDistance); x < terrainGenerator.relativePlayerPosition.x + terrainGenerator.renderDistance + 1; x++)
            {
                for (int y = (int)(terrainGenerator.relativePlayerPosition.y - terrainGenerator.renderDistance); y < terrainGenerator.relativePlayerPosition.y + terrainGenerator.renderDistance + 1; y++)
                {
                    offsetChunk(terrainGenerator.generatedChunks[x, y], new Vector2(x,y));
                }
            }

        }
    }

    public void offsetChunk(GameObject chunk, Vector2 arrayCoordinates)
    {
        Vector2 chunkOriginCoordinates = new Vector2((arrayCoordinates.x - 2344) * terrainGenerator.chunkDimensions, (arrayCoordinates.y - 2344) * terrainGenerator.chunkDimensions);
        chunk.transform.position = new Vector3(chunkOriginCoordinates.x + offset.x, 0, chunkOriginCoordinates.y + offset.y);
    }
}
