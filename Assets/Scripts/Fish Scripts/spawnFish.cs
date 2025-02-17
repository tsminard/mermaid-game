using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script to spawn fish and direct them to a given location
// ? should this control ALL fish for ALL sirens ? let;s make it do one for now...

public class controlFish : MonoBehaviour
{
    public GameObject fishPrefab;
    Vector2 goalLocation; 

    // Start is called before the first frame update
    void Start()
    {
        goalLocation = new Vector2(10, 25); // manually setting this for now
        int numFish = Random.Range(1, 20);
        for(int i = 0; i < numFish; i++)
        {
            // note : this modifier system DOES NOT WORK if the fishManager is placed at (0, 0, 0)
            float xLocModifier = Random.Range(0.75f, 1.75f); 
            float yLocModifier = Random.Range(0.75f, 1.75f);
            float zLocModifier = Random.Range(0.95f, 1.05f);
            Vector3 spawnLoc = new Vector3(transform.position.x * xLocModifier, transform.position.y * yLocModifier, transform.position.z * zLocModifier);
            GameObject fish = Instantiate(fishPrefab, transform);
            fish.transform.position = spawnLoc;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
