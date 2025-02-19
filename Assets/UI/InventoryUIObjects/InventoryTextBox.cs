using UnityEngine;
using UnityEngine.UIElements;

// script responsible for the UI text box which describes an item 
// there should only ever be 1 of these, just being moved to a new location
public class InventoryTextBox : VisualElement
{
    Label itemLabel;
    TextField itemDescription; 

    public InventoryTextBox()
    {
        AddToClassList("slotTextContainer");
        style.visibility = Visibility.Hidden; // hidden by default
        itemLabel = new Label();
        itemLabel.text = "<UNKNOWN ITEM>";
        itemDescription = new TextField(); 
        itemDescription.value = "<NO DESCRIPTION>";
        Add(itemLabel);
        Add(itemDescription);
    }

    public void changeLabelName(string itemName)
    {
        Debug.Log("Changing label name to " + itemName);
        itemLabel.text = itemName; 
    }
    public void changeTextDescription(string itemDescription)
    {
        this.itemDescription.value = itemDescription; 
    }

    // HELPER METHODS
    public void toggleVisibility(bool isVisible)
    {
        if (isVisible) style.visibility = Visibility.Visible;
        else style.visibility = Visibility.Hidden; 
    }
}
