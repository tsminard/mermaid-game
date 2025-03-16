using UnityEngine;
using UnityEngine.UIElements; 

public class WaresSlot : InventorySlot
{
    public WaresSlot(int locId) : base(locId)
    { 
        icon = new Image();
        Add(icon);
        icon.AddToClassList("slotIcon"); // associate the uss style for "slotIcon" to our icon image
        this.AddToClassList("slotContainer"); // associates the uss style for "slotContainer" to our actual inventory slot
        this.locId = locId;
    }

}
