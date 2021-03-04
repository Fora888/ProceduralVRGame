using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
public class Tablet : MonoBehaviour
{
    public GameObject forwardButton, backButton;
    public GameObject[] options, buttons;
    public GameObject playerInventoryGO;
    private Inventory playerInventory;
    public int currentMenu, currentPage;
    private MenuItemSlot slot;
    private Component[] buttonActions;
    // Update is called once per frame
    void Start()
    {
        playerInventory = playerInventoryGO.GetComponent<PlayerInventory>().inventory;
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].AddComponent<MenuItemSlot>().Init(playerInventory.slots[i + currentPage * (buttons.Length - 1)]); 
        }
    }

    
}
