using static MeshCalculation;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour, IInventory
{
    UnityEvent m_MyEvent = new UnityEvent();
    public Inventory inventory;
    public int maxSize = 10;
    private MaterialDictionary materialDictionary;
    void Start()
    {
        inventory = new Inventory(maxSize);
        materialDictionary = GameObject.FindGameObjectWithTag("MaterialDictionary").GetComponent<MaterialDictionary>();
    }
    public bool AddItem(GameObject Item)
    {
        return inventory.AddItem(materialDictionary.GetNameFromAlias(Item.name), CalculateVolume(Item.GetComponent<MeshFilter>().sharedMesh, Item.transform.lossyScale));
    }
}
