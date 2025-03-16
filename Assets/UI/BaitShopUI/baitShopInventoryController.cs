using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

// script to generate shop UI
public class baitShopInventoryController : MonoBehaviour
{
    private VisualElement root;
    private VisualElement waresRoot;
    private VisualElement itemDescription;

    // values for display
    private static int numWareSlots = 15;

    // values for keeping track of items displayed
    Dictionary<int, WaresSlot> waresSlotsById = new Dictionary<int, WaresSlot>();

    // names of slots that we will be filling
    string waresRootName = "Wares"; // container for all ware items


    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        waresRoot = root.Query(waresRootName);
        Debug.Log("Creating Ware Slots");
        buildWareSlots();
    }

    private void buildWareSlots()
    {
        for(int i = 0; i < numWareSlots; i++)
        {
            WaresSlot wareSlot = new WaresSlot(i);
            waresSlotsById.Add(i, wareSlot);
            waresRoot.Add(wareSlot);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
