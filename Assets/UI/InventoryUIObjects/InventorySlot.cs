using UnityEngine.UIElements;

// class responsible for each individual UI inventory slots
// these DO NOT care about the actual item contained, only whether one is present or not
public class InventorySlot : VisualElement
{
    public Image icon;
    public int locId; // indicates which space on the screen this slot takes up
    private ItemDetails currentItem; // contains item information for display
    private string containerClassName = "slotContainer"; // naming generically so this class can be extended
    private string iconClassName = "slotIcon";

    public InventorySlot(int locId)
    {
        icon = new Image();
        Add(icon);
        icon.AddToClassList(iconClassName); // associate the uss style for "slotIcon" to our icon image
        AddToClassList(containerClassName); // associates the uss style for "slotContainer" to our actual inventory slot
        this.locId = locId;
    }

    public void holdItem(ItemDetails itemDetails)
    {
        icon.sprite = itemDetails.itemData.icon;
        currentItem = itemDetails;
    }

    public void dropItem()
    {
        icon.sprite = null;
        currentItem = null;
    }

    public void displayText(InventoryTextBox textBox)
    {
        string itemName = currentItem.uiName;
        string itemDescription = currentItem.itemData.description;
        textBox.changeLabelName(itemName);
        textBox.changeTextDescription(itemDescription);
    }

    public string interactWithItem()
    {
        return currentItem.interactWith(); 
    }

    // HELPER METHODS
    public bool isEmpty()
    {
        if (currentItem == null) return true;
        else return false; 
    }

    public bool isInteractable()
    {
        return currentItem.getCanInteract();
    }

    public bool isDroppable()
    {
        return currentItem.isDroppable();
    }

    // GETTERS + SETTERS
    public ItemDetails getSlotItem()
    {
        return currentItem;
    }
}
