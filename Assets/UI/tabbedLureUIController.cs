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
    private ScrollView scrollView;
    private List<LureInventorySlot> lureInventorySlots = new List<LureInventorySlot>();
    private static Dictionary<SirenTypes, LureInventorySlot> lureSlotsBySiren = new Dictionary<SirenTypes, LureInventorySlot>(); // keep track of which lure slot is assigned to which Siren

    // handle scrolling
    private float currScrollLocation;
    private float lureSlotHeight = 0;

    // handles displaying selected slot
    private static int selectedSlotId = 0; // All slots are ordered together for selection (bait and fish) running 0-20
    private bool isSlotSelected = false;
    private string selectedSlotUssName = "selectedSlotContainer";

    // handle manuevering through the inventory via arrow keys
    [SerializeField]
    GameObject gameManager;
    PlayerInput playerInput;
    InputAction inputAction;


    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        scrollView = root.Query<ScrollView>();
        lureVisualElement = root.Query("LureScrollView"); // name of UI container for lure slots
        SirenLureManager.buildSirenLures(); // this has to be called BEFORE we create Lure Inventory Slots to prevent any race conditions
        buildLureInventorySlots();
        selectLureSlot(selectedSlotId);

        currScrollLocation = scrollView.verticalScroller.value;

        // retrieve components needed for inventory navigation
        playerInput = gameManager.GetComponent<PlayerInput>();
        inputAction = playerInput.actions.FindAction("NavigateMenu");
    }

    public void Update() // we need to run fixed update to check if someone is playing a lure
    {
        
    }

    private void buildLureInventorySlots()
    {
        Debug.Log("Building lure inventory slots");
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

        if (lureSlotHeight == 0)
        {
            //calculate slot height here to ensure visual elements are rendered before making this call
            lureSlotHeight = lureInventorySlots[0].resolvedStyle.height;
        }
        if (xyValue.y > 0)
        {
            if(selectedSlotId != 0)
            {
                newSelectedSlotId -= 1;
                currScrollLocation -= lureSlotHeight;
            }
        }
        else if(xyValue.y < 0)
        {
            if(selectedSlotId != lureInventorySlots.Count - 1)
            {
                newSelectedSlotId += 1;
                currScrollLocation += lureSlotHeight;
            }
        }
        // move our screen to the new selected slot
        scrollToLure(scrollView, currScrollLocation);
        selectLureSlot(newSelectedSlotId);
    }

    public void setSlotIsSelected(bool isSlotSelected)
    {
        Debug.Log("Slot " + selectedSlotId + " is selected");
        this.isSlotSelected = isSlotSelected;
    }

    // HELPER METHODS
    private void selectLureSlot(int newSelectedSlotId) // disassociates last selected slot and increments to new selected slot by Id
    {
        lureInventorySlots[selectedSlotId].RemoveFromClassList(selectedSlotUssName);
        lureInventorySlots[newSelectedSlotId].AddToClassList(selectedSlotUssName);
        selectedSlotId = newSelectedSlotId;
    }

    // method to scroll between points
    private void scrollToLure(ScrollView scroll, float endPosition)
    {
        float scrollTime = 0.5f;
        float currTime = 0f;
        float startPosition = scroll.verticalScroller.value; 

        while(currTime < scrollTime)
        {
            float t = currTime / scrollTime;
            float newPos = Mathf.SmoothStep(startPosition, endPosition, t); // interpolate between current and final position based on amount of time elapsed
            scroll.verticalScroller.value = newPos;
            currTime += Time.deltaTime; 
        }
        scroll.verticalScroller.value = endPosition; 
    }

    // GETTERS + SETTERS
    public static LureInventorySlot getInventorySlotBySiren(SirenTypes sirenType)
    {
        LureInventorySlot retrievedSlot;
        lureSlotsBySiren.TryGetValue(sirenType, out retrievedSlot);
        return retrievedSlot; 
    }
}
