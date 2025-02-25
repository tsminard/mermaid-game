using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

// script responsible for the visual element which provides possible inventory actions 
// Drop
// Swap <- should just swap with neighbor because i want this to be a keyboard-only game by default ( i like playing games on the sofa :3 )
// Interact 
public enum InteractionName
{
    Equip,
    Interact
}

public class InventoryActionsBox
{
    VisualElement visualElement;
    Label dropField;
    Label swapField;
    Label interactField;

    // variables to handle menu interactions
    Label[] fields = new Label[3]; // array to increment through the selected field
    int currIndex = 0;
    string selectedSlotUssName = "selectedSubMenuContainer";

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
        unselectCurrOption(); 
        // now change index
        if(direction < 0)
        {
            incrementSelectedFieldDown(); 
        }
        else if(direction > 0)
        {
            incrementSelectedFieldUp(); 
        }
        updateSelectedSlot(); 
    }

    private void incrementSelectedFieldUp()
    {
        if(currIndex == 0)
        {
            currIndex = 2;
        }
        else
        {
            currIndex = (currIndex -= 1) % 3;
        }
    }

    private void incrementSelectedFieldDown()
    {
        if(currIndex == 2)
        {
            currIndex = 0; 
        }
        else
        {
            currIndex = currIndex + 1;
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
                // we need to check if the item is droppable
                if (tabbedInventoryUIController.isCurrentSelectedSlotDroppable())
                {
                    ItemInventoryType whichInventory = currInventoryIndex < 5 ? ItemInventoryType.Bait : ItemInventoryType.Fish;
                    int correctedCurrInventoryIndex = whichInventory == ItemInventoryType.Bait ? currInventoryIndex : currInventoryIndex - 5;
                    inventoryController.removeItemFromInventory(correctedCurrInventoryIndex); // remove inventory from backend representation
                    tabbedInventoryUIController.onInventoryChanged(correctedCurrInventoryIndex, null, InventoryChangeType.Drop, whichInventory); // update UI to indicate item has been dropped
                }
                else // if the item is not droppable, we have to change the item description text to reflect this
                {
                    tabbedInventoryUIController.setUIInventoryTextBoxDescription("Cannot drop this object !"); 
                }
                break;
            case 1: // swap action
                break; 
            case 2: // interact action
                tabbedInventoryUIController.triggerInteract();
                break;
        }
        unselectCurrOption();
    }

    public void unselectCurrOption() // method to indicate that submenu is no longer being used at all 
    {
        Label currField = fields[currIndex];
        currField.RemoveFromClassList(selectedSlotUssName);
    }

    // helper methods
    // method to change label text depending on fish inventory ("interact") vs bait inventory ("equip")
    public void toggleInteractActionName(InteractionName interactionName)
    {
        string labelText = "";
        switch (interactionName)
        {
            case InteractionName.Equip:
                labelText = "Equip ?";
                break;
            case InteractionName.Interact:
                labelText = "Interact ?";
                break;
        }
        interactField.text = labelText;
    }

}
