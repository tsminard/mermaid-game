using UnityEngine;
using TMPro; // UI Text import
using System.Collections;
using UnityEngine.UI;

// helper class to concisely hold data for spawning fish
 public class RhythmFishData
{   
    // 0 fish : left fish
    // 1 fish : right fish
    // 2 fish : both fish
    int numFish; 
    float timeToWait;

    public RhythmFishData(int numFish, float timeToWait)
    {
        this.numFish = numFish;
        this.timeToWait = timeToWait; 
    }

    public override string ToString()
    {
        return "RhythmFish : (" + numFish + ", " + timeToWait + ")";
    }

    // GETTERS
    public int getNumFish()
    {
        return numFish;
    }

    public float getTimeToWait()
    {
        return timeToWait; 
    }
}
public class runFishingGame : MonoBehaviour
{
    private RhythmFishData[] gamePattern; // represents the fish that will be generated
    public int minimumFish = 7; // TODO : these can be toggled depending on how the game feels
    public int maximumFish = 15;
    public float minimumTime = 0.5f; // TODO : these can be toggled depending on how the game feels
    public float maximumTime = 1.4f;
    public float minimumFishSpeed = 3f;
    public float maximumFishSpeed = 9f; 

    // handle rhythm fish spawning
    public float nextFishDue;
    private float currTime = 0;
    private int currFishSpawning;

    public GameObject fishPrefab;
    private float fishSpeed; // classwide so all fish move the same speed
    public GameObject leftSpawnLocation;
    public GameObject rightSpawnLocation;
    private Vector3 leftFishLocation;
    private Vector3 rightFishLocation;

    // variables for deciding success of fishing
    private playFishingGame[] fishingGameResults;
    private float successPercentage = 0.8f; // TODO : make this a changeable value based on lures, etc. 
    private int numFishSpawned = 0; // count actual number of fish spawned
    ItemManager itemManager;

    // variables for displaying result
    public TMP_Text uiFishStatus;
    public Image uiFishImage;
    public caughtFishController caughtFishController;
    bool hasTextDisplayed = false; 

    // these actions just need to be run once when the script is instantiated
    private void Awake()
    {       
        // retrieve references to scripts attached to our bar colliders
        fishingGameResults = new playFishingGame[2];
        fishingGameResults[0] = GameObject.FindGameObjectWithTag("LeftBar").GetComponent<playFishingGame>();
        fishingGameResults[1] = GameObject.FindGameObjectWithTag("RightBar").GetComponent<playFishingGame>();

        caughtFishController = gameObject.GetComponent<caughtFishController>();
        itemManager = ItemManager.Instance;
    }

    // these actions need to run every time the object is enabled, not just when the gameobject is 
    public void OnEnable()
    {
        hasTextDisplayed = false;
        uiFishImage.enabled = false;
        uiFishStatus.text = ""; // blank any existing text 

        leftFishLocation = leftSpawnLocation.transform.position;
        rightFishLocation = rightSpawnLocation.transform.position;

        fishSpeed = Random.Range(minimumFishSpeed, maximumFishSpeed);

        // fishing game only uses left and right fish, so we pass in 2 (left, right, both)
        gamePattern = generateFishingPattern(2, minimumFish, maximumFish, minimumTime, maximumTime);
        Debug.Log("Generated fishing pattern of " + gamePattern.Length + " fish");
        nextFishDue = gamePattern[0].getTimeToWait(); // we should ALWAYS have at least one fish in this pattern
        Debug.Log("Next fish due in " + nextFishDue);
        currFishSpawning = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // if we still have fish to spawn, spawn them 
        if (currFishSpawning < gamePattern.Length)
        {
            if (currTime >= nextFishDue)
            {
                RhythmFishData currRhythmFish = gamePattern[currFishSpawning];
                spawnRhythmFish(currRhythmFish);
                currTime = 0; // reset timer
                currFishSpawning++; // increment number to indicate that we have spawned this fish
                if(currFishSpawning < gamePattern.Length)
                {
                    nextFishDue = gamePattern[currFishSpawning].getTimeToWait(); // if we still have fish to spawn in the future, update the amount of time to wait
                }
            }
            currTime += Time.deltaTime; // increment timer towards next fish
        }
        else // we are done spawning fish and can start checking for remaining fish
        {
            int numRemainingFish = getNumRemainingMinigameFish();
            if (numRemainingFish == 0) // this is pretty inefficient, but i think message broadcasts only work on children
            {
                // trigger game end
                // get fish status and display status
                bool caughtFish = isFishingSuccessful();
                if (caughtFish && !hasTextDisplayed)
                {
                    StartCoroutine(catchFish());
                }
                else if (!caughtFish)
                {
                    StartCoroutine(loseFish());
                }
            }
        }
    }

    // method to hide fishing minigame after everything is completed 
    public void endGame()
    {
        gameObject.SetActive(false); 
    }

