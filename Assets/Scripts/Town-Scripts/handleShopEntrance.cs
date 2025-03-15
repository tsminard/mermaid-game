using UnityEngine;
// reusable script to check if player is interacting with a shop door
public class handleShopEntrance : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    float playerWidth;

    [SerializeField]
    GameObject shopMessage;
    SpriteRenderer shopMessageRenderer;
    Bounds spriteBoundaries; 
    bool isInFrontOfDoor = false; 
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

}
