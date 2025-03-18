using UnityEngine;
using System.Collections.Generic;
using System;

// SINGLETON to handle persisting data between scenes
public class PersistData : MonoBehaviour
{
    // Siren interaction variables with defaults
    Dictionary<SirenTypes, int> sirenFamiliarity = new Dictionary<SirenTypes, int>(); 
    SirenTypes siren = SirenTypes.Sea_Angel; // indicates which siren we are currently talking to 

    // inventory variables
    Dictionary<int, ItemDetails> currentInventory = new Dictionary<int, ItemDetails>();
    List<SirenTypes> discoveredLures = new List<SirenTypes>(); // storing by siren type so the lure menu on reload can search up its child visualelements by siren type instead of persisting object through scenes 
    public int numInventorySlots = 20; // this is the TOTAL NUMBER OF SLOTS, fish and bait
    public int numBaitSlots = 5; // these two should add up to our total number of inventory slots
    public int numFishSlots = 15;

    private float currMoney = 20.05f; 

    // singleton variables
    private static PersistData _Instance;
    public static PersistData Instance
    {
        get
        {
            if (!_Instance)
            {
                _Instance = new GameObject().AddComponent<PersistData>();
                _Instance.name = _Instance.GetType().ToString();
                DontDestroyOnLoad(_Instance.gameObject);
            }
            return _Instance;
        }
    }

    private void Awake()
    {
        foreach(SirenTypes sirenType in Enum.GetValues(typeof(SirenTypes))){
            sirenFamiliarity.Add(sirenType, 1); // set our immediate familiarity with each siren to 1
        }
    }

    // GETTERS + SETTERS
    // this should probably be the majority of this class
    // siren methods
    public void setSirenInteractionNumber(SirenTypes sirenType, int sirenInteractionNumber) {
        sirenFamiliarity[sirenType] = sirenInteractionNumber;
    }

    public int getSirenInteractionNumber() {
        int interactionNumber;
        sirenFamiliarity.TryGetValue(siren, out interactionNumber);
        return interactionNumber; 
    }

    public void setSiren(SirenTypes siren) { this.siren = siren;  }

    public SirenTypes getSiren() { return siren;  }

    // inventory methods
    public void saveInventoryContents(Dictionary<int, ItemDetails> currentInventory)
    {
        this.currentInventory = currentInventory; 
    }

    public Dictionary<int, ItemDetails> retrieveInventoryContents()
    {
        return currentInventory; 
    }

    public float getCurrentMoney() { return currMoney;  }

    public void addMoney(float money) { currMoney += money;  }
    public void removeMoney(float money) { currMoney -= money; }

    // call generateNewInventoryIndex first to ensure there is space before we actually add anything to our inventory
    public void addItemToInventory(int itemIndex, ItemDetails item)
    {
        currentInventory.Add(itemIndex, item);
    }

    public void removeItemFromInventory(ItemDetails item)
    {
        for(int i = 0; i < numInventorySlots; i++) // have to iterate over every slot because items might not be arranged consecutively
        {
            ItemDetails checkItem;
            currentInventory.TryGetValue(i, out checkItem);
            if(checkItem != null)
            {
                // compare items to see if this is the index we want
                if (item.Equals(checkItem))
                {
                    currentInventory.Remove(i);
                    return;
                }
            }
        }
        Debug.Log("Could not find this item in inventory");
    }

    public void discoverLure(SirenTypes sirenLure)
    {
        discoveredLures.Add(sirenLure);
    }

    public List<SirenTypes> getDiscoveredLures()
    {
        return discoveredLures;
    }

    // helper methods

    // method that checks our stored inventory for space available
    // if there is NO SPACE AVAILABLE we return -1
    public int generateNewInventoryIndex(ItemInventoryType inventoryType)
    {
        List<int> inventorySlotsOccupied = new List<int>(currentInventory.Keys);
        int numSlotsTotal = (inventoryType == ItemInventoryType.Bait) ? numBaitSlots : numBaitSlots + numFishSlots;
        int i = (inventoryType == ItemInventoryType.Bait) ? 0 : 5; 
        while (i < numSlotsTotal)
        {
            if (!inventorySlotsOccupied.Contains(i))
            {
                return i; 
            }
            i++;
        }
        return -1; 
    }
}
