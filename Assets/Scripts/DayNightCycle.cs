using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float secondsPerDay, currentTime = 900;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentTime = currentTime + (2400 / secondsPerDay * Time.deltaTime);
        if (currentTime > 2400) currentTime = 0;
        transform.rotation = Quaternion.AngleAxis((currentTime/6.666f-90f), Vector3.right);
    }
}
