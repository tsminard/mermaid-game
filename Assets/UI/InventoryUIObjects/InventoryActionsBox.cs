using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

// script responsible for the visual element which provides possible inventory actions 
// Drop
// Swap <- should just swap with neighbor because i want this to be a keyboard-only game by default ( i like playing games on the sofa :3 )
// Interact 

public class InventoryActionsBox
{
    VisualElement visualElement;
    Label dropField;
    Label swapField;
    Label interactField;
    // variables to handle menu interactions
    Label[] fields = new Label[3]; // array to increment through the selected field
    int currIndex = 0;
    string selectedSlotUssName = "selectedSlotContainer"; 

    public InventoryActionsBox(VisualElement visualElement)
    {
        this.visualElement = visualElement;
        List<Label> labels = this.visualElement.Query<Label>().ToList();

        if(labels.Count != 3)
        {
            Debug.Log("Incorrect number of labels found...");
            return;
        }

        // assign label objects
        dropField = labels[0];
        swapField = labels[1];
        interactField = labels[2];
        // set text
        dropField.text = "Drop ?";
        swapField.text = "Swap ?";
        interactField.text = "Interact ?";

        fields[0] = dropField;
        fields[1] = swapField;
        fields[2] = interactField;

        // Apply indicator of selected field
        Label currField = fields[currIndex];
        currField.AddToClassList(selectedSlotUssName);
    }

    // methods to handle element selection
    // menu options should loop over
    public void changeSelectedField(int direction)
    {
        Label currField = fields[currIndex];
        currField.RemoveFromClassList(selectedSlotUssName);
        // now change index
        if(direction < 0)
        {
            incrementSelectedFieldDown(); 
        }
        else if(direction > 0)
        {
            incrementSelectedFieldUp(); 
        }
        else
        {
            Debug.Log("Invalid 0 input to InventoryActionsBox - please check method call");
        }
        updateSelectedSlot(); 
    }

    private void incrementSelectedFieldUp()
    {
        currIndex = (currIndex += 1) % 3;
    }

    private void incrementSelectedFieldDown()
    {
        if(currIndex > 0)
        {
            currIndex -= 1;
        }
        else
        {
            currIndex = 2; 
        }
    }

    private void updateSelectedSlot()
    {
        Label newSelectedTextField = fields[currIndex];
        newSelectedTextField.AddToClassList(selectedSlotUssName);
    }

    public void applySelectedSlot()
    {
        switch (currIndex)
        {
            case 0: // drop action
                // to drop, we need to know which inventory item we're getting rid of 
                int currInventoryIndex = tabbedInventoryUIController.returnCurrentSelectedSlot();
                inventoryController.removeItemFromInventory(currInventoryIndex); // remove inventory from backend representation
                ItemInventoryType whichInventory = currInventoryIndex < 5 ? ItemInventoryType.Bait : ItemInventoryType.Fish;
                int correctedCurrInventoryIndex = whichInventory == ItemInventoryType.Bait ? currInventoryIndex : currInventoryIndex - 5;
                
                break;
            case 1: // swap action
                break; 
            case 2: // interact action
                break;
        }
    }
}
