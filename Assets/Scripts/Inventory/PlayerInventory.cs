using static MeshCalculation;
using UnityEngine;

public class PlayerInventory : MonoBehaviour, IInventory
{
    public Inventory inventory;
    public bool printInventory;
    void Start()
    {
        inventory = new Inventory(10);
    }
    public bool AddItem(GameObject Item)
    {
        return inventory.AddItem(Item.name, CalculateVolume(Item.GetComponent<MeshFilter>().sharedMesh, Item.transform.lossyScale));
    }
    private void OnValidate()
    {
        if(printInventory == true)
        {
            for(int i = 0; i < inventory.numberOfSlots; i++)
            {
                Debug.Log(inventory.GetName(i) + ", " + inventory.GetCount(i));
            }
            printInventory = false;
        }
    }
}
