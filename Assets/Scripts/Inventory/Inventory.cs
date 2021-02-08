using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Inventory
{
    public InventorySlot[] slots;
    public int numberOfSlots;
    public Inventory(int size)
    {
        slots = new InventorySlot[size];
        for(int i = 0; i<size; i++)
        {
            slots[i] = new InventorySlot();
        }
        numberOfSlots = size;
    }

    public bool AddItem(string name, float count)
    {
        bool added = false;
        for(int i = 0; i < numberOfSlots; i++)
        {
            if(slots[i].name == name)
            {
                slots[i].count += count;
                added = true;
                break;
            }
        }
        if(added == false)
        {
            for (int i = 0; i < numberOfSlots; i++)
            {
                if (string.IsNullOrEmpty(slots[i].name))
                {
                    slots[i].count = count;
                    slots[i].name = name;
                    added = true;
                    break;
                }
            }
        }
        return added;
    }

    public float RetriveItem(int index, float count)
    {
        float retrivedCount = 0;
        if (slots[index].count != 0)
        {
            if (slots[index].count <= count)
            {
                retrivedCount = slots[index].count;
                slots[index].count = 0;
                slots[index].name = null;
            }
            else
            {
                retrivedCount = count;
                slots[index].count -= count;
            }
        }
        return retrivedCount;
    }
    
    public InventorySlot GetSlot(int index)
    {
        return slots[index];
    }
    public string GetName(int index)
    {
        return slots[index].name;
    }
    public float GetCount(int index)
    {
        return slots[index].count;
    }
}
