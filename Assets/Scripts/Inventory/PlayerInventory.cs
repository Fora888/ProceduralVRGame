using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour, IInventory
{
    public int[,] inventory;
    private void Start()
    {
        inventory = new int[10,2];
    }
    public void AddItem(GameObject Item)
    {

    }

    public bool ContainsID(int iD)
    {
        for(int i = 0; i < inventory.GetLength(0); i++)
        {
            if (inventory[i,0] == iD)
                return true;
        }
        return false;
    }
}
