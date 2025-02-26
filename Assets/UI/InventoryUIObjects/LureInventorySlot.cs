using UnityEngine;
using UnityEngine.UIElements;

// class responsible for each individual UI lure slot
public class LureInventorySlot : VisualElement
{
    public Image lureImage; 
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
    }

    public void findLure()
    {
        Debug.Log("Sprite path : Sprites/LureSprites/ " + lureFor);
        lureImage.sprite = Resources.Load<Sprite>("Sprites/LureSprites/" + lureFor);
    }
}
