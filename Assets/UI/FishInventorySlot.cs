using UnityEngine;
using UnityEngine.UIElements;

public class FishInventorySlot : InventorySlot
{
    public FishInventorySlot(int fishId) : base(fishId)
    {
        icon = new Image();
        Add(icon);
        icon.AddToClassList("slotIcon"); // associate the uss style for "slotIcon" to our icon image
        AddToClassList("fishSlotContainer"); // associates the uss style for "slotContainer" to our actual inventory slot
        Debug.Log("Associating fishSlotContainer class");
        this.locID = fishId;
    }
    // NOTE : i don't think update is called in VisualElements...
}
