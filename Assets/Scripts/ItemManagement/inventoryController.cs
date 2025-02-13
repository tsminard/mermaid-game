using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// classes that handle player current inventory contents
public class ItemDetails // wrapper class which only additionally contains canDrop - specifically for inventory purposes
{
    public ItemData itemData;
    public bool canDrop; // TODO : not sure if we'll need this but i'll keep it in for now

    public ItemDetails(ItemData itemData, bool canDrop)
    {
        this.itemData = itemData;
        this.canDrop = canDrop;
    }
    

    public override string ToString()
    {
        return itemData.itemName;
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

public delegate void OnInventoryChangeDelegate(int id, ItemDetails itemDetails, InventoryChangeType inventoryChangeType, ItemInventoryType inventoryType); // delegate to handle when inventory state is changed
public class inventoryController : MonoBehaviour
{
    // setting these as static because we should only ever have 1 inventoryController, which should be accessible everywhere
    private static Dictionary<int, ItemDetails> currentInventory = new Dictionary<int, ItemDetails>(); // int will function as location in the inventory
    public static event OnInventoryChangeDelegate onInventoryChanged; // blank delegate to avoid np exception
    public void Start()
    {
        //populate with dummy data for now 
        //populateInventory(); // TODO : remove this - Dictionary should be maintained in game state to keep fish in consistent inventory slots
        foreach (var item in currentInventory)
        {
            Debug.Log("Adding item " + item.Value.ToString());
            onInventoryChanged(item.Key, item.Value, InventoryChangeType.Pickup, ItemInventoryType.Bait);
        }
    }


    public static void addItemToInventory(ItemDetails itemDetails, ItemInventoryType inventoryType)
    {
        Debug.Log("Adding item " + itemDetails.ToString() + " to inventory");
        // retrieve first available inventory slot
        List<int> usedKeys = new List<int>(currentInventory.Keys);
        foreach(int val in usedKeys){
            Debug.Log("Used key : " + val);
        }
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
            onInventoryChanged(newIndex, itemDetails, InventoryChangeType.Pickup, inventoryType);
            currentInventory.Add(newIndex, itemDetails); // update our record of the inventory
        }
        else // we have run out of inventory slots
        {
            Debug.Log("No available inventory left");
        }
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
        currentInventory.Add(0, ItemManager.getItemByName("arowana")); 
    }
}