    // HELPER METHODS
    // retrieve score from our left and right colliders
    private bool isFishingSuccessful()
    {
        int successfulFish = 0; 
        foreach (playFishingGame fishingGame in fishingGameResults)
        {
            successfulFish += fishingGame.getSuccessRate(); 
        }
        if (successfulFish / numFishSpawned >= successPercentage)
        {
            return true; 
        }
        return false; // default return 
    }

    // COUROUTINES

    IEnumerator catchFish()
    {
        hasTextDisplayed = true;
        uiFishStatus.text = "You caught a fish !";
        // generate which kind of fish was caught based on probabilities
        FishSpeciesInfo fish = caughtFishController.catchFish();
        // retrieve caught item information
        ItemDetails fishingCatch = itemManager.getItemByName(fish.getFishSpeciesName());
        Sprite fishSprite = fishingCatch.itemData.icon;
        // display results
        uiFishImage.overrideSprite = fishSprite;
        uiFishImage.enabled = true; 
        uiFishStatus.text = "You caught a " + fish.ToString();

        // add catch to inventory
        inventoryController.addItemToInventory(fishingCatch, ItemInventoryType.Fish);

        yield return new WaitForSeconds(2.5f);
        Debug.Log("Closing scene");
        gameObject.SetActive(false); 
    }

    // wait 2 seconds before hiding gameobjects
    IEnumerator loseFish()
    {
        uiFishStatus.text = "The fish got away....";
        yield return new WaitForSeconds(2);
        Debug.Log("Closing scene");
        gameObject.SetActive(false); 
    }

    // FISH SPAWNING RELATED METHODS

    // converting this to static so Siren Game can use this method as well to generate fishing patterns
    public static RhythmFishData[] generateFishingPattern(int maxRhythmFish, int minNumFish, int maxNumFish, float minWaitTime, float maxWaitTime)
    {
        // first, generate the number of fish to spawn this minigame
        int numFish = Random.Range(minNumFish, maxNumFish);
        RhythmFishData[] generatedPattern = new RhythmFishData[numFish];
        // next, generate this many fish data points
        for(int i = 0; i < numFish; i++)
        {
            RhythmFishData newFishData = generateRhythmFishData(maxRhythmFish, minWaitTime, maxWaitTime);
            generatedPattern[i] = newFishData;
        }
        return generatedPattern;
    }

    // does what it says on the tin
    // maxFish variable indicates the maximum value of RhythmFishData class 
    // it is a variable so we can use RhythmFishData for spawning any number of fish
    public static RhythmFishData generateRhythmFishData(int maxFish, float minWaitTime, float maxWaitTime)
    {
        // generate number of fish 
        int numFish = Random.Range(0, maxFish);
        float timeToWait = Random.Range(minWaitTime, maxWaitTime);
        return new RhythmFishData(numFish, timeToWait); 
    }

    // instantiates fish prefabs based on data in helper class
    private void spawnRhythmFish(RhythmFishData fishData)
    {
        // this logic holds for all fish spawned this game
        GameObject newFishPrefab = Instantiate(fishPrefab, this.transform); // prefabs must be instantiated as children of the current game object
        moveRhythmFish newFishPrefabScript = newFishPrefab.GetComponent<moveRhythmFish>();
        newFishPrefabScript.setFishSpeed(fishSpeed);
        numFishSpawned += 1;

        switch (fishData.getNumFish())
        {
            case 0: // spawn 1 left fish
                spawnRhythmFishInWorld(newFishPrefab, newFishPrefabScript, FISHSIDE.LEFT, leftFishLocation);
                break;
            case 1: // spawn 1 right fish
                spawnRhythmFishInWorld(newFishPrefab, newFishPrefabScript, FISHSIDE.RIGHT, rightFishLocation);
                break;
            case 2: // spawn both left and right fish
                GameObject newFishPrefab2 = Instantiate(fishPrefab, this.transform);
                moveRhythmFish newFishPrefabScript2 = newFishPrefab2.GetComponent<moveRhythmFish>();
                newFishPrefabScript2.setFishSpeed(fishSpeed);
                spawnRhythmFishInWorld(newFishPrefab, newFishPrefabScript, FISHSIDE.LEFT, leftFishLocation);
                spawnRhythmFishInWorld(newFishPrefab2, newFishPrefabScript2, FISHSIDE.RIGHT, rightFishLocation);
                numFishSpawned += 1;
                break; 
        }
    }

    // FISH SPAWNING HELPER METHODS
    public static void spawnRhythmFishInWorld(GameObject fishPrefab, moveRhythmFish fishScript, FISHSIDE fishSide, Vector3 loc)
    {
        fishPrefab.transform.position = loc;
        fishScript.setFishSide(fishSide); 
    }

    // making this a static method just in case i change how i retrieve minigame fish in the future
    public static int getNumRemainingMinigameFish()
    {
        GameObject[] remainingFish = GameObject.FindGameObjectsWithTag("MinigameFish"); // DUDE THERE ARE UNDERWATER FISH TOO YOU NEED MORE TAGS
        return remainingFish.Length;
    }
}
