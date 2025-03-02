using UnityEngine;
using UnityEngine.UIElements;

// class responsible for each individual UI lure slot
public class LureInventorySlot : VisualElement
{
    public Image lureImage;
    public LureNote[] lureNotes; // list of notes which must be hit for this lure
    public bool isLureFound; // indicates whether this lure slot has lure information in it or not
    public SirenTypes lureFor; // indicates which Siren the lure will be used for

    public LureInventorySlot(SirenTypes sirenType)
    {
        lureFor = sirenType;
        lureImage = new Image();
        lureImage.sprite = Resources.Load<Sprite>("Sprites/LureSprites/unknown-lure");
        Add(lureImage);
        isLureFound = false;
        AddToClassList("lureSlotContainer");
        // set lure notes based on siren type
        // we have to manually build siren lures BEFORE this so we don't get any weird side effects from running like 4 Awake() at the same time
        lureNotes = SirenLureManager.getLureBySiren(sirenType);
    }

    // method that updates lure visual element to display lure music 
    public void findLure()
    {
        if(lureNotes != null)
        {
            Remove(lureImage);
            foreach (LureNote note in lureNotes)
            {
                Add(note);
            }
        }
        else
        {
            Debug.Log("Siren lure array not instantiated properly");
        }
    }

    // method that sets lure notes based on siren
    public void setLureNotes(LureNote[] lureNotes)
    {
        this.lureNotes = lureNotes;
    }
}
