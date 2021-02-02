using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class UsefullMethods : MonoBehaviour
{
    float[,] ConvertArray(NativeArray<float> heights, int xSize, int ySize)
    {
        float[,] convertedHeights = new float[(int)(xSize), (int)(ySize)];
        for (int x = 0; x < heights.Length; x++)
        {
            convertedHeights[x % convertedHeights.GetLength(1), x / convertedHeights.GetLength(0)] = heights[x];
        }
        heights.Dispose();
        return convertedHeights;
    }
}
