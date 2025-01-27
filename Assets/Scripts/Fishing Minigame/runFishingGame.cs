using UnityEngine;
using TMPro; // UI Text import
using System.Collections; 

// helper class to concisely hold data for spawning fish
 class RhythmFishData
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
    public float minimumFishSpeed = 2f;
    public float maximumFishSpeed = 3f; 

    // handle rhythm fish spawning
    public float nextFishDue;
    private float currTime = 0;
    private int currFishSpawning;

    public GameObject fishPrefab;
    private float fishSpeed; // classwide so all fish move the same speed
    private Vector3 leftFishLocation = new Vector3(7.35f, -7.85f, -2.0f);
    private Vector3 rightFishLocation = new Vector3(12.15f, -7.85f, -2.0f);

    // variables for deciding success of fishing
    private playFishingGame[] fishingGameResults;
    private float successPercentage = 0.8f; // TODO : make this a changeable value based on lures, etc. 

    // variables for displaying result
    public TMP_Text uiFishStatus;

    public void Awake()
    {
        // transform our locations to worldspace
        leftFishLocation = this.transform.TransformPoint(leftFishLocation);
        rightFishLocation = this.transform.TransformPoint(rightFishLocation);
        fishSpeed = Random.Range(minimumFishSpeed, maximumFishSpeed);

        // retrieve references to scripts attached to our bar colliders
        fishingGameResults = new playFishingGame[2];
        fishingGameResults[0] = GameObject.FindGameObjectWithTag("LeftBar").GetComponent<playFishingGame>();
        fishingGameResults[1] = GameObject.FindGameObjectWithTag("RightBar").GetComponent<playFishingGame>(); 

        generateFishingPattern();
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
            }
            // if we still have fish to spawn in the future, update the amount of time to wait
            if (currFishSpawning < gamePattern.Length)
            {
                nextFishDue = gamePattern[currFishSpawning].getTimeToWait(); 
            }
            currTime += Time.deltaTime; // increment timer towards next fish
        }
        else // we are done spawning fish and can start checking for remaining fish
        {
            GameObject[] remainingFish = GameObject.FindGameObjectsWithTag("MinigameFish"); // DUDE THERE ARE UNDERWATER FISH TOO YOU NEED MORE TAGS
            if (remainingFish.Length == 0) // this is pretty inefficient, but i think message broadcasts only work on children
            {
                Debug.Log("All fish destroyed"); 
                // trigger game end
                // get fish status and display status
                bool caughtFish = isFishingSuccessful();
                if (caughtFish)
                {
                    uiFishStatus.text = "You caught a fish !"; 
                }
                else
                {
                    uiFishStatus.text = "The fish got away...."; 
                }
                StartCoroutine(waitBeforeClosingMinigame()); 
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
        Debug.Log("Successful fish : " + successfulFish);
        Debug.Log("Total fish spawned : " + gamePattern.Length); 
        if (successfulFish / gamePattern.Length >= successPercentage)
        {
            return true; 
        }
        return false; // default return 
    }

    IEnumerator waitBeforeClosingMinigame()
    {
        Debug.Log("Waiting 2 seconds before closing scene");
        yield return new WaitForSeconds(2);
        Debug.Log("Closing scene");
        gameObject.SetActive(false); 
    }

    private void generateFishingPattern()
    {
        // first, generate the number of fish to spawn this minigame
        int numFish = Random.Range(minimumFish, maximumFish);
        gamePattern = new RhythmFishData[numFish]; 
        // next, generate this many fish data points
        for(int i = 0; i < numFish; i++)
        {
            RhythmFishData newFishData = generateRhythmFishData();
            gamePattern[i] = newFishData; 
        }
    }

    // does what it says on the tin
    private RhythmFishData generateRhythmFishData()
    {
        // generate number of fish 
        int numFish = Random.Range(0, 2);
        float timeToWait = Random.Range(minimumTime, maximumTime);
        return new RhythmFishData(numFish, timeToWait); 
    }

    // HELPER METHOD FOR TESTING
    private void generateTestRhythmFishData()
    {
        RhythmFishData testRhythmFishLeft = new RhythmFishData(0, 1f);
        RhythmFishData testRhythmFishRight = new RhythmFishData(1, 1f);
        gamePattern = new RhythmFishData[2];
        gamePattern[0] = testRhythmFishLeft;
        gamePattern[1] = testRhythmFishRight;
    }

    // instantiates fish prefabs based on data in helper class
    private void spawnRhythmFish(RhythmFishData fishData)
    {
        // this logic holds for all fish spawned this game
        GameObject newFishPrefab = Instantiate(fishPrefab, this.transform); // prefabs must be instantiated as children of the current game object
        moveRhythmFish newFishPrefabScript = newFishPrefab.GetComponent<moveRhythmFish>();
        newFishPrefabScript.setFishSpeed(fishSpeed);

        switch (fishData.getNumFish())
        {
            case 0: // spawn 1 left fish
                
                newFishPrefab.transform.position = leftFishLocation; 
                newFishPrefabScript.setFishSide(FISHSIDE.LEFT);
                break;
            case 1: // spawn 1 right fish
                newFishPrefab.transform.position = rightFishLocation;
                newFishPrefabScript.setFishSide(FISHSIDE.RIGHT); 
                break;
            case 2: // spawn both left and right fish
                GameObject newFishPrefab2 = Instantiate(fishPrefab, this.transform);
                moveRhythmFish newFishPrefabScript2 = newFishPrefab2.GetComponent<moveRhythmFish>();
                newFishPrefab.transform.position = leftFishLocation;
                newFishPrefabScript.setFishSide(FISHSIDE.LEFT);
                newFishPrefab2.transform.position =rightFishLocation;
                newFishPrefabScript2.setFishSide(FISHSIDE.RIGHT);
                newFishPrefabScript2.setFishSpeed(fishSpeed);
                break; 
        }
    }
}
