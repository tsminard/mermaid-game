using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.InputSystem;

// class that builds inventoryUI and holds logic for adding items to that inventoryUI
public class tabbedInventoryUIController : MonoBehaviour
{
    private VisualElement root;
    public VisualElement slotVisualElement;
    public VisualElement fishSlotVisualElement;

    // keep track of inventory ids so they will always render in the same order
    private static Dictionary<int, InventorySlot> inventorySlotsById = new Dictionary<int, InventorySlot>();
    private static List<InventorySlot> inventorySlots = new List<InventorySlot>(); // runs 0-4

    private static Dictionary<int, FishInventorySlot> fishInventorySlotsById = new Dictionary<int, FishInventorySlot>();// runs 0-14
    private static List<FishInventorySlot> fishInventorySlots = new List<FishInventorySlot>();  

    public int numInventorySlots;
    private int numFishSlots = 15;
    
    // handles displaying selected slot
    private static int selectedSlotId = 0; // All slots are ordered together for selection (bait and fish) running 0-20
    private string selectedSlotUssName = "selectedSlotContainer";

    // handle inventory descriptions and actions
    private static InventoryTextBox inventoryTextBox;
    private static InventoryActionsBox inventoryActionsBox;
    private bool isBaitSection = true;

    // handle manuevering through the inventory via arrow keys
    [SerializeField]
    GameObject gameManager;
    PlayerInput playerInput;
    InputAction inputAction;

    private void Awake()
    {
        // build inventory slots for various baits
        root = GetComponent<UIDocument>().rootVisualElement; // retrieve root from UI document
        slotVisualElement = root.Query("SlotsContainer");
        buildInventorySlots();

        // initialise textBox for UI display
        VisualElement textBoxVisualElement = root.Query("SlotTextContainer");
        inventoryTextBox = new InventoryTextBox(textBoxVisualElement);

        //initialise actionsBox for UI display
        VisualElement actionBoxVisualElement = root.Query("SlotActionsContainer");
        inventoryActionsBox = new InventoryActionsBox(actionBoxVisualElement);

        // build fish slots for collected fish
        fishSlotVisualElement = root.Query("FishInventoryContainer");
        buildFishInventorySlots();

        // indicate which slot was last selected
        InventorySlot selectedSlot = inventorySlotsById[selectedSlotId];
        selectedSlot.AddToClassList(selectedSlotUssName);

        // retrieve components needed for inventory navigation
        playerInput = gameManager.GetComponent<PlayerInput>();
        inputAction = playerInput.actions.FindAction("NavigateMenu");


    }

    private void buildInventorySlots() // helper method for building bait inventory slots
    {
        numInventorySlots = 5;

        for (int i = 0; i < numInventorySlots; i++)
        {
            InventorySlot itemSlot = new InventorySlot(i); // keep track of inventory ids
            inventorySlotsById.Add(i, itemSlot);
            inventorySlots.Add(itemSlot); // keep track of this in list form for convenience
            slotVisualElement.Add(itemSlot);
        }
    }

    private void buildFishInventorySlots() // helper method for building fish-holding inventory slots
    {
        for (int i = 0; i < numFishSlots; i++)
        {
            FishInventorySlot fishSlot = new FishInventorySlot(i);
            fishInventorySlotsById.Add(i, fishSlot);
            fishInventorySlots.Add(fishSlot);
            fishSlotVisualElement.Add(fishSlot);
        }
    }

    public static void onInventoryChanged(int id, ItemDetails itemDetails, InventoryChangeType inventoryChangeType, ItemInventoryType whichInventory)
    {
        if (inventoryChangeType == InventoryChangeType.Pickup)
        {
            bool addedToInventory = false;
            if (whichInventory == ItemInventoryType.Bait)
            {
                InventorySlot inventorySlot = inventorySlotsById[id];
                if (inventorySlot != null)
                {
                    inventorySlot.holdItem(itemDetails);
                    addedToInventory = true;
                }
            }
            else if (whichInventory == ItemInventoryType.Fish)
            {
                FishInventorySlot fishInventorySlot = fishInventorySlotsById[id];
                if (fishInventorySlot != null)
                {
                    fishInventorySlot.holdItem(itemDetails);
                    addedToInventory = true;
                }
            }
            else
            {
                Debug.Log("INVALID INVENTORY TYPE");
            }
            if (!addedToInventory)
            {
                Debug.Log("Could not find a valid inventory slot with ID " + id + " : unable to store item");
            }
        }
        else if (inventoryChangeType == InventoryChangeType.Drop)
        {
            if (whichInventory == ItemInventoryType.Bait)
            {
                InventorySlot inventorySlot = inventorySlotsById[id];
                if (inventorySlot != null)
                {
                    inventorySlot.dropItem();
                }
            }
            else if (whichInventory == ItemInventoryType.Fish)
            {
                FishInventorySlot fishInventorySlot = fishInventorySlotsById[id];
                if (fishInventorySlot != null)
                {
                    fishInventorySlot.dropItem();
                }
            }
        }
    }

