using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

// classes that handle player current inventory contents
public class ItemDetails // wrapper class which  additionally contains canDrop + interaction options - specifically for inventory purposes
{
    public string uiName; // name for display in UI
    public ItemData itemData;
    private bool canDrop; // TODO : not sure if we'll need this but i'll keep it in for now
    private bool canInteract;
    public delegate string interactionDelegate(); // interaction always returns a String which represents the new text in the UI description 
    interactionDelegate interact; 

    public ItemDetails(string uiName, ItemData itemData, bool canDrop)
    {
        this.uiName = uiName;
        this.itemData = itemData;
        this.canDrop = canDrop;
        interact = defaultInteract; 
        canInteract = false;
    }
    
    // overloaded constructor for objects which can be interacted with
    public ItemDetails(string uiName, ItemData itemData, bool canDrop, bool canInteract)
    {
        this.uiName = uiName;
        this.itemData = itemData;
        this.canDrop = canDrop;
        interact = defaultInteract; // if you want a different interaction, you should set this
        this.canInteract = canInteract;
    }

    public string interactWith()
    {
        return interact(); 
    }

    public override string ToString()
    {
        return itemData.itemName;
    }


    // default interact action 
    private string defaultInteract()
    {
        return "It seems perfectly ordinary...";
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

    public void setInteractionFunction(interactionDelegate newInteraction)
    {
        interact = newInteraction;
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
    // we need to save our inventory when the scene closes
    PersistData persistData;
    ItemManager itemManager; // singleton class
    public void Start()
    {
        persistData = PersistData.Instance;
        itemManager = ItemManager.Instance;
        UnityEditor.SceneManagement.EditorSceneManager.sceneClosing += onSceneClose; // listener to closing scene
                                                                                     // retrieve any persisted data
        loadSavedInventory();
        if (currentInventory.Count == 0)
        {
            populateInventory(); // TODO : remove this - Dictionary should be maintained in game state to keep fish in consistent inventory slots
        }
        foreach (var item in currentInventory)
        {
            tabbedInventoryUIController.onInventoryChanged(item.Key, item.Value, InventoryChangeType.Pickup, ItemInventoryType.Fish);
        }
    }

    // generic method to add item to inventory without designated index
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

    // more specialised method to add an object to inventory at a given location
    public static void addItemToInventory(int index,ItemDetails itemDetails, ItemInventoryType inventoryType)
    {
        if((inventoryType == ItemInventoryType.Bait && index > 4) ||
                (inventoryType == ItemInventoryType.Fish && index > 14)){
            Debug.Log("Invalid index of " + index + " for inventory type " + inventoryType.ToString());
            return;
        }
        else
        {
            tabbedInventoryUIController.onInventoryChanged(index, itemDetails, InventoryChangeType.Pickup, inventoryType);
            currentInventory.Add(index, itemDetails); // update our record of the inventory
        }
    }

    //method to remove backend object, not blank the icon in the inventory
    // this is a BARE BONES METHOD that exists to purely remove an item from our backend
    public static void removeItemFromInventory(int index)
    {
        currentInventory.Remove(index);
    }


    // this is a more fleshed out, reuseable method which deletes the item from the backend AND frontend inventory after normalising for inventory type
    // param : index = selected slot in the UNIVERSAL INVENTORY UI INDEX
    public static void dropItemFromInventory(int currInventoryIndex)
    {
        ItemInventoryType whichInventory = currInventoryIndex < 5 ? ItemInventoryType.Bait : ItemInventoryType.Fish;
        int correctedCurrInventoryIndex = whichInventory == ItemInventoryType.Bait ? currInventoryIndex : currInventoryIndex - 5;
        inventoryController.removeItemFromInventory(correctedCurrInventoryIndex); // remove inventory from backend representation
        tabbedInventoryUIController.onInventoryChanged(correctedCurrInventoryIndex, null, InventoryChangeType.Drop, whichInventory); // update UI to indicate item has been dropped
    }

    // method to retrieve any saved inventory
    private void loadSavedInventory()
    {
        Debug.Log("Retrieving inventory");
        currentInventory = persistData.retrieveInventoryContents();
    }

    // method to save our inventory to an undestroyed object when the scene is closed
    private void onSceneClose(Scene scene, bool removingScene)
    {
        Debug.Log("Persisting inventory");
        persistData.saveInventoryContents(currentInventory);
    }

    // helper methods
    // method to handle normalizing inventory index between bait and fish 
    // note to self - never do things this way again :weary: 
    public static int normalizeInventorySlot(int inventoryIndex)
    {
        ItemInventoryType whichInventory = inventoryIndex < 5 ? ItemInventoryType.Bait : ItemInventoryType.Fish;
        int correctedInventoryIndex = whichInventory == ItemInventoryType.Bait ? inventoryIndex : inventoryIndex - 5;
        return correctedInventoryIndex; 
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
        inventoryController.addItemToInventory(itemManager.getItemByName("arowana"), ItemInventoryType.Fish);
        inventoryController.addItemToInventory(itemManager.getItemByName("tilapia"), ItemInventoryType.Fish);
        inventoryController.addItemToInventory(itemManager.getItemByName("lobster-trap"), ItemInventoryType.Fish);
        inventoryController.addItemToInventory(itemManager.getItemByName("message-in-bottle"), ItemInventoryType.Fish);
    }
}
