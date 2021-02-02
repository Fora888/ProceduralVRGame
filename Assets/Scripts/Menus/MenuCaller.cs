using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class MenuCaller : MonoBehaviour
{
    private GameObject menus;
    private bool onePress,temp;
    public SteamVR_Action_Boolean menuButton;
    //Aktiviert das Menu Objekt wenn der Menu Knopf gedrückt wird

    // Start is called before the first frame update
    void Start()
    {
        menus = GameObject.FindWithTag("DeveloperMenu");
        menus.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(onePress == false && menuButton.GetState(SteamVR_Input_Sources.Any) == true && temp == false)
        {
            onePress = true;
        }
        else if(onePress == true && menuButton.GetState(SteamVR_Input_Sources.Any) == true && temp == false)
        {
            onePress = false;
            temp = true;
        }
        else if(temp == true && menuButton.GetState(SteamVR_Input_Sources.Any) == false)
        {
            temp = false;
        }

       if(onePress == true && menus.activeSelf == false)
        {
            menus.SetActive(true);
        }

        else if (onePress == true && menus.activeSelf == true)
        {
            menus.SetActive(false);
        }
    }
}
