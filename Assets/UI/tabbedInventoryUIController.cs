using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class tabbedInventoryUIController : MonoBehaviour
{
    private VisualElement root;
    public VisualElement slotVisualElement;

    // keep track of inventory ids so they will always render in the same order
    private Dictionary<int, InventorySlot> inventorySlotsById = new Dictionary<int, InventorySlot>();
    private List<InventorySlot> inventorySlots = new List<InventorySlot>(); 

    public int numInventorySlots;

    private void Awake()
    {
        numInventorySlots = 5; 
        root = GetComponent<UIDocument>().rootVisualElement; // retrieve root from UI document
        slotVisualElement = root.Query("SlotsContainer"); 
        for(int i = 0; i < numInventorySlots; i++)
        {
            InventorySlot itemSlot = new InventorySlot(i); // keep track of inventory ids
            inventorySlotsById.Add(i, itemSlot);
            inventorySlots.Add(itemSlot); // keep track of this in list form for convenience
            slotVisualElement.Add(itemSlot); 
        }
        inventoryController.onInventoryChanged += inventoryController_onInventoryChanged;  // add event listener so our UI will change when items change
    }

    private void inventoryController_onInventoryChanged(int id, ItemDetails itemDetails, InventoryChangeType inventoryChangeType)
    {
        Debug.Log("calling OnInventoryChanged");
        if(inventoryChangeType == InventoryChangeType.Pickup)
        {
            InventorySlot inventorySlot = inventorySlotsById[id];
            if(inventorySlot != null)
            {
                inventorySlot.holdItem(itemDetails); 
            }
            else
            {
                Debug.Log("Could not find a valid inventory slot with ID " + id + " : unable to store item"); 
            }
        }
    }
}
