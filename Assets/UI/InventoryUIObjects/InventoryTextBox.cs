using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

// script responsible for the UI text box which describes an item 
// there should only ever be 1 of these, just being moved to a new location
public class InventoryTextBox
{
    VisualElement visualElement; // we only have 1 text box, so we can just maintain a ref to that visual element
    Label itemName;
    Label itemDescription;

    public InventoryTextBox(VisualElement visualElement)
    {
        this.visualElement = visualElement;
        itemName = this.visualElement.Query<Label>(className : "itemName").First();
        itemDescription = this.visualElement.Query<Label>(className: "itemDescription").First();

        blankTextBox(); 
    }

    public void changeLabelName(string itemName)
    {
        this.itemName.text = itemName;
    }
    public void changeTextDescription(string itemDescription)
    {
        this.itemDescription.text = itemDescription; 
    }

    // HELPER METHODS
    public void blankTextBox()
    {
        itemName.text = "";
        itemDescription.text = "";
    }
}
