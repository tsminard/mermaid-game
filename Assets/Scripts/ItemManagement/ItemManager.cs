using System.Collections.Generic;
using UnityEngine;

// class that holds templates for ALL ITEMS. there should only ever be one of these !!
// contains instance of ItemData AND ItemDetail ( specifically for inventory usage )
public class ItemManager : MonoBehaviour 
{
    private static Dictionary<string, ItemDetails> allItems = new Dictionary<string, ItemDetails>();

    private void Awake()
    {
        // build all our possible objects to be referenced later
        // NOTE : each of these corresponds to a SPRITE REPRESENTATION. if we want multiple sprites, we need to either change the ItemDetails.ItemData.Sprite field or make a new object
        Debug.Log("Building all fish");
        buildAllFish();
        buildAllTrash(); 
    }

    private void buildAllFish() // i think this all has to be hard-coded :')
    {
        ItemData arowana = buildItemData("arowana", 25f, "What the heck is this ? At least it's...big ??");
        ItemData koi = buildItemData("koi", 15f,  "I know this one from anime!");
        ItemData tilapia = buildItemData("tilapia", 100f, "I think this one is..edible...");

        // add all fish to itemData list so we can iterate through it and build every ItemDetail
        List<ItemData> allItemDatas = new List<ItemData>()
        {
            arowana,
            koi,
            tilapia
        };

        foreach(ItemData itemData in allItemDatas)
        {
            ItemDetails itemDetails = buildItemDetails(itemData, true); // for now, all objects are droppable
            allItems.Add(itemData.itemName, itemDetails); 
        }
    }

    private void buildAllTrash()
    {
        ItemData boot = buildItemData("boot", .99f, "A boot...perfect for my third foot!");
        ItemData can = buildItemData("can", 1.50f, "Hey, I recognise this brand ! They're a midrange option for most single-person bomb shelters");
        ItemData lobsterTrap = buildItemData("lobster-trap", 12.99f, "Thank goodness there isn't a lobster inside - what do you even do with those ???");
        ItemData bottleMessage = buildItemData("message-in-bottle", 21, "Is there something inside this ?");

        // add all trash to itemData list so we can iterate through it and build every ItemDetail
        List<ItemData> allItemDatas = new List<ItemData>()
        {
            boot, 
            can,
            lobsterTrap, 
            bottleMessage
        };

        foreach (ItemData itemData in allItemDatas)
        {
            ItemDetails itemDetails = buildItemDetails(itemData, true); // for now, all objects are droppable
            allItems.Add(itemData.itemName, itemDetails);
        }

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

    private ItemDetails buildItemDetails(ItemData itemData, bool canDrop)
    {
        return new ItemDetails(itemData, canDrop);
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
}
