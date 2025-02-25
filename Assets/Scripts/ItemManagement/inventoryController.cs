using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// classes that handle player current inventory contents
public class ItemDetails // wrapper class which  additionally contains canDrop + interaction options - specifically for inventory purposes
{
    public string uiName; // name for display in UI
    public ItemData itemData;
    private bool canDrop; // TODO : not sure if we'll need this but i'll keep it in for now
    private bool canInteract;

    public ItemDetails(string uiName, ItemData itemData, bool canDrop)
    {
        this.uiName = uiName;
        this.itemData = itemData;
        this.canDrop = canDrop;
        canInteract = false;
    }
    
    // overloaded constructor for objects which can be interacted with
    public ItemDetails(string uiName, ItemData itemData, bool canDrop, bool canInteract)
    {
        this.uiName = uiName;
        this.itemData = itemData;
        this.canDrop = canDrop;
        this.canInteract = canInteract;
    }

    public override string ToString()
    {
        return itemData.itemName;
    }

    // getters & setters
    public void setCanDrop(bool canDrop)
    {
        this.canDrop = canDrop;
    }

    public bool isDroppable()
    {
        return canDrop;
    }

    public void setCanInteract(bool canInteract)
    {
        this.canInteract = canInteract;
    }

    public bool getCanInteract()
    {
        return canInteract;
    }
}

public enum InventoryChangeType
{
    Pickup, 
    Drop
}

public enum ItemInventoryType // using an enum to differentiate between the kinds of inventory in case we need to expand this more in the future
{
    Fish,
    Bait
}

//public delegate void OnInventoryChangeDelegate(int id, ItemDetails itemDetails, InventoryChangeType inventoryChangeType, ItemInventoryType inventoryType); // delegate to handle when inventory state is changed
public class inventoryController : MonoBehaviour
{
    // setting these as static because we should only ever have 1 inventoryController, which should be accessible everywhere
    private static Dictionary<int, ItemDetails> currentInventory = new Dictionary<int, ItemDetails>(); // int will function as location in the inventory
    public void Start()
    {
        //populate with dummy data for now 
        populateInventory(); // TODO : remove this - Dictionary should be maintained in game state to keep fish in consistent inventory slots
        foreach (var item in currentInventory)
        {
            tabbedInventoryUIController.onInventoryChanged(item.Key, item.Value, InventoryChangeType.Pickup, ItemInventoryType.Fish);
        }
    }


    public static void addItemToInventory(ItemDetails itemDetails, ItemInventoryType inventoryType)
    {
        // retrieve first available inventory slot
        List<int> usedKeys = new List<int>(currentInventory.Keys);
        int newIndex = 0;
        bool newIndexFound = false; 
        while (!newIndexFound && newIndex < 20)// we have 20 inventory slots available
        {
            if (usedKeys.Contains(newIndex))
            {
                newIndex++;
            }
            else
            {
                newIndexFound = true; // once we have a unique key, we can exit the loop
            }
        }
        if (newIndexFound)
        {
            // call method in tabbedInventoryUIController, which is responsible for displaying inventory
            tabbedInventoryUIController.onInventoryChanged(newIndex, itemDetails, InventoryChangeType.Pickup, inventoryType);
            currentInventory.Add(newIndex, itemDetails); // update our record of the inventory
        }
        else // we have run out of inventory slots
        {
            Debug.Log("No available inventory left");
        }
    }

    //method to remove backend object, not blank the icon in the inventory
    public static void removeItemFromInventory(int index)
    {
        currentInventory.Remove(index);
    }

    // GETTERS + SETTERS
    public static ItemDetails getItemByLocation(int loc)
    {
        return currentInventory[loc];
    }

    // TEST FUNCTION
    private void populateInventory()
    {
        Debug.Log("Populating inventory with dummy data");
        ItemDetails arowana = ItemManager.getItemByName("arowana");
        arowana.setCanDrop(false);
        inventoryController.addItemToInventory(arowana, ItemInventoryType.Fish);
        inventoryController.addItemToInventory(ItemManager.getItemByName("tilapia"), ItemInventoryType.Fish);
        inventoryController.addItemToInventory(ItemManager.getItemByName("lobster-trap"), ItemInventoryType.Fish);
    }
}
