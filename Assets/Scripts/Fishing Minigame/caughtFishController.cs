using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// class to generate which fish is caught when player successfully completes fishing minigame 
public enum FISH_TIER
{
    S,
    A, 
    B, 
    C, 
    D
}

// internal class just to bundle fish information
public class FishSpeciesInfo
{
    string fishSpeciesName;
    FISH_TIER tier;
    float value;

    public FishSpeciesInfo(string fishSpeciesName, FISH_TIER tier, float value)
    {
        this.fishSpeciesName = fishSpeciesName;
        this.tier = tier;
        this.value = value; 
    }

    public override string ToString()
    {
        return fishSpeciesName;
    }

    // GETTERS + SETTERS
    public string getFishSpeciesName()
    {
        return fishSpeciesName;
    }

    public FISH_TIER getFishTier()
    {
        return tier; 
    }

    public float getFishValue()
    {
        return value; 
    }
}

public class caughtFishController : MonoBehaviour
{
    private FishSpeciesInfo[] possibleSpecies;
    private Dictionary<FISH_TIER, List<FishSpeciesInfo>> fishByTier;
    public bool lureUsed;

    // chances of catching each tier
    // TODO : augment these based on lures / bait used
    private int chanceOfD = 40;
    private int chanceOfC = 30;
    private int chanceOfB = 15;
    private int chanceOfA = 10;
    private int chanceOfS = 5; 
    
    private void OnEnable()
    {
        // blank any existing variables
        possibleSpecies = new FishSpeciesInfo[0];
        fishByTier = new Dictionary<FISH_TIER, List<FishSpeciesInfo>>();
        lureUsed = false; 

        // TODO : change fish retrieved based on overworld map location eventually 
        // this is OnEnable because the possible species will eventually be dependent on the boat location, which will change
        possibleSpecies = populatePossibleSpecies();
        // now we need to sort this array by FISH_TIER
        sortFishByTier(possibleSpecies);
    }

    public FishSpeciesInfo catchFish()
    {
        float catchChance = Random.Range(0, 100);
        FishSpeciesInfo caughtFish; 
        
        // this is all in an if-else block because i think we need to compare our catch chance to variables, considering our lures and bait will augment our fishing chances
        if(catchChance <= chanceOfD)
        {
            caughtFish = catchFishFromTier(FISH_TIER.D);
        }
        else if(catchChance <= chanceOfD + chanceOfC)
        {
            caughtFish = catchFishFromTier(FISH_TIER.C);
        }
        else if(catchChance <= chanceOfD + chanceOfC + chanceOfB)
        {
            caughtFish = catchFishFromTier(FISH_TIER.B);
        }
        else if(catchChance <= chanceOfD + chanceOfC + chanceOfB + chanceOfA)
        {
            caughtFish = catchFishFromTier(FISH_TIER.A);
        }
        else if(catchChance <= chanceOfD + chanceOfC + chanceOfB + chanceOfA + chanceOfS)
        {
            caughtFish = catchFishFromTier(FISH_TIER.S);
        }
        else
        {
            Debug.Log("ERROR ! catchChance value of " + catchChance + " is out of bounds");
            caughtFish = new FishSpeciesInfo("ERROR FISH", FISH_TIER.S, -100); // this is an error if you couldn't tell
        }
        Debug.Log("Caught fish " + caughtFish.ToString() + " ! "); 
        return caughtFish; 
    }

    // HELPER METHODS
    // for now, this will populate with the same fish species everywhere
    private FishSpeciesInfo[] populatePossibleSpecies()
    {
        // Build all possible fish species here, which is wretched but i'm not sure how to make them programatically
        FishSpeciesInfo[] encyclopedia =
         {
            // create fish here
            new FishSpeciesInfo("arowana", FISH_TIER.D, 10),
            new FishSpeciesInfo("tilapia", FISH_TIER.C, 12),
            new FishSpeciesInfo("koi", FISH_TIER.A, 50),

            // create garbage items here 
            new FishSpeciesInfo("boot", FISH_TIER.D, 1),
            new FishSpeciesInfo("can", FISH_TIER.D, 2),

            new FishSpeciesInfo("boot", FISH_TIER.C, 1.5f),
            new FishSpeciesInfo("can", FISH_TIER.C, 3),

            new FishSpeciesInfo("lobster-trap", FISH_TIER.B, 14.99f),
            new FishSpeciesInfo("message-in-bottle", FISH_TIER.B, 5), // declaring them all as different tiers in case we want that level of granularity
            
            new FishSpeciesInfo("message-in-bottle", FISH_TIER.A, 55),

            new FishSpeciesInfo("message-in-bottle", FISH_TIER.S, 499.99f)
        };

        return encyclopedia; 
    }

    // sort all fish in our fish list by their tier and insert them into our classwide dictionary
    // doing this in the method because this script should be the only script needing this information 
    private void sortFishByTier(FishSpeciesInfo[] fishList)
    {
        IEnumerable<IGrouping<FISH_TIER, FishSpeciesInfo>> fishDictionary = (
            from fishSpecies in fishList
            group fishSpecies by fishSpecies.getFishTier()); 
        foreach(var fishTierList in fishDictionary)
        {
            List<FishSpeciesInfo> fish = new List<FishSpeciesInfo>(); 
            foreach(FishSpeciesInfo fishInfo in fishTierList)
            {
                fish.Add(fishInfo); 
            }
            fishByTier.Add(fishTierList.Key, fish); 
        }
    }

    // method to retrieve a fish from a given tier
    private FishSpeciesInfo catchFishFromTier(FISH_TIER tier)
    {
        Debug.Log("Catching fish from tier " + tier.ToString());
        List<FishSpeciesInfo> possibleCatches = fishByTier[tier];
        int index = Random.Range(0, possibleCatches.Capacity);
        return possibleCatches[index]; 
    }

    // GETTERS + SETTERS
    // if the player uses a lure, toggle it on in this class to affect probability of siren
    public void setLureUsed(bool lureUsed)
    {
        this.lureUsed = lureUsed;
    }
}
