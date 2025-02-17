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
    private Dictionary<int, InventorySlot> inventorySlotsById = new Dictionary<int, InventorySlot>();
    private List<InventorySlot> inventorySlots = new List<InventorySlot>();

    private Dictionary<int, FishInventorySlot> fishInventorySlotsById = new Dictionary<int, FishInventorySlot>();
    private List<FishInventorySlot> fishInventorySlots = new List<FishInventorySlot>();  

    public int numInventorySlots;
    private int numFishSlots = 15;
    
    // handles displaying selected slot
    private int selectedSlotId = 0; // All slots are ordered together for selection (lure and fish)
    private string selectedSlotUssName = "selectedSlotContainer";

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

        inventoryController.onInventoryChanged += inventoryController_onInventoryChanged;  // add event listener so our UI will change when items change

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

    private void inventoryController_onInventoryChanged(int id, ItemDetails itemDetails, InventoryChangeType inventoryChangeType, ItemInventoryType whichInventory)
    {
        Debug.Log("calling OnInventoryChanged");
        if(inventoryChangeType == InventoryChangeType.Pickup)
        {
            bool addedToInventory = false; 
            if(whichInventory == ItemInventoryType.Bait)
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
                if(fishInventorySlot != null)
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
    }

    // methods to handle UI input 
    public void OnNavigateMenu()
    {
        Debug.Log("OnNavigateMenu called");
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
            if (selectedSlotId < 10) newSelectedSlotId += 5;
        }
        changeSelectedSlot(newSelectedSlotId); // associate uss to new selected slot and remove from last selected slot
    }

    // helper method to remove Uss from old selected slot and apply it to a new one
    private void changeSelectedSlot(int newSelectedSlotId)
    {
        Debug.Log("Changing selected slot from " + selectedSlotId + " to " + newSelectedSlotId);
        InventorySlot lastSelectedSlot = inventorySlotsById[selectedSlotId];
        lastSelectedSlot.RemoveFromClassList(selectedSlotUssName);

        InventorySlot newSelectedSlot = inventorySlotsById[newSelectedSlotId];
        newSelectedSlot.AddToClassList(selectedSlotUssName);

        selectedSlotId = newSelectedSlotId; 
    }
}
