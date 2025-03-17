using UnityEngine;

// class to send messages from player input to BAIT SHOP gameobjects
public class SendBaitShopInput : MonoBehaviour
{
    // references to gameobjects which need player input information 
    [SerializeField]
    GameObject baitShopUI;

    baitShopInventoryController baitShopUIController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baitShopUIController = baitShopUI.GetComponent<baitShopInventoryController>();
    }

    public void OnNavigateMenu()
    {
        if (!baitShopUIController.isSlotSelected) // if a slot is not selected, navigate the items listed
        {
            baitShopUIController.OnNavigateMenu();
        }
        else // otherwise, navigate the submenu 
        {
            baitShopUIController.OnNavigateSubMenu();
        }  
    }

    public void OnSubmit()
    {
        baitShopUIController.OnSubmit();
    }

    public void OnCancel()
    {
        baitShopUIController.OnCancel();
    }
}
