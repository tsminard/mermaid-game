using System.Collections.Generic;
using UnityEngine;
using System;

// class that holds templates for ALL ITEMS. there should only ever be one of these !!
// contains instance of ItemData AND ItemDetail ( specifically for inventory usage )
public class ItemManager : MonoBehaviour 
{
    private static Dictionary<string, ItemDetails> allItems = new Dictionary<string, ItemDetails>();
    private static List<SirenTypes> availableLures = new List<SirenTypes>();

    private void Awake()
    {
        // populate our sirentypes with all possible siren types
        foreach(SirenTypes sirenType in Enum.GetValues(typeof(SirenTypes)))
        {
            availableLures.Add(sirenType);
        }
        // build all our possible objects to be referenced later
        // NOTE : each of these corresponds to a SPRITE REPRESENTATION. if we want multiple sprites, we need to either change the ItemDetails.ItemData.Sprite field or make a new object
        Debug.Log("Building all fish");
        buildAllFish();
        buildAllTrash(); 
    }

    private void buildAllFish() // i think this all has to be hard-coded :')
    {
        // NOTE : the first-passed string must correspond EXACTLY to the name of the sprite in the Resources folder
        ItemData arowana = buildItemData("arowana", 25f, "What the heck is this ? At least it's...big ??");
        ItemData koi = buildItemData("koi", 15f,  "I know this one from anime!");
        ItemData tilapia = buildItemData("tilapia", 100f, "I think this one is..edible...");

        // add all fish to itemData list so we can iterate through it and build every ItemDetail
        List<(string uiName, ItemData itemData)> allItemDatas = new List<(string, ItemData)>()
        {
            ("Big fish thing", arowana),
            ("Weeb fish ?", koi),
            ("...Generic fish", tilapia)
        };

        foreach(var itemTuple in allItemDatas)
        {
            ItemDetails itemDetails = buildItemDetails(itemTuple.uiName, itemTuple.itemData, true); // for now, all objects are droppable
            allItems.Add(itemTuple.itemData.itemName, itemDetails); 
        }

    }

    private void buildAllTrash()
    {
        ItemData boot = buildItemData("boot", .99f, "A boot...perfect for my third foot!");
        ItemData can = buildItemData("can", 1.50f, "Hey, I recognise this brand ! They're a midrange option for most single-person bomb shelters");
        ItemData lobsterTrap = buildItemData("lobster-trap", 12.99f, "Thank goodness there isn't a lobster inside - what do you even do with those ???");
        ItemData bottleMessage = buildItemData("message-in-bottle", 21, "Is there something inside this ?");

        // add all trash to itemData list so we can iterate through it and build every ItemDetail
        List<(string uiName, ItemData itemData)> allItemDatas = new List<(string, ItemData)>()
        {
            ("Dirty wet boot", boot), 
            ("Recyclable can", can),
            ("Lobster trap", lobsterTrap), 
            ("A bottle... with something inside it ? ", bottleMessage)
        };

        foreach (var itemTuple in allItemDatas)
        {
            ItemDetails itemDetails = buildItemDetails(itemTuple.uiName, itemTuple.itemData, true); // for now, all objects are droppable
            allItems.Add(itemTuple.itemData.itemName, itemDetails);
        }

        // handle exceptions, like droppable, interact, etc. 
        ItemDetails messageInBottle; 
        allItems.TryGetValue(bottleMessage.itemName, out messageInBottle);
        messageInBottle.setCanDrop(false);
        messageInBottle.setInteractionFunction(messageInBottleInteraction);
    }

    // METHODS FOR INDIVIDUAL ITEMS
    public string messageInBottleInteraction() // TODO : add message in bottle info to UI
    {
        // need to change LURE object which means retrieving LureInventorySlot element
        SirenTypes? lureType = generateRandomSirenType(); 
        if(lureType == null)
        {
            // TODO : handle case when bottle is caught when all lures are found
            return "\"We've been trying to reach you about your car's extended warranty...\"";
        }
        else // set the corresponding Lure to an image
        {
            LureInventorySlot lureSlot = tabbedLureUIController.getInventorySlotBySiren((SirenTypes)lureType);
            lureSlot.findLure(); 
        }

        return "There's some kind of message in here !";
    }

    // HELPER METHODS
    private ItemData buildItemData(string itemName, float value, string itemDesc) // itemData is specifically for throwing into 
    {
        ItemData item = (ItemData)ScriptableObject.CreateInstance("ItemData");
        if(itemDesc != null)
        {
            item.init(itemName, Resources.Load<Sprite>("Sprites/" + itemName), value, itemDesc);
        }
        else
        {
            item.init(itemName, Resources.Load<Sprite>("Sprites/" + itemName));
        }
        return item;
    }

    private ItemDetails buildItemDetails(string uiName, ItemData itemData, bool canDrop)
    {
        return new ItemDetails(uiName, itemData, canDrop);
    }

    public static ItemDetails getItemByName(string name)
    {
        if (allItems.ContainsKey(name))
        {
            return allItems[name]; 
        }
        else
        {
            Debug.Log("No record of item " + name);
            return null; 
        }
    }

    // this should only be called from within ItemManager because it depends entirely on an internal list of used sirenTypes
    private static SirenTypes? generateRandomSirenType()
    {
        if(availableLures.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, availableLures.Count - 1);
            SirenTypes selectedSirenType = availableLures[index];
            availableLures.RemoveAt(index);
            //return selectedSirenType; 
            return SirenTypes.Moray; // TODO : This is just for testing since I don't have all the sprites yet !
        }
        else
        {
            return null; 
        }
    }
}
