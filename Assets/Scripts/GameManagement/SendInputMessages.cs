using UnityEngine;

// class to trigger appropriate controls across project 
// this is necessary because PlayerInput is attached to a perpetuated gameobject instead of individual scene-dependent ones
// Messages are only broadcast to the gameObject that PlayerInput is attached to, so i have to hook it all up manually
public class SendInputMessages : MonoBehaviour
{
    // list gameObjects which need player input
    [SerializeField]
    GameObject boat;
    [SerializeField]
    GameObject itemUI;
    [SerializeField]
    GameObject fishingMinigame;

    // list scripts which need player input
    controlBoat boatControls;
    TabbedUIController tabController;
    tabbedInventoryUIController inventoryControls;

    // list internal variables
    bool isUsingSubMenu = false; 

    private void Awake() // TODO : change this based on scene loaded
    {
        boatControls = boat.GetComponent<controlBoat>();
        tabController = itemUI.GetComponent<TabbedUIController>(); 
        inventoryControls = itemUI.GetComponent<tabbedInventoryUIController>();
    }

    // boat controls
    public void OnDropAnchor()
    {
        if (!tabController.isVisible)
        {
            boatControls.OnDropAnchor();
        }
    }

    public void OnOpenInventory()
    {
        boatControls.OnOpenInventory();
    }

    public void OnFish()
    {
        boatControls.OnFish();
    }

    // inventory controls
    public void OnNavigateMenu()
    {
        if(tabController.isVisible && !fishingMinigame.activeSelf) // the fishing minigame controls should take precendence over the inventory controls
        {
            if (!isUsingSubMenu)
            {
                inventoryControls.OnNavigateMenu();
            }
            else
            {
                inventoryControls.OnNavigateSubMenu(); // we enter into subgenre of control navigation using same arrow controls
            }
        }
    }

    public void OnSubmit()
    {
        if (tabController.isVisible && !fishingMinigame.activeSelf) // the fishing minigame controls should take precendence over the inventory controls
        {
            if (!isUsingSubMenu)
            {
                if (!tabbedInventoryUIController.isCurrentSelectedSlotEmpty())
                {
                    isUsingSubMenu = true; // we don't want to handle submenu logic for empty slots
                }
            }
            else
            {
                inventoryControls.OnSelectSubMenu(); 
                isUsingSubMenu = false;
            }
        }  
    }

    public void OnCancel()// this so far only applies to exiting out of submenus without performing an action
    {
        tabbedInventoryUIController.OnCeaseNavigateSubMenu();
        isUsingSubMenu = false;
    }
}
