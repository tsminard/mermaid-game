using System.Collections.Generic;
using UnityEngine;
using System;

// Singleton that holds templates for ALL ITEMS. there should only ever be one of these !!
// contains instance of ItemData AND ItemDetail ( specifically for inventory usage )
public class ItemManager : MonoBehaviour 
{
    private Dictionary<string, ItemDetails> allItems = new Dictionary<string, ItemDetails>();
    private Dictionary<string, ItemDetails> allBaits = new Dictionary<string, ItemDetails>(); // separating Bait from Items because they are used differently, even though they use the same base classes
    private  List<SirenTypes> availableLures = new List<SirenTypes>();

    // strings relating to filepaths
    private static string itemPath = "ItemSprites/";
    private static string baitPath = "BaitSprites/";

    private static ItemManager _Instance;
    public static ItemManager Instance
    {
        get
        {
            if (!_Instance)
            {
                _Instance = new GameObject().AddComponent<ItemManager>();
                _Instance.name = _Instance.GetType().ToString();
                DontDestroyOnLoad(_Instance.gameObject);
            }
            return _Instance;
        }
    }
    
    private void Awake()
    {
        // populate our sirentypes with all possible siren types
        foreach(SirenTypes sirenType in Enum.GetValues(typeof(SirenTypes)))
        {
            availableLures.Add(sirenType);
        }
        // build all our possible objects to be referenced later
        // NOTE : each of these corresponds to a SPRITE REPRESENTATION. if we want multiple sprites, we need to either change the ItemDetails.ItemData.Sprite field or make a new object
        buildAllFish();
        buildAllTrash();
        buildAllBait();
    }

    private void buildAllFish() // i think this all has to be hard-coded :')
    {
        // NOTE : the first-passed string must correspond EXACTLY to the name of the sprite in the Resources folder
        ItemData arowana = buildItemData("arowana", itemPath, 25f, "What the heck is this ? At least it's...big ??");
        ItemData koi = buildItemData("koi", itemPath, 15f,  "I know this one from anime!");
        ItemData tilapia = buildItemData("tilapia", itemPath, 100f, "I think this one is..edible...");

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
        ItemData boot = buildItemData("boot", itemPath, .99f, "A boot...perfect for my third foot!");
        ItemData can = buildItemData("can", itemPath, 1.50f, "Hey, I recognise this brand ! They're a midrange option for most single-person bomb shelters");
        ItemData lobsterTrap = buildItemData("lobster-trap", itemPath, 12.99f, "Thank goodness there isn't a lobster inside - what do you even do with those ???");
        ItemData bottleMessage = buildItemData("message-in-bottle", itemPath, 21, "Is there something inside this ?");
        ItemData emptyBottle = buildItemData("empty-bottle", itemPath, 1.99f, "An empty bottle that seems somehow forlorn");

        // add all trash to itemData list so we can iterate through it and build every ItemDetail
        List<(string uiName, ItemData itemData)> allItemDatas = new List<(string, ItemData)>()
        {
            ("Dirty wet boot", boot), 
            ("Recyclable can", can),
            ("Lobster trap", lobsterTrap), 
            ("A bottle... with something inside it ? ", bottleMessage),
            ("A bottle without anything inside it...", emptyBottle)
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

    private void buildAllBait()
    {
        ItemData chumBucket = buildItemData("chum-bucket", baitPath, 20.99f, "A bucket of chopped-up fish sure to attract some bigger catches, or your money back! (\"Bigger catches\" may include boots, garbage floats, and carcasses)");
        ItemData hotdog = buildItemData("hotdog", baitPath, 3.99f, "I know it looks weird, but I swear some fish LOVE these things!");

        List<(string uiName, ItemData itemData)> allBaitDetails = new List<(string, ItemData)>()
        {
            ("Chum Bucket", chumBucket), 
            ("CostCo hotdog", hotdog)
        };

        foreach (var itemTuple in allBaitDetails)
        {
            ItemDetails itemDetails = buildItemDetails(itemTuple.uiName, itemTuple.itemData, true); // for now, all objects are droppable
            allBaits.Add(itemTuple.itemData.itemName, itemDetails);
        }
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
            PersistData.Instance.discoverLure(lureSlot.lureFor);// persist that we have discovered this lure
            lureSlot.findLure();
            // replace the bottle in our inventory with an empty bottle
            int currSlotIndex = tabbedInventoryUIController.returnCurrentSelectedSlot();
            // remove the message in a bottle from our inventory
            inventoryController.dropItemFromInventory(currSlotIndex);
            // add the empty bottle to our inventory
            ItemDetails emptyBottle;
            allItems.TryGetValue("empty-bottle", out emptyBottle);
            if (emptyBottle != null)
            {
                int normalizedIndex = inventoryController.normalizeInventorySlot(currSlotIndex);
                inventoryController.addItemToInventory(normalizedIndex,emptyBottle, ItemInventoryType.Fish);
            }
            else
            {
                Debug.Log("Error retrieving empty bottle item - check ItemManager");
            }
        }

        return "There's some kind of message in here !";
    }

    // HELPER METHODS
    private ItemData buildItemData(string itemName, string spriteFolder, float value, string itemDesc) // itemData is specifically for throwing into ItemDetails
    {
        ItemData item = (ItemData)ScriptableObject.CreateInstance("ItemData");
        if(itemDesc != null)
        {
            item.init(itemName, Resources.Load<Sprite>("Sprites/" + spriteFolder + itemName), value, itemDesc);
        }
        else
        {
            item.init(itemName, Resources.Load<Sprite>("Sprites/" + spriteFolder + itemName));
        }
        return item;
    }

    private ItemDetails buildItemDetails(string uiName, ItemData itemData, bool canDrop)
    {
        return new ItemDetails(uiName, itemData, canDrop);
    }

    public ItemDetails getItemByName(string name)
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

    public ItemDetails getBaitByName(string name)
    {
        if (allBaits.ContainsKey(name))
        {
            return allBaits[name];
        }
        else
        {
            Debug.Log("No record of bait " + name);
            return null;
        }
    }

    // this should only be called from within ItemManager because it depends entirely on an internal list of used sirenTypes
    private SirenTypes? generateRandomSirenType()
    {
        if(availableLures.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, availableLures.Count - 1);
            SirenTypes selectedSirenType = availableLures[index];
            availableLures.RemoveAt(index);
            return selectedSirenType; 
            //return SirenTypes.Moray; // TODO : This is just for testing since I don't have all the sprites yet !
        }
        else
        {
            return null; 
        }
    }
}
