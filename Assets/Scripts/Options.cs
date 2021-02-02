using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour
{
    public int fpsCap = 144;
    // Start is called before the first frame update
    void Start()
    {
        UpdateSettings();
    }

    private void OnValidate()
    {
        UpdateSettings();
    }
    public void UpdateSettings()
    {
        Application.targetFrameRate = fpsCap;
    }
}
