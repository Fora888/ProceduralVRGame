using NUnit.Framework.Internal.Execution;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuItemSlot : MonoBehaviour
{
    private TextMeshPro textBox;
    private InventorySlot slot;
    public void Init(InventorySlot slot)
    {
        this.slot = slot;
        textBox = gameObject.GetComponentInChildren<TextMeshPro>(false);
    }
    private void FixedUpdate()
    {
        UpdateDisplay(slot.name, slot.count);
    }
    public void UpdateDisplay(string name, float count)
    {
        if(count > 0)
            textBox.SetText(name + ":\n" + count);
    }
}
