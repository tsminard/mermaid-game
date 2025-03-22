using UnityEngine;
using UnityEngine.SceneManagement; 

// reusable script to check if player is interacting with a shop door
public enum ShopTypes
{
    Bait_Shop, 
    Boat_Shop
}

public class handleShopEntrance : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    float playerWidth;

    // handle UI popups if player walks in front of door
    [SerializeField]
    GameObject shopMessage;
    SpriteRenderer shopMessageRenderer;
    Bounds spriteBoundaries; 
    bool isInFrontOfDoor = false;

    // handle loading shop scenes
    int baitShopSceneIndex = 4;
    int boatShopSceneIndex = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shopMessageRenderer = shopMessage.GetComponent<SpriteRenderer>();
        shopMessageRenderer.enabled = false;
        spriteBoundaries = shopMessageRenderer.bounds;

        playerWidth = player.GetComponent<SpriteRenderer>().bounds.size.x / 2;
    }

    private void Update()
    {
        if (isInFrontOfDoor)
        {
            // check player's position until it is not in front of door
            float playerXPos = player.transform.position.x; 
            if(playerXPos > (spriteBoundaries.max.x + playerWidth) || playerXPos < (spriteBoundaries.min.x - playerWidth))
            {
                isInFrontOfDoor = false;
                shopMessageRenderer.enabled = false; 
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isInFrontOfDoor = true;
        shopMessageRenderer.enabled = true;
    }

    // interaction functions
    public void tryEnterShop(ShopTypes shopType)
    {
        if (isInFrontOfDoor)
        {
            switch (shopType) {
                case ShopTypes.Bait_Shop:
                    SceneManager.LoadScene(baitShopSceneIndex);
                    break;
                case ShopTypes.Boat_Shop:
                    SceneManager.LoadScene(boatShopSceneIndex);
                    break;
            }
        }
    }
}
