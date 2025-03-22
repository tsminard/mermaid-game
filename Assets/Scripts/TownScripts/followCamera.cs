using UnityEngine;

// script to follow player character within Left / Right walls
public class followCamera : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    [SerializeField]
    GameObject leftWall;
    [SerializeField]
    GameObject rightWall;

    float leftBoundary;
    float rightBoundary;

    bool moveCamera = false; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float leftWallLoc = leftWall.transform.position.x;
        float rightWallLoc = rightWall.transform.position.x;
        // we need to subtract half the camera's viewport size from our invisible wall locations to stop the camera from leaving the scene
        Camera camera = Camera.main;
        float halfViewport = (camera.orthographicSize * camera.aspect);
        leftBoundary = leftWallLoc + halfViewport;
        rightBoundary = rightWallLoc - halfViewport;
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newPos = player.transform.position;
        if (moveCamera)
        {
            transform.position = new Vector3(newPos.x, transform.position.y, transform.position.z);
        }
        moveCamera = isPlayerInBounds();
    }

    private bool isPlayerInBounds()
    {
        if (player.transform.position.x < leftBoundary)
        {
            return false;
        }
        else if(player.transform.position.x > rightBoundary)
        {
            return false;
        }
        return true;
    }
}
