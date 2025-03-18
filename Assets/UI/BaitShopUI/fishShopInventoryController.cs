using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;


// script to generate and run fishmonger UI
public class fishShopInventoryController : baitShopInventoryController
{
    private float resaleDiscount = 0.75f; // value that you can sell your items back for
    private double itemPrice = 0; 
    public override void Awake()
    {
        setFishShopVariables();
        root = GetComponent<UIDocument>().rootVisualElement;
        buildItemSlots(waresRootName);

        setActionVisualElements(this, descriptionBoxName, purchaseBoxName, noOptionName, yesOptionName);
        setMoneyVisualElements(this, moneyBoxName);
        setShopkeeperVisualElements(this);

        // retrieve input action
        inputAction = gameManager.GetComponent<PlayerInput>().actions.FindAction("NavigateMenu");
    }

    // methods to handle player interaction
    public override void onSelection()
    {
        populateShopWithInventory(); // populate new item slots with the player's inventory
        base.onSelection();
    }

    protected override void changeSlotText(WaresSlot newSlot)
    {
        if (!newSlot.isEmpty())
        {
            if (newSlot.isItemSold())
            {
                newSlot.displayText(inventoryTextBox);
                inventoryTextBox.changeTextDescription("I'll put this to good use (probably)");
                shopkeeperSpeechLabel.text = "Coolio, thanks";
            }
            else
            {
                newSlot.displayText(inventoryTextBox);
                float currItemPrice = newSlot.getSlotItem().itemData.value;
                float resaleItemPrice = currItemPrice * resaleDiscount;
                itemPrice = Math.Truncate(100 * resaleItemPrice) / 100;
                string priceInfo = shopkeeperSpeech.Insert(insertionIndex, itemPrice.ToString());
                shopkeeperSpeechLabel.text = priceInfo;
            }
        }
        else
        {
            inventoryTextBox.blankTextBox();
            shopkeeperSpeechLabel.text = "Man, you don't really have anything to sell...";
        }
    }

    // helper methods

    // overriden methods
    protected override void displayCurrentText()
    {
        WaresSlot currSlot = waresSlotsById[selectedSlotId];
        if (!currSlot.isEmpty())
        {
            currSlot.displayText(inventoryTextBox);
        }
        else
        {
            shopkeeperSpeechLabel.text = "Man, you don't really have anything to sell...";
        }
    }

    protected override void attemptAction(WaresSlot wareSlot)
    {
        if (wareSlot.isItemSold())
        {
            inventoryTextBox.changeTextDescription("Dude, you already sold that to me");
        }
        else
        {
            if (willPurchase) // logic for selling item to the fish monger
            {
                Debug.Log("Attempting to sell item");
                wareSlot.sellItem();
                ItemDetails currItem = wareSlot.getSlotItem();
                PersistData.Instance.removeItemFromInventory(currItem);
                PersistData.Instance.addMoney((float)itemPrice);
                refreshMoney();
            }
        }
    }

    // class-specific methods
    private void populateShopWithInventory()
    {
        Dictionary<int, ItemDetails> currInventory = PersistData.Instance.retrieveInventoryContents();
        int numItems = 0; 
        foreach(var itemTuple in currInventory)
        {
            int index = itemTuple.Key;
            ItemDetails item = itemTuple.Value;
            WaresSlot slot = waresSlotsById[index];
            slot.holdItem(item);
            numItems++; 
        }
        if(numItems == 0)
        {
            base.shopkeeperSpeechLabel.text = "Man, you don't really have anything to sell...";
        }
    }
    private void setFishShopVariables()
    {
        waresRootName = "WaresForSale"; // container for all ware items
        descriptionBoxName = "SoldItemDescriptionBox"; // container for item descriptions
        purchaseBoxName = "SellBox";
        noOptionName = "SellOptionNo";
        yesOptionName = "SellOptionYes";
        moneyBoxName = "SellMoneyBox";

        shopkeeperSpeech = "I guess I could give you $ for it ?";
        insertionIndex = 25;
    }
}
