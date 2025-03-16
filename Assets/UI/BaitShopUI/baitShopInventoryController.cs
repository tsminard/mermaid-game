using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// script to generate shop UI
public class baitShopInventoryController : MonoBehaviour
{
    private VisualElement root;
    private VisualElement waresRoot;
    private VisualElement itemDescriptionBox;

    // values for display
    private static int numWareSlots = 15;

    // values for keeping track of items displayed
    Dictionary<int, WaresSlot> waresSlotsById = new Dictionary<int, WaresSlot>();

    // values for interacting with inventory
    [SerializeField]
    GameObject gameManager;
    InputAction inputAction;

    private int selectedSlotId = 0;

    InventoryTextBox inventoryTextBox;

    // names of slots that we will be filling
    string waresRootName = "Wares"; // container for all ware items
    string descriptionBoxName = "ItemDescriptionBox"; // container for item descriptions
    string selectedSlotUssName = "selectedSlotContainer";
    
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        waresRoot = root.Query(waresRootName);
        Debug.Log("Creating Ware Slots");
        buildWareSlots();
        testAddingBait();

        // handle displaying text
        itemDescriptionBox = root.Query(descriptionBoxName);
        Debug.Log("Found item description box : " + itemDescriptionBox.name);
        inventoryTextBox = new InventoryTextBox(itemDescriptionBox);
        // the following methods depend on InventoryTextBox so have to be called after its instantiation
        // highlight current selected slot
        changeSelectedSlot(selectedSlotId);
        // display text for current slot
        displayCurrentText();

        // retrieve input action
        inputAction = gameManager.GetComponent<PlayerInput>().actions.FindAction("NavigateMenu");
    }

    private void buildWareSlots()
    {
        for(int i = 0; i < numWareSlots; i++)
        {
            WaresSlot wareSlot = new WaresSlot(i);
            waresSlotsById.Add(i, wareSlot);
            waresRoot.Add(wareSlot);
        }
    }

    // UI handling method
    public void OnNavigateMenu()
    {
        Vector2 xyValue = inputAction.ReadValue<Vector2>();
        int newSelectedSlotId = selectedSlotId; 
        if(xyValue.x > 0)
        {
            newSelectedSlotId += (selectedSlotId < numWareSlots - 1 ? 1 : 0);
            
        }
        else if (xyValue.x < 0)
        {
            newSelectedSlotId -= (selectedSlotId > 0 ? 1 : 0);
        }

        if(xyValue.y > 0)
        {
            newSelectedSlotId -= (selectedSlotId > 4 ? 5 : 0); ; // we have rows of 5 items
        }
        else if(xyValue.y < 0)
        {
            newSelectedSlotId += (selectedSlotId < numWareSlots - 5 ? 5 : 0);
        }
        if(newSelectedSlotId != selectedSlotId)
        {
            changeSelectedSlot(newSelectedSlotId); 
        }
    }
    
    // leaving the shop reloads the town scene
    public void OnCancel()
    {
        SceneManager.LoadScene(4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Helper methods
    // Change contents of UI helper methods
    private void addItemToShop(int slotId, ItemDetails bait)
    {
        WaresSlot wareSlot;
        waresSlotsById.TryGetValue(slotId, out wareSlot);
        wareSlot.holdItem(bait);
    }

    // UI helper methods
    private void displayCurrentText()
    {
        WaresSlot currSlot = waresSlotsById[selectedSlotId];
        if (!currSlot.isEmpty())
        {
            currSlot.displayText(inventoryTextBox);
        }
    }

    private void changeSelectedSlot(int newSlotId)
    {
        // add highlighting
        WaresSlot oldSlot;
        waresSlotsById.TryGetValue(selectedSlotId, out oldSlot);
        WaresSlot newSlot;
        waresSlotsById.TryGetValue(newSlotId, out newSlot);
        toggleSelectedSlot(oldSlot, false);
        toggleSelectedSlot(newSlot, true);
        selectedSlotId = newSlotId;
        // change text if slot contains bait
        if (!newSlot.isEmpty())
        {
            newSlot.displayText(inventoryTextBox);
        }
        else
        {
            inventoryTextBox.blankTextBox();
        }
    }

    private void toggleSelectedSlot(WaresSlot waresSlot, bool isSelected)
    {
        if (isSelected)
        {
            waresSlot.AddToClassList(selectedSlotUssName);
        }
        else
        {
            waresSlot.RemoveFromClassList(selectedSlotUssName);
        }
    }

    // TEST METHODS
    private void testAddingBait()
    {
        ItemDetails chumBucket = ItemManager.Instance.getBaitByName("chum-bucket");
        ItemDetails hotdog = ItemManager.Instance.getBaitByName("hotdog");
        addItemToShop(0, chumBucket);
        addItemToShop(1, hotdog);
    }
}
