using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections; // need this for coroutines
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
    private string selectedLureUssName = "correctLureNote";

    // handles playing the lure
    private LureNote[] currLure;
    private int currLureIndex; 
    private LureNote currLureNote;

    // handles successful lure
    [SerializeField]
    public GameObject sirenGame;

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

    public void FixedUpdate() // we need to run fixed update to check if someone is playing a lure
    {
        if (isSlotSelected)
        {
            KeyCode listenFor = currLureNote.inputKey;
            if (Input.anyKeyDown)
            {
                Debug.Log("Listening for " + listenFor.ToString());
                if (Input.GetKeyDown(listenFor)) // the correct button is pressed
                {
                    Debug.Log("Correct note played");
                    currLureNote.toggleCorrectNote(true);
                    // increment currNote forwards
                    currLureIndex += 1;
                    if (currLureIndex == currLure.Length) // we are out of lure notes
                    {
                        currLureIndex = 0;
                        // TODO : put successful lure response here !
                        Debug.Log("Lure played succesfully!");
                        deactivateCorrectLure(); // remove highlighting
                        isSlotSelected = false; // deactivate our listening loop
                        sirenGame.SetActive(true);
                    }
                    else
                    {
                        currLureNote = currLure[currLureIndex];
                    }
                }
                else
                {
                    Debug.Log("Incorrect note played : " + Input.anyKey.ToString());
                    // remove all correct lures
                    for (int i = currLureIndex; i >= 0; i--)
                    {
                        currLure[i].toggleCorrectNote(false);
                        currLure[i].toggleIncorrectNote(true);
                        IEnumerator waitForLure = pauseForLureFeedback(0.25f); // scale our wait time to the number of notes 
                        StartCoroutine(waitForLure);
                        currLure[i].toggleIncorrectNote(false);
                    }
                    currLureIndex = 0;
                    currLureNote = currLure[currLureIndex]; // reset our current lure note to the first one
                }
            }
        }
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

    // method for input manager to toggle whether slots are selected or not
    public void setSlotIsSelected(bool isSlotSelected)
    {
        this.isSlotSelected = isSlotSelected;
        LureInventorySlot currLureSlot = lureInventorySlots[selectedSlotId];
        if (isSlotSelected)
        {
            currLureSlot.AddToClassList(selectedLureUssName); // indicate that we are "locked in" to this lure now
            currLure = currLureSlot.lureNotes;
            currLureNote = currLure[0];
            currLureIndex = 0;
        }
        else
        {
            currLureSlot.RemoveFromClassList(selectedLureUssName);
            currLureIndex = 0; 
        }
       
    }


    // HELPER METHODS
    private void deactivateCorrectLure()
    {
        Debug.Log("Deactivating lure");
        for(int i = currLure.Length - 1; i >= 0; i--)
        {
            currLure[i].toggleCorrectNote(false);
            StartCoroutine(pauseForLureFeedback(0.5f));
        }
    }
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

    // little coroutine to pause so player knows a mistake has occured in the lure
    private IEnumerator pauseForLureFeedback(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }

    // GETTERS + SETTERS
    public static LureInventorySlot getInventorySlotBySiren(SirenTypes sirenType)
    {
        LureInventorySlot retrievedSlot;
        lureSlotsBySiren.TryGetValue(sirenType, out retrievedSlot);
        return retrievedSlot; 
    }
}
