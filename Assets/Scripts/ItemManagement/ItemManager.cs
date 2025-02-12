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
        Debug.Log("Building all fish");
        buildAllFish(); 
    }

    private void buildAllFish() // i think this all has to be hard-coded :')
    {
        ItemData arowana = buildItemData("arowana", 25f, "What the heck is this ? At least it's...big ??");
        ItemData koi = buildItemData("koi", 15f,  "I know this one from anime!");
        ItemData tilapia = buildItemData("???", 100f, "I think this one is..edible...");

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
        // TODO : sort out that both FishSpeciesInfo and ItemDetails have VALUE 
    }

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

    // HELPER METHODS
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
