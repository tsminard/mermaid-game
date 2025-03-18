using UnityEngine;

// class to send messages from player input to BAIT SHOP gameobjects
public class SendBaitShopInput : MonoBehaviour
{
    // references to gameobjects which need player input information 
    [SerializeField]
    GameObject baitShopUI;

    baitShopInventoryController baitShopUIController;
    fishShopInventoryController fishShopUIController;
    TabbedUIController tabController;

    private int activeTab = 0; // indicate which inventory tab is active

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baitShopUIController = baitShopUI.GetComponent<baitShopInventoryController>();
        tabController = baitShopUI.GetComponent<TabbedUIController>();
        fishShopUIController = baitShopUI.GetComponent<fishShopInventoryController>();
        // make UI visible again - tabController automatically sets it to false
        tabController.toggleDisplay();
    }

    public void OnNavigateMenu()
    {
        if(activeTab == 0) // bait shop UI is currently active
        {
            if (!baitShopUIController.isBaitShopSlotSelected()) // if a slot is not selected, navigate the items listed
            {
                baitShopUIController.OnNavigateMenu();
            }
            else // otherwise, navigate the submenu 
            {
                baitShopUIController.OnNavigateSubMenu();
            }
        }
        else if (activeTab == 1) // fish monger shop is active
        {
            if (!fishShopUIController.isBaitShopSlotSelected()) // fish monger shop is a child of bait shop
            {
                fishShopUIController.OnNavigateMenu();
            }
            else
            {
                fishShopUIController.OnNavigateSubMenu();
            }
        }
        else if (activeTab == 2) // boat repair screen
        {

        }
    }

    public void OnSubmit()
    {
        if(activeTab == 0)
        {
            baitShopUIController.OnSubmit();
        }
        else if (activeTab == 1)
        {
            fishShopUIController.OnSubmit();
        }
    }

    public void OnCancel()
    {
        baitShopUIController.OnCancel();
    }

    // methods to change shop tabs
    public void OnSelectTabOne() // inventory tab
    {
        tabController.setActiveTab(0);
        baitShopUIController.onSelection();// runs start-up stuff for this UI which differentiates it from other UIs
        activeTab = 0;
    }

    public void OnSelectTabTwo() // notebook tab
    {
        tabController.setActiveTab(1);
        fishShopUIController.onSelection(); // runs start-up stuff for this UI which differentiates it from other UIs
        activeTab = 1;
    }

    public void OnSelectTabThree() // lure tab
    {
        tabController.setActiveTab(2);
        activeTab = 2;
    }
}
