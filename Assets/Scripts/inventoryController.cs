using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDetails
{
    public ItemData itemData;
    public float value; 
    public bool canDrop; // TODO : not sure if we'll need this but i'll keep it in for now

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

public delegate void OnInventoryChangeDelegate(int id, ItemDetails itemDetails, InventoryChangeType inventoryChangeType); // delegate to handle when inventory state is changed
public class inventoryController : MonoBehaviour
{
    // setting these as static because we should only ever have 1 inventoryController, which should be accessible everywhere
    private static Dictionary<int, ItemDetails> currentInventory = new Dictionary<int, ItemDetails>(); // int will function as location in the inventory
    public static event OnInventoryChangeDelegate onInventoryChanged; // blank delegate to avoid np exception
    public void Start()
    {
        //populate with dummy data for now 
        populateInventory(); // TODO : remove this - Dictionary should be maintained in game state to keep fish in consistent inventory slots
        foreach (var item in currentInventory)
        {
            Debug.Log("Adding item " + item.Value.ToString());
            onInventoryChanged(item.Key, item.Value, InventoryChangeType.Pickup);
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

        ItemData arowana = (ItemData) ScriptableObject.CreateInstance("ItemData");

        arowana.init("Arowana", Resources.Load<Sprite>("Sprites/arowana"), "A large fish");

        ItemData koi = (ItemData)ScriptableObject.CreateInstance("ItemData");
        koi.init("Koi", Resources.Load<Sprite>("Sprites/koi"), "A stately fish");

        ItemData tilapia = (ItemData)ScriptableObject.CreateInstance("ItemData");
        tilapia.init("???", Resources.Load<Sprite>("Sprites/tilapia"));

        currentInventory.Add(0, new ItemDetails()
        {
            itemData = arowana,
            value = 25f,
            canDrop = true
        });
        currentInventory.Add(1, new ItemDetails()
        {
            itemData = koi,
            value = 15f,
            canDrop = true
        });
        currentInventory.Add(2, new ItemDetails()
        {
            itemData = tilapia,
            value = 100f,
            canDrop = false
        });
    }
}
