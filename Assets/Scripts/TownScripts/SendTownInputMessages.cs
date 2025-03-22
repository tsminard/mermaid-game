using UnityEngine;
using UnityEngine.InputSystem;

// script to trigger appropriate controls in town scene
public class SendTownInput : MonoBehaviour
{
    [SerializeField]
    GameObject baitShop; // gameobject that represents the door
    [SerializeField]
    GameObject boatShop; // gameobject that represents the door

    handleShopEntrance baitShopEntrance;
    handleShopEntrance boatShopEntrance;

    void Start()
    {
        baitShopEntrance = baitShop.GetComponent<handleShopEntrance>();
        boatShopEntrance = baitShop.GetComponent<handleShopEntrance>();
    }

    public void OnInteract()
    {
        // just running both of them because they independently keep track of player's location
        baitShopEntrance.tryEnterShop(ShopTypes.Bait_Shop);
        boatShopEntrance.tryEnterShop(ShopTypes.Boat_Shop);
    }

}
