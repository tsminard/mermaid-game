using UnityEngine;
using UnityEngine.UIElements;

// script responsible for the visual element which provides possible inventory actions 
// Drop
// Swap <- should just swap with neighbor because i want this to be a keyboard-only game by default ( i like playing games on the sofa :3 )
// Interact 

public class InventoryActionsBox : VisualElement
{
    TextField dropField;
    TextField swapField;
    TextField interactField;
    // variables to handle menu interactions
    TextField[] fields = new TextField[3]; // array to increment through the selected field
    int currIndex = 0;
    string selectedSlotUssName = "selectedSlotContainer"; 

    public InventoryActionsBox()
    {
        AddToClassList("slotActionsContainer");
        dropField = new TextField();
        swapField = new TextField();
        interactField = new TextField();

        dropField.value = "Drop ?";
        swapField.value = "Swap ?";
        interactField.value = "Interact ?";

        fields[0] = dropField;
        fields[1] = swapField;
        fields[2] = interactField;

        // Apply indicator of selected field
        TextField currField = fields[currIndex];
        currField.AddToClassList(selectedSlotUssName);
    }

    // methods to handle element selection
    // menu options should loop over
    public void changeSelectedField(int direction)
    {
        TextField currField = fields[currIndex];
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
        TextField newSelectedTextField = fields[currIndex];
        newSelectedTextField.AddToClassList(selectedSlotUssName);
    }
}
