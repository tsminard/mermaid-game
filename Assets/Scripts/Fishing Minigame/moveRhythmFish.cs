using UnityEngine;

public enum FISHSIDE
{
    LEFT, 
    RIGHT, 
    UP, 
    DOWN
}

// each rhythm fish prefab will have this script attached
public class moveRhythmFish : MonoBehaviour
{

    private float fishSpeed = 2.5f;
    public FISHSIDE fishSide;
    public float upperBound;

    private void Awake()
    {
        upperBound = getUpperBoundary().y;
    }

    // move fish consistently upwards in a straight line
    public void FixedUpdate()
    {
        Vector3 newPosition = new Vector3 (transform.position.x, transform.position.y + (fishSpeed * Time.deltaTime), transform.position.z);
        transform.position = newPosition; 
        if(transform.position.y >= upperBound)
        {
            Object.Destroy(gameObject); 
        }
    }

    // HELPER METHODS
    // change the colour of the fish
    public void changeFishColour(Color colour)
    {
        gameObject.GetComponent<SpriteRenderer>().color = colour;
    }

    // find the upper y bound where fish should be destroyed based on current camera position
    private Vector3 getUpperBoundary()
    {
        GameObject cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        Camera camera = cameraObject.GetComponent<Camera>();
        // get distance in z space from our main camera, which is located at -18
        float distance = Mathf.Abs(cameraObject.gameObject.transform.position.z - gameObject.transform.position.z);
        Vector3 pos = camera.ViewportToWorldPoint(new Vector3(0, 1, distance));
        return pos; 
    }

    // GETTERS + SETTERS
    // we need the side so we can check the correct collider bar
    public void setFishSide(FISHSIDE side)
    {
        fishSide = side;
        switch (side)
        {
            case FISHSIDE.LEFT:
                changeFishColour(Color.red);
                break;
            case FISHSIDE.RIGHT:
                changeFishColour(Color.blue);
                break;
        }
    }

    // lets us change fish speed depending on the fishing minigame
    public void setFishSpeed(float fishSpeed)
    {
        this.fishSpeed = fishSpeed; 
    }

    public FISHSIDE getFishSide()
    {
        return fishSide; 
    }
}
