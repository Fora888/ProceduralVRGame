using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using System;
using Random = UnityEngine.Random;
using System.Data;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

class PerlinNoiseTest : MonoBehaviour
{
    private float time, capture1, capture2, capture3, capture4, x;
    private DateTime dateTime;
    void Start()
    {
        time = Stopwatch.GetTimestamp();
        x = Mathf.Pow(Random.value, 2);
        capture1 = Stopwatch.GetTimestamp();
        x = Mathf.Pow(Random.value, 200);
        capture2 = Stopwatch.GetTimestamp();
        x = Random.value * 2;
        capture3 = Stopwatch.GetTimestamp();
        x = Random.value * 200;
        capture4 = Stopwatch.GetTimestamp();

        Debug.Log(capture1 - time);
        Debug.Log(capture2 - time);
        Debug.Log(capture3 - time);
        Debug.Log(capture4 - time);
        Debug.Log(Stopwatch.Frequency);
    }

    private void FixedUpdate()
    {

    }
}