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

    // list scripts which need player input
    controlBoat boatControls;
    tabbedInventoryUIController inventoryControls;

    private void Awake() // TODO : change this based on scene loaded
    {
        boatControls = boat.GetComponent<controlBoat>();
        inventoryControls = itemUI.GetComponent<tabbedInventoryUIController>();
    }

    // boat controls
    public void OnDropAnchor()
    {
        boatControls.OnDropAnchor();
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
        if(inventoryControls.enabled == true)
        {
            inventoryControls.OnNavigateMenu();
        }
    }
}
