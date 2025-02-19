using UnityEngine.UIElements;

// script responsible for the individual UI inventory slots
// these DO NOT care about the actual item contained, only whether one is present or not
public class InventorySlot : VisualElement
{
    public Image icon;
    public int locID; // indicates which space on the screen this slot takes up
    private ItemDetails currentItem; // contains item information for display

    public InventorySlot(int locID)
    {
        icon = new Image();
        Add(icon);
        icon.AddToClassList("slotIcon"); // associate the uss style for "slotIcon" to our icon image
        AddToClassList("slotContainer"); // associates the uss style for "slotContainer" to our actual inventory slot

        this.locID = locID;
    }

    public void holdItem(ItemDetails itemDetails)
    {
        icon.sprite = itemDetails.itemData.icon;
        currentItem = itemDetails;
    }

    public void dropItem()
    {
        icon.image = null;
        currentItem = null; 
    }

    public void displayText(InventoryTextBox textBox)
    {
        string itemName = currentItem.uiName;
        string itemDescription = currentItem.itemData.description;
        textBox.changeLabelName(itemName);
        textBox.changeTextDescription(itemDescription);
        textBox.toggleVisibility(true);
    }

    // HELPER METHODS

    public bool isEmpty()
    {
        if (currentItem == null) return true;
        else return false; 
    }
}
