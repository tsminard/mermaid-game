using UnityEngine;

// script to trigger appropriate controls in town scene
public class SendTownInput : MonoBehaviour
{
    [SerializeField]
    GameObject baitShop; // gameobject that represents the door
    [SerializeField]
    GameObject boatShop; // gameobject that represents the door
    [SerializeField]
    GameObject exitObject;

    handleShopEntrance baitShopEntrance;
    handleShopEntrance boatShopEntrance;
    exitTown exitTownControls;

    void Start()
    {
        baitShopEntrance = baitShop.GetComponent<handleShopEntrance>();
        boatShopEntrance = baitShop.GetComponent<handleShopEntrance>();
        exitTownControls = exitObject.GetComponent<exitTown>();
    }

    public void OnInteract()
    {
        // just running both of them because they independently keep track of player's location
        baitShopEntrance.tryEnterShop(ShopTypes.Bait_Shop);
        boatShopEntrance.tryEnterShop(ShopTypes.Boat_Shop);
    }

    public void OnEnterLocation()
    {
        if (exitTownControls.getIsMessageDisplayed()) // letting this script check this in case we need to re-use this keymapping in the same scene
        {
            exitTownControls.closeTownScene();
        }
    }
}
