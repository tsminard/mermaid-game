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
    tabbedLureUIController lureInventoryControls;

    // list internal variables
    // inventory UIS
    bool isUsingSubMenu = false;
    int activeTab = 0; // indicate which inventory tab is active

    private void Awake() // TODO : change this based on scene loaded
    {
        boatControls = boat.GetComponent<controlBoat>();
        tabController = itemUI.GetComponent<TabbedUIController>(); 
        inventoryControls = itemUI.GetComponent<tabbedInventoryUIController>();
        lureInventoryControls = itemUI.GetComponent<tabbedLureUIController>();
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
            if(activeTab == 0) // logic for inventory tab
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
            else if(activeTab == 1)
            {
                // TODO : FILL IN NOTEBOOK LOGIC
            }
            else if (activeTab == 2) // logic for lure tab
            {
                lureInventoryControls.OnNagivateLureMenu(); 
            }
        }
    }

    public void OnSubmit()
    {
        if (tabController.isVisible && !fishingMinigame.activeSelf) // the fishing minigame controls should take precendence over the inventory controls
        {
            if(activeTab == 0) // logic for inventory tab
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
    }

    public void OnCancel()// this so far only applies to exiting out of submenus without performing an action
    {
        if(activeTab == 0)
        {
            tabbedInventoryUIController.OnCeaseNavigateSubMenu();
            isUsingSubMenu = false;
        }
    }

    // methods to change inventory tabs
    public void OnSelectInventoryTab()
    {
        tabController.setActiveTab(0);
        activeTab = 0;
    }

    public void OnSelectNotebookTab()
    {
        tabController.setActiveTab(1);
        activeTab = 1;
    }

    public void OnSelectLureTab()
    {
        tabController.setActiveTab(2);
        activeTab = 2;
    }
}
