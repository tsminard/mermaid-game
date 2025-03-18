using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// script to generate and run shop UI
public class baitShopInventoryController : MonoBehaviour
{
    protected VisualElement root;
    protected VisualElement waresRoot;
    protected VisualElement itemDescriptionBox;
    protected VisualElement purchaseBox;
    protected VisualElement noOption;
    protected VisualElement yesOption;
    protected Label moneyBoxLabel;
    protected Label shopkeeperSpeechLabel;

    // values for display
    protected static int numWareSlots = 15;

    // values for handling purchases
    protected float currMoney = 0f;

    // values for keeping track of items displayed
    protected Dictionary<int, WaresSlot> waresSlotsById = new Dictionary<int, WaresSlot>();

    // values for interacting with inventory
    [SerializeField]
    protected GameObject gameManager;
    protected InputAction inputAction;
    protected bool isSlotSelected = false;
    protected bool willPurchase = false;
    protected int selectedSlotId = 0;

    protected string shopkeeperSpeech = "That'll set you back $, buddy.";
    protected private int insertionIndex = 21; // keeps track of where we should insert the price into the above text

    protected InventoryTextBox inventoryTextBox;

    // names of slots that we will be filling
    protected string waresRootName = "Wares"; // container for all ware items
    protected string descriptionBoxName = "ItemDescriptionBox"; // container for item descriptions
    protected string purchaseBoxName = "BuyBox";
    protected string noOptionName = "OptionNo";
    protected string yesOptionName = "OptionYes";
    protected string moneyBoxName = "MoneyBox";
    protected string shopkeeperSpeechName = "SpeechBubble";
    protected string selectedSlotUssName = "selectedSlotContainer";
    protected string selectedOptionUssName = "selectedOptionContainer";
    
    public virtual void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        buildItemSlots(waresRootName); // retrieve item slot VisualElements and build item slots
        testAddingBait(); 

        // retrieve other VisualElements
        setActionVisualElements(this, descriptionBoxName, purchaseBoxName, noOptionName, yesOptionName);

        // retrieve VisualElements for displaying money
        setMoneyVisualElements(this, moneyBoxName);

        // retrieve VisualElements for handling shopkeeper interactions
        setShopkeeperVisualElements(this);

        // the following methods depend on InventoryTextBox so have to be called after its instantiation
        // highlight current selected slot
        changeSelectedSlot(selectedSlotId);
        // display text for current slot
        displayCurrentText();

