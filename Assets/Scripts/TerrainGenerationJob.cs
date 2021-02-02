using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

//The job that gets instanciated by TerrainGenerator to calculate the vertice Offset of a chunk and various attributes of Scenery elements.
//Mostly various points from within the Plane are translated to Global coordinates and the seed offset is applied. The translated coordinates are then plugged into a perlin noise function. The resulting value is againput into an curve to get a better controll of distribution of the noise
struct TerrainGenerationJob : IJob
{
    public float noiseSize, noiseScale, heightOffsetNoiseSize, vertexIndexToPosition, stoneNoiseScale, stoneMaxAxisScaleDeviation;
    public int xseed, yseed, chunkDimensions, verticesPerEdge, stoneListSize;
    public Vector2 chunkWorldCoordinates, chunkArrayCoordinates;
    public NativeList<GeneratedGameObjectData> stoneData;
    public NativeArray<Vector3> vertices, triangleVertices;
    public SampledAnimationCurve strongDisplacementCurveLUT, stoneQuantityDistributionCurveLUT, stoneSizeDistributuionCurveLUT;
    //private MathU.Random randomX, randomY;

    public void Execute()
    {
        CalculateMesh();
    }

    void CalculateMesh()
    {
        float xChunkOffset = chunkWorldCoordinates.x + (150000 + 300000 * xseed);
        float yChunkOffset = chunkWorldCoordinates.y + (150000 + 300000 * yseed);
        float largeHeightOffset, height;
        XORSHIFT Random = new XORSHIFT((int)xChunkOffset, (int)yChunkOffset);
        //Execute Per Vertex
        for (int x = 0; x < verticesPerEdge; x++)
        {
            for (int y = 0; y < verticesPerEdge; y++)
            {
                largeHeightOffset = strongDisplacementCurveLUT.Evaluate(Mathf.PerlinNoise((x * vertexIndexToPosition + xChunkOffset) * heightOffsetNoiseSize, (y * vertexIndexToPosition + yChunkOffset) * heightOffsetNoiseSize)); //muss zu int gecastet werden und mit anzahl an samples multipliziert werden;
                height = Mathf.PerlinNoise((x * vertexIndexToPosition + xChunkOffset) * noiseSize, (y * vertexIndexToPosition + yChunkOffset) * noiseSize) * noiseScale + largeHeightOffset;
                vertices[(int)((x * verticesPerEdge) + y)] = new Vector3(x * vertexIndexToPosition, height * 256, y * vertexIndexToPosition);

                //Middle Vertex of the Plane gets shiftet both x and z
                if ((int)((x * verticesPerEdge) + y) == 4)
                    vertices[4] = new Vector3(vertices[4].x + Random.NextFloat(-8, 8), vertices[4].y, vertices[4].z + Random.NextFloat(-8, 8));
                    
            }
        }

        //Execute Per Face
        for (int x = 0; x < verticesPerEdge - 1; x++)
        {
            for (int y = 0; y < (verticesPerEdge - 1) * 2; y++)
            {

                Vector2 approximateTriangleMidpoint = new Vector2((chunkDimensions / (verticesPerEdge - 1)) * x, (chunkDimensions / ((verticesPerEdge - 1) * 2)) * y);
                getTriangleVerticesPosition(vertices, x, y);
                int stoneCount = (int)stoneQuantityDistributionCurveLUT.Evaluate(Mathf.PerlinNoise(-(xChunkOffset + approximateTriangleMidpoint.x) * stoneNoiseScale, (yChunkOffset + approximateTriangleMidpoint.y) * stoneNoiseScale));
                for(int i = 0; i < stoneCount; i++)
                {
                    float r1 = Random.NextFloat();
                    float r2 = Random.NextFloat();
                    float size = stoneSizeDistributuionCurveLUT.Evaluate(Random.NextFloat());
                                                                //Get Point from barycentric coordinates https://stackoverflow.com/questions/19654251/random-point-inside-triangle-inside-java
                    stoneData.Add(new GeneratedGameObjectData((1 - Mathf.Sqrt(r1)) * triangleVertices[0] + (Mathf.Sqrt(r1) * (1 - r2)) * triangleVertices[1] + (Mathf.Sqrt(r1) * r2) * triangleVertices[2],
                                                                new Vector3(Random.NextFloat(0, 360), Random.NextFloat(0, 360), Random.NextFloat(0, 360)),
                                                                new Vector3 (size * Random.NextFloat(1 - stoneMaxAxisScaleDeviation, 1 + stoneMaxAxisScaleDeviation), size * Random.NextFloat(1 - stoneMaxAxisScaleDeviation, 1 + stoneMaxAxisScaleDeviation), size * Random.NextFloat(1 - stoneMaxAxisScaleDeviation, 1 + stoneMaxAxisScaleDeviation)),
                                                                (int)Random.NextFloat(0, stoneListSize)));
                }
                
            }
        }

    }

    //Provides the Vertices of a Triangle in a Plane
    private void getTriangleVerticesPosition(NativeArray<Vector3> vertices, int x, int y)
    {
        if(y % 2 == 0)
        {
            y = (int)(y * 0.5);
            triangleVertices[0] = vertices[x * verticesPerEdge + y];
            triangleVertices[1] = vertices[x * verticesPerEdge + y + 1];
            triangleVertices[2] = vertices[(x + 1) * verticesPerEdge + y];
        }
        else
        {
            y = (int)(y * 0.5);
            triangleVertices[0] = vertices[(x + 1) * verticesPerEdge + y];
            triangleVertices[1] = vertices[x * verticesPerEdge + y + 1];
            triangleVertices[2] = vertices[(x + 1) * verticesPerEdge + y + 1];
        }
    }

    public void Dispose()
    {
        stoneData.Dispose();
        vertices.Dispose();
        triangleVertices.Dispose();
    }
}