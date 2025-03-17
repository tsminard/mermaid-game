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
    private VisualElement purchaseBox;
    private VisualElement noOption;
    private VisualElement yesOption;
    private Label moneyBoxLabel;

    // values for display
    private static int numWareSlots = 15;

    // values for keeping track of items displayed
    Dictionary<int, WaresSlot> waresSlotsById = new Dictionary<int, WaresSlot>();

    // values for interacting with inventory
    [SerializeField]
    GameObject gameManager;
    InputAction inputAction;
    public bool isSlotSelected = false;
    public bool willPurchase = false;

    private int selectedSlotId = 0;

    InventoryTextBox inventoryTextBox;

    // names of slots that we will be filling
    string waresRootName = "Wares"; // container for all ware items
    string descriptionBoxName = "ItemDescriptionBox"; // container for item descriptions
    string purchaseBoxName = "BuyBox";
    string noOptionName = "OptionNo";
    string yesOptionName = "OptionYes";
    string moneyBoxName = "MoneyBox";
    string selectedSlotUssName = "selectedSlotContainer";
    string selectedOptionUssName = "selectedOptionContainer";
    
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        waresRoot = root.Query(waresRootName);
        buildWareSlots();
        testAddingBait();

        // retrieve other VisualElements
        itemDescriptionBox = root.Query(descriptionBoxName);
        purchaseBox = root.Query(purchaseBoxName);
        noOption = root.Query(noOptionName);
        yesOption = root.Query(yesOptionName);

        VisualElement moneyBox = root.Query(moneyBoxName);
        moneyBoxLabel = moneyBox.Query<Label>().First();

        // handle displaying text
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

    // UI handling methods
    public void OnNavigateMenu() // navigate the item boxes if slot is not selected
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

    public void OnNavigateSubMenu() // navigate the submenu buy slots 
    {
        Vector2 xyValue = inputAction.ReadValue<Vector2>();
        changeSubMenuSlot(xyValue.x);
    }

    public void OnSubmit() // pressing the space key indicates an interaction with the current item - either to purchase, or not to purchase
    {
        toggleBuySlot();
    }
    
    // leaving the shop reloads the town scene
    public void OnCancel()
    {
        SceneManager.LoadScene(4);
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

    private void changeSelectedSlot(int newSlotId) // change selected item slot
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
            if (newSlot.isItemSold())
            {
                inventoryTextBox.changeTextDescription("Thanks for your purchase!");
            }
            else
            {
                newSlot.displayText(inventoryTextBox);
            }
        }
        else
        {
            inventoryTextBox.blankTextBox();
        }
    }

    private void changeSubMenuSlot(float x)
    {
        if(x > 0 && !willPurchase) // moving right
        {
            willPurchase = true;
            noOption.RemoveFromClassList(selectedOptionUssName);
            yesOption.AddToClassList(selectedOptionUssName);
        }
        else if (x < 0 && willPurchase) // move left
        {
            willPurchase = false;
            yesOption.RemoveFromClassList(selectedOptionUssName);
            noOption.AddToClassList(selectedOptionUssName);
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

    private void toggleBuySlot()
    {
        if (!isSlotSelected) // if we haven't selected a slot yet, we indicate selection by highlighting the object description
        {
            purchaseBox.AddToClassList(selectedSlotUssName); // indicate selection
            noOption.AddToClassList(selectedOptionUssName);
            isSlotSelected = true;
        }
        else // a slot is already selected and we are performing an action on it - either buying or not buying
        {
            WaresSlot currWareSlot = waresSlotsById[selectedSlotId];
            if (currWareSlot.isItemSold())
            {
                Debug.Log("Item already sold");
                inventoryTextBox.changeTextDescription("Thanks for your purchase!");
            }
            else
            {
                if (willPurchase)
                {
                    Debug.Log("Attempting to purchase item!");
                    // TODO : add purchasing logic here
                    int newInventoryIndex = PersistData.Instance.generateNewInventoryIndex(ItemInventoryType.Bait);
                    if (newInventoryIndex != -1) // add the item to our PERSISTED INVENTORY
                    {
                        currWareSlot.sellItem();
                        PersistData.Instance.addItemToInventory(newInventoryIndex, currWareSlot.getSlotItem());
                    }
                    else // change our message to indicate that we cannot purchase anything 
                    {
                        inventoryTextBox.changeTextDescription("Oops, looks like your bag is already full !");
                    }
                }
                else
                {
                    Debug.Log("Not purchasing item");
                }
            }
            // unselected everything regardless
            purchaseBox.RemoveFromClassList(selectedSlotUssName); 
            noOption.RemoveFromClassList(selectedOptionUssName);
            yesOption.RemoveFromClassList(selectedOptionUssName);
            isSlotSelected = false;
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
