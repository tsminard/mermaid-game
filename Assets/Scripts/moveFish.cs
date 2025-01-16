using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script to control fish movement
public class moveFish : MonoBehaviour
{
    Vector3 goalLocation;
    float circlingDistance;
    float timeCounter; 

    // Start is called before the first frame update
    void Start()
    {
        // set default values if we don't get them ( even though this shouldn't happen )
        goalLocation = new Vector3(0, 0, transform.position.z);
        circlingDistance = 5f;
        timeCounter = 0; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // using this to control fish movement towards goal
    private void FixedUpdate()
    {
        if(Vector3.Distance(transform.position, goalLocation) <= circlingDistance)
        {
            // circle that point
            timeCounter += Time.deltaTime;
            float x = Mathf.Cos(timeCounter);
            float y = Mathf.Sin(timeCounter);
            transform.position = new Vector3(x, y, transform.position.z);
            if(timeCounter > 10000000) // reset when it gets large
            {
                timeCounter = 0; 
            }
        }
        else
        {
            // move towards goal location
        }
    }

    // getters + setters
    public void setGoalLocation(Vector2 goalLocation)
    {
        this.goalLocation = goalLocation;
    }

    public void setCirclingDistance(float circlingDistance)
    {
        this.circlingDistance = circlingDistance;
    }
}