        // retrieve input action
        inputAction = gameManager.GetComponent<PlayerInput>().actions.FindAction("NavigateMenu");
    }

    protected void buildWareSlots()
    {
        for(int i = 0; i < numWareSlots; i++)
        {
            WaresSlot wareSlot = new WaresSlot(i);
            waresSlotsById.Add(i, wareSlot);
            waresRoot.Add(wareSlot);
        }
    }

    // UI handling methods
    // method to handle setting up UI when switching back to it
    public virtual void onSelection()
    {
        changeSelectedSlot(selectedSlotId);
        // display text for current slot
        displayCurrentText();
        refreshMoney();
    }

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
    // build UI methods

    // retrieves item-containing visual element and builds item slots
    protected void buildItemSlots(string rootName) 
    {
        waresRoot = root.Query(rootName);
        buildWareSlots();
    }

    // retrieves visual elements for actions players can take on this UI
    protected void setActionVisualElements(baitShopInventoryController obj, string descriptionBoxName, string purchaseBoxName, string noOptionName, string yesOptionName)
    {
        obj.itemDescriptionBox = root.Query(descriptionBoxName);
        obj.purchaseBox = root.Query(purchaseBoxName);
        obj.noOption = root.Query(noOptionName);
        obj.yesOption = root.Query(yesOptionName);

        // handle displaying text
        obj.inventoryTextBox = new InventoryTextBox(itemDescriptionBox);
    }

    // retrieves visual elements for displaying current money 
    protected void setMoneyVisualElements(baitShopInventoryController obj, string moneyBoxName)
    {
        VisualElement moneyBox = root.Query(moneyBoxName);
        obj.moneyBoxLabel = moneyBox.Query<Label>().First();

        // retrieve saved information and display it
        obj.currMoney = PersistData.Instance.getCurrentMoney();
        obj.moneyBoxLabel.text = currMoney.ToString() + "$";
    }

    // retrieves visual elements for displaying shopkeeper interactions
    protected void setShopkeeperVisualElements(baitShopInventoryController obj)
    {
        VisualElement speechBubble = root.Query(shopkeeperSpeechName);
        obj.shopkeeperSpeechLabel = speechBubble.Query<Label>().First();
    }

    // Change contents of UI helper methods
    protected void addItemToShop(int slotId, ItemDetails bait)
    {
        WaresSlot wareSlot;
        waresSlotsById.TryGetValue(slotId, out wareSlot);
        wareSlot.holdItem(bait);
    }

    // UI helper methods
    protected void refreshMoney()
    {
        currMoney = PersistData.Instance.getCurrentMoney();
        moneyBoxLabel.text = currMoney.ToString() + "$";
    }

    protected virtual void displayCurrentText()
    {
        WaresSlot currSlot = waresSlotsById[selectedSlotId];
        if (!currSlot.isEmpty())
        {
            currSlot.displayText(inventoryTextBox);
        }
    }

    protected void changeSelectedSlot(int newSlotId) // change selected item slot
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
        changeSlotText(newSlot); // this method will be overriden for other UIs in this scene
    }

    protected virtual void changeSlotText(WaresSlot newSlot)
    {
        if (!newSlot.isEmpty())
        {
            if (newSlot.isItemSold())
            {
                newSlot.displayText(inventoryTextBox);
                inventoryTextBox.changeTextDescription("Thanks for your purchase!");
                shopkeeperSpeechLabel.text = "Pleasure doing business with ya";
            }
            else
            {
                newSlot.displayText(inventoryTextBox);
                float currItemPrice = newSlot.getSlotItem().itemData.value;
                string priceInfo = shopkeeperSpeech.Insert(insertionIndex, currItemPrice.ToString());
                shopkeeperSpeechLabel.text = priceInfo;
            }
        }
        else
        {
            inventoryTextBox.blankTextBox();
            shopkeeperSpeechLabel.text = "We're still waiting on some inventory to come in...";
        }
    }

    protected void changeSubMenuSlot(float x)
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

    protected void toggleSelectedSlot(WaresSlot waresSlot, bool isSelected)
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

    protected void toggleBuySlot()
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
            attemptAction(currWareSlot);
            // unselected everything regardless
            purchaseBox.RemoveFromClassList(selectedSlotUssName); 
            noOption.RemoveFromClassList(selectedOptionUssName);
            yesOption.RemoveFromClassList(selectedOptionUssName);
            isSlotSelected = false;
            willPurchase = false;
        }
    }

    // this method is meant to encapsulate any action being performed on a slot
    // this is important so it can be overriden by child classes which represent other UIs
    protected virtual void attemptAction(WaresSlot wareSlot)
    {
        if (wareSlot.isItemSold())
        {
            Debug.Log("Item already sold");
            inventoryTextBox.changeTextDescription("Thanks for your purchase!");
        }
        else
        {
            if (willPurchase) // put all purchasing logic here !
            {
                Debug.Log("Attempting to purchase item!");
                ItemDetails currItem = wareSlot.getSlotItem();
                // check if the player has enough money 
                if (currMoney > currItem.itemData.value) // if the player doesn't have enough money, display new text and break out
                {
                    // if player has enough money, check if player has enough inventory space
                    int newInventoryIndex = PersistData.Instance.generateNewInventoryIndex(ItemInventoryType.Bait);
                    if (newInventoryIndex != -1) // add the item to our PERSISTED INVENTORY
                    {
                        sellWare(wareSlot, currItem, newInventoryIndex);
                    }
                    else // change our message to indicate that we cannot purchase anything 
                    {
                        inventoryTextBox.changeTextDescription("Oops, looks like your bag is already full !");
                    }
                }
                else
                {
                    shopkeeperSpeechLabel.text = "Oh, bummer - not enough cash.";
                }
            }
            else
            {
                Debug.Log("Not purchasing item");
            }
        }
    }

    // helper method to compress all actions related to playing buying an item
    private void sellWare(WaresSlot wareSlot, ItemDetails item, int inventoryIndex)
    {
        wareSlot.sellItem(); // set sprite to indicate sold
        shopkeeperSpeechLabel.text = "Sweet, thanks !";
        moneyBoxLabel.text = (currMoney - item.itemData.value).ToString() + "$";
        // persist data from this interaction
        PersistData.Instance.addItemToInventory(inventoryIndex, item); // save item to inventory
        PersistData.Instance.removeMoney(item.itemData.value); // persist reduced amount of money
    }

    // GETTERS + SETTERS
    public bool isBaitShopSlotSelected()
    {
        return isSlotSelected;
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
