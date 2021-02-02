using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindNameFromTag : MonoBehaviour
{
    public static GameObject FindGameObjectFromTag(GameObject[] tagList, string findObject)
    {
        for(int i = 0; i < tagList.Length; i++)
        {
            if(tagList[i].name.Equals(findObject))
            {
                return tagList[i];
            }
        }
        return null;
    }
    
}
