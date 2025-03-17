using UnityEngine;
using UnityEngine.UIElements; 

public class WaresSlot : InventorySlot
{
    public Image soldItem; // this icon will overlay the existing item when it is sold
    private bool isSold = false; 
    private static string soldSignPath = "Sprites/BaitSprites/sold-item";
    public WaresSlot(int locId) : base(locId)
    { 
        icon = new Image();
        soldItem = new Image();
        Add(icon);
        Add(soldItem);
        this.AddToClassList("slotContainer"); // associates the uss style for "slotContainer" to our actual inventory slot
        this.locId = locId;
    }

    public void sellItem()
    {
        soldItem.sprite = Resources.Load<Sprite>(soldSignPath);
        isSold = true;
    }

    // GETTERS + SETTERS
    public bool isItemSold()
    {
        return isSold;
    }

}
