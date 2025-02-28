using UnityEngine;

public class playSirenGame : playFishingGame
{
    private string upBarTag = "UpBar";
    private string downBarTag = "DownBar";
    private bool isUpSide; // we also have isLeftSide in parent class

    public override void Awake()
    {
        base.Awake(); // call parent Awake() to set left / right colliders
        setUpDownColliders();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject fishPrefab = collision.gameObject;
        moveRhythmFish prefabScript = fishPrefab.GetComponent<moveRhythmFish>();
        FISHSIDE fishSide = prefabScript.getFishSide();

        FISHSIDE currFishSide; 
        // if isUpOrDown was set by setLeftRightColliders() we know that our isUpSide variable matters
        if(isUpOrDown)
        {
            if (isUpSide) currFishSide = FISHSIDE.UP;
            else currFishSide = FISHSIDE.DOWN;
        }
        else
        {
            if (isLeftSide) currFishSide = FISHSIDE.LEFT;
            else currFishSide = FISHSIDE.RIGHT;
        }

        if(currFishSide == fishSide)
        {
            prefabScript.changeFishColour(Color.green);
            successfulFish++;
        }

    }

    // HELPER METHODS
    protected void setUpDownColliders()
    {
        if (gameObject.tag == upBarTag)
        {
            isUpSide = true;
        }
        else if (gameObject.tag == downBarTag)
        {
            isUpSide = false; 
        }
        
    }
}
