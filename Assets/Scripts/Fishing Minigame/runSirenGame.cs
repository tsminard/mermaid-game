using UnityEngine;

// class to run siren minigame, which is similar to the fish minigame except with more fish, faster speeds, and triggers siren event
public class runSirenGame : MonoBehaviour
{
    public GameObject fishPrefab;
    private float fishSpeed;

    // handle fish spawning
    // it's more efficient to reference game objects instead of searching for them
    [SerializeField]
    private GameObject leftSpawnLocation;
    [SerializeField]
    private GameObject rightSpawnLocation;
    [SerializeField]
    private GameObject upSpawnLocation;
    [SerializeField]
    private GameObject downSpawnLocation;

    // toggleable variables to impact game difficulty
    // TODO : toggle these. currently they're all set to a bit above regular fishing game
    float minFishSpeed = 3.5f;
    float maxFishSpeed = 10.5f;
    int minNumFish = 10;
    int maxNumFish = 20;
    float minTimeBetweenFish = 0.4f;
    float maxTimeBetweenFish = 1f;

    // handle running game
    RhythmFishData[] gamePattern;
    int currFishSpawning; // indicates how many fish we've spawned
    private float nextFishDue;
    private float currTime = 0f;

    // variables to handle endgame
    private SirenTypes siren; // this is the siren that will appear upon successful completion

    public void Awake()
    {
        if(currFishSpawning < gamePattern.Length)
        {
            if(currTime >= nextFishDue)
            {
                RhythmFishData currRhythmFish = gamePattern[currFishSpawning];
                spawnRhythmFish(currRhythmFish);
                currFishSpawning++;
                nextFishDue = gamePattern[currFishSpawning].getTimeToWait();
            }
            currTime += Time.deltaTime;
        }
        else // we are done spawning fish
        {
            int numRemainingFish = runFishingGame.getNumRemainingMinigameFish();
            if(numRemainingFish == 0) // all fish have exited the screen so we can proceed
            {

            }
        }
    }

    // these actions need to run every time the object is enabled, not just when the gameobject is 
    public void OnEnable()
    {
        fishSpeed = Random.Range(minFishSpeed, maxFishSpeed);
        // we need to handle left, right, up, down, left + right, left + up, left + down, right + up, right + down ( 9 combos )
        gamePattern = runFishingGame.generateFishingPattern(8, minNumFish, maxNumFish, minTimeBetweenFish, maxTimeBetweenFish);
        nextFishDue = gamePattern[0].getTimeToWait();
        currFishSpawning = 0; 
    }

    // private method which handles spawning the fish for all our combinations
    private void spawnRhythmFish(RhythmFishData fishData)
    {
        GameObject newFishPrefab = Instantiate(fishPrefab, this.transform); // makes prefab a child of this object
        moveRhythmFish newFishPrefabScript = newFishPrefab.GetComponent<moveRhythmFish>();
        newFishPrefabScript.setFishSpeed(fishSpeed);

        GameObject newFishPrefab2 = null;
        moveRhythmFish newFishPrefabScript2 = null;
        if (fishData.getNumFish() > 3) // for these values, we'll be spawning multiple fish
        {
            newFishPrefab2 = Instantiate(fishPrefab, this.transform);
            newFishPrefabScript2 = newFishPrefab2.GetComponent<moveRhythmFish>();
        }

        switch (fishData.getNumFish())
        {
            case 0: // spawn 1 left fish
                runFishingGame.spawnRhythmFishInWorld(newFishPrefab, newFishPrefabScript, FISHSIDE.LEFT, leftSpawnLocation.transform.position);
                break;
            case 1: // spawn 1 right fish
                runFishingGame.spawnRhythmFishInWorld(newFishPrefab, newFishPrefabScript, FISHSIDE.RIGHT, rightSpawnLocation.transform.position);
                break;
            case 2: // spawn 1 up fish
                runFishingGame.spawnRhythmFishInWorld(newFishPrefab, newFishPrefabScript, FISHSIDE.UP, upSpawnLocation.transform.position);
                break;
            case 3: // spawn 1 down fish
                runFishingGame.spawnRhythmFishInWorld(newFishPrefab, newFishPrefabScript, FISHSIDE.DOWN, downSpawnLocation.transform.position);
                break;
            case 4: // spawn 1 left, 1 right fish
                runFishingGame.spawnRhythmFishInWorld(newFishPrefab, newFishPrefabScript, FISHSIDE.LEFT, leftSpawnLocation.transform.position);
                runFishingGame.spawnRhythmFishInWorld(newFishPrefab2, newFishPrefabScript2, FISHSIDE.RIGHT, rightSpawnLocation.transform.position);
                break;
            case 5: // spawn 1 left, 1 up fish
                runFishingGame.spawnRhythmFishInWorld(newFishPrefab, newFishPrefabScript, FISHSIDE.LEFT, leftSpawnLocation.transform.position);
                runFishingGame.spawnRhythmFishInWorld(newFishPrefab2, newFishPrefabScript2, FISHSIDE.UP, upSpawnLocation.transform.position);
                break;
            case 6: //spawn 1 left, 1 down fish
                runFishingGame.spawnRhythmFishInWorld(newFishPrefab, newFishPrefabScript, FISHSIDE.LEFT, leftSpawnLocation.transform.position);
                runFishingGame.spawnRhythmFishInWorld(newFishPrefab2, newFishPrefabScript2, FISHSIDE.DOWN, downSpawnLocation.transform.position);
                break;
            case 7: // spawn 1 right, 1 up fish
                runFishingGame.spawnRhythmFishInWorld(newFishPrefab, newFishPrefabScript, FISHSIDE.RIGHT, rightSpawnLocation.transform.position);
                runFishingGame.spawnRhythmFishInWorld(newFishPrefab2, newFishPrefabScript2, FISHSIDE.UP, upSpawnLocation.transform.position);
                break;
            case 8: // spawn 1 right, 1 down fish
                runFishingGame.spawnRhythmFishInWorld(newFishPrefab, newFishPrefabScript, FISHSIDE.RIGHT, rightSpawnLocation.transform.position);
                runFishingGame.spawnRhythmFishInWorld(newFishPrefab2, newFishPrefabScript2, FISHSIDE.DOWN, downSpawnLocation.transform.position);
                break;
        }
    }

    // HELPER METHODS
    // getters + setters
    public void setSirenType(SirenTypes siren)
    {
        this.siren = siren;
    }
}
