using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InputHandler : MonoBehaviour
{
    private List<UnityEngine.XR.InputDevice>  leftHandDevices = new List<UnityEngine.XR.InputDevice>();
    private List<UnityEngine.XR.InputDevice>  rightHandDevices = new List<UnityEngine.XR.InputDevice>();

    public bool leftGripIsPressed;
    public bool leftMenuButton;
    public Vector2 primary2DAxisLeft;
    public Vector2 secondary2DAxisLeft;
    

    public bool rightGripIsPressed;
    public bool rightMenuButton;
    public Vector2 primary2DAxisRight;
    public Vector2 secondary2DAxisRight;
    

    // Start is called before the first frame update
    void Start()
    {
        checkForLeftController();
        checkForRightController();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (leftHandDevices.Count == 0)
            checkForLeftController();
        else
            checkForLeftButtonPresses();

        if (rightHandDevices.Count == 0)
            checkForRightController();
        else
            checkForRightButtonPresses();
    }

    void checkForLeftController()
    {  
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);
        if (leftHandDevices.Count >= 1)
        {
            Debug.Log("Controller links erkannt");
        }
    }

    void checkForRightController()
    {
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);
        if (rightHandDevices.Count >= 1)
        {
            Debug.Log("Controller rechts erkannt");
        }

    }
    
    void checkForLeftButtonPresses()
    {
        int count = 0;
        foreach (var device in leftHandDevices)
        {
            leftHandDevices[count].TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out leftGripIsPressed);
            leftHandDevices[count].TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out primary2DAxisLeft);
            leftHandDevices[count].TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondary2DAxis, out secondary2DAxisLeft);
            leftHandDevices[count].TryGetFeatureValue(UnityEngine.XR.CommonUsages.menuButton, out leftMenuButton);
            count++;
        }
    }

    void checkForRightButtonPresses()
    {
        int count = 0;
        foreach (var device in rightHandDevices)
        {
            rightHandDevices[count].TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out rightGripIsPressed);
            rightHandDevices[count].TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out primary2DAxisRight);
            rightHandDevices[count].TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondary2DAxis, out secondary2DAxisRight);
            rightHandDevices[count].TryGetFeatureValue(UnityEngine.XR.CommonUsages.menuButton, out leftMenuButton);
            count++;
        }
    }
}
