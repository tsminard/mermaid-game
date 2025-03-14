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

    public void discoverLure(SirenTypes sirenLure)
    {
        discoveredLures.Add(sirenLure);
    }

    public List<SirenTypes> getDiscoveredLures()
    {
        return discoveredLures;
    }
}