    // methods to handle UI input 
    public void OnNavigateMenu()
    {
        Vector2 xyValue = inputAction.ReadValue<Vector2>();
        int newSelectedSlotId = selectedSlotId;
        if(xyValue.x > 0)
        {
            newSelectedSlotId += 1; // slots are ordered left to right
        }
        else if(xyValue.x < 0)
        {
            newSelectedSlotId -= 1; 
        }

        if(xyValue.y > 0) 
        {
            if(selectedSlotId > 4) newSelectedSlotId -= 5; // slots are ordered top to bottom
        }
        else if(xyValue.y < 0)
        {
            if (selectedSlotId < 15) newSelectedSlotId += 5;
        }

        if(newSelectedSlotId >= 0 && newSelectedSlotId < 20)
        {
            changeSelectedSlot(newSelectedSlotId); // associate uss to new selected slot and remove from last selected slot
        }

        // check if we've transitioned between bait and fish sections
        if (hasInventoryTypeChanged(newSelectedSlotId))
        {
            if (isBaitSection)
            {
                inventoryActionsBox.toggleInteractActionName(InteractionName.Equip);
            }
            else
            {
                inventoryActionsBox.toggleInteractActionName(InteractionName.Interact);
            }
        }
    }

    // method to select action to apply to inventory object
    public void OnNavigateSubMenu()
    {
        if (!isCurrentSelectedSlotEmpty())
        {
            Vector2 xyValue = inputAction.ReadValue<Vector2>();
            inventoryActionsBox.changeSelectedField((int)xyValue.y);
        }
    }

    // method to indicate submenu is no longer being navigated
    public static void OnCeaseNavigateSubMenu()
    {
        inventoryActionsBox.unselectCurrOption();
    }

    // method to apply the action selected
    public void OnSelectSubMenu()
    {
        inventoryActionsBox.applySelectedSlot();
    }
    

    // helper method to remove Uss from old selected slot and apply it to a new one
    private void changeSelectedSlot(int newSelectedSlotId)
    {
        InventorySlot lastSelectedSlot = retrieveSlotFromAllInventories(selectedSlotId);
        lastSelectedSlot.RemoveFromClassList(selectedSlotUssName);

        InventorySlot newSelectedSlot = retrieveSlotFromAllInventories(newSelectedSlotId);
        newSelectedSlot.AddToClassList(selectedSlotUssName);
        if (!newSelectedSlot.isEmpty())
        {
            newSelectedSlot.displayText(inventoryTextBox);
        }
        else
        {
            inventoryTextBox.blankTextBox(); 
        }
        selectedSlotId = newSelectedSlotId;
    }

    // we have both Inventory slots and Fish Inventory slots.
    // the player should be able to move between both of them freely
    private static InventorySlot retrieveSlotFromAllInventories(int slotIndex)
    {
        if(slotIndex < 5)
        {
            return inventorySlotsById[slotIndex];
        }
        else if(slotIndex < 20)
        {
            return fishInventorySlotsById[slotIndex - 5];
        }
        else
        {
            Debug.Log("INVALID SLOT ID : " + slotIndex);
            return null; 
        }
    }

    // HELPER METHODS
    // helper method to check whether selected id has moved between our two inventory types
    // this is important for changing the inventory action text
    private bool hasInventoryTypeChanged(int newSelectedSlot)
    {
        if (newSelectedSlot > 4 && isBaitSection) // we have changed our location from fish inventory to bait inventory
        {
            isBaitSection = false;
            return true;
        }
        else if (newSelectedSlot < 4 && !isBaitSection) // we have changed our location from bait inventory to fish inventory
        {
            isBaitSection = true;
            return true;
        }
        return false;
    }

    // GETTERS + SETTERS
    public static int returnCurrentSelectedSlot()
    {
        return selectedSlotId;
    }

    public static bool isCurrentSelectedSlotEmpty()
    {
        InventorySlot selectedSlot = retrieveSlotFromAllInventories(selectedSlotId);
        return selectedSlot.isEmpty();
    }

    public static bool isCurrentSelectedSlotDroppable()
    {
        InventorySlot selectedSlot = retrieveSlotFromAllInventories(selectedSlotId);
        return selectedSlot.isDroppable();
    }

    public static void setUIInventoryTextBoxDescription(string newText) 
    {
        inventoryTextBox.changeTextDescription(newText);
    }
}
