using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;

// class that builds lureInventoryUI and holds logic for adding items to that lureUI
// we build 1 lure slot per siren
public class tabbedLureUIController : MonoBehaviour
{
    private VisualElement root;
    private VisualElement lureVisualElement;
    private List<LureInventorySlot> lureInventorySlots = new List<LureInventorySlot>();
    private static Dictionary<SirenTypes, LureInventorySlot> lureSlotsBySiren = new Dictionary<SirenTypes, LureInventorySlot>(); // keep track of which lure slot is assigned to which Siren

    // handles displaying selected slot
    private static int selectedSlotId = 0; // All slots are ordered together for selection (bait and fish) running 0-20
    private string selectedSlotUssName = "selectedSlotContainer";

    // handle manuevering through the inventory via arrow keys
    [SerializeField]
    GameObject gameManager;
    PlayerInput playerInput;
    InputAction inputAction;


    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        lureVisualElement = root.Query("LureScrollView"); // name of UI container for lure slots
        buildLureInventorySlots();
        selectLureSlot(selectedSlotId);

        // retrieve components needed for inventory navigation
        playerInput = gameManager.GetComponent<PlayerInput>();
        inputAction = playerInput.actions.FindAction("NavigateMenu");
    }

    private void buildLureInventorySlots()
    {
        foreach(SirenTypes sirenType in Enum.GetValues(typeof(SirenTypes))) // retrieve all enums in SirenType
        {
            LureInventorySlot lureSlot = new LureInventorySlot(sirenType);
            lureVisualElement.Add(lureSlot);
            // keep track of our LureInventorySlot objects
            lureSlotsBySiren.Add(sirenType, lureSlot);
            lureInventorySlots.Add(lureSlot);
        }
    }

    public void OnNagivateLureMenu() // scroll lures up and down
    {
        Vector2 xyValue = inputAction.ReadValue<Vector2>();
        int newSelectedSlotId = selectedSlotId;
        if(xyValue.y > 0)
        {
            newSelectedSlotId += 1;
        }
        else if(xyValue.y < 0)
        {
            newSelectedSlotId -= 1; 
        }
        selectLureSlot(newSelectedSlotId);
    }

    // HELPER METHODS
    private void selectLureSlot(int newSelectedSlotId) // disassociates last selected slot and increments to new selected slot by Id
    {
        lureInventorySlots[selectedSlotId].RemoveFromClassList(selectedSlotUssName);
        lureInventorySlots[newSelectedSlotId].AddToClassList(selectedSlotUssName);
        selectedSlotId = newSelectedSlotId;
    }
}
