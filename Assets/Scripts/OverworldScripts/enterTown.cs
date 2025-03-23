using UnityEngine;
using UnityEngine.SceneManagement;

public class enterTown : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    SpriteRenderer townMessageRenderer;

    bool displayMessage = false;
    float distance = 10; // amount of space between island and player before prompt text disappears
    int townSceneNumber = 4;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        townMessageRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (displayMessage)
        {
            // check distance between island and player to decide whether to turn off message
            displayMessage = isPlayerInRange();
            if (!displayMessage)
            {
                townMessageRenderer.enabled = false;
            }
        }   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        displayMessage = true;
        townMessageRenderer.enabled = true;
    }

    private bool isPlayerInRange()
    {
        float currDistance = Vector2.Distance(player.transform.position, gameObject.transform.position);
        if(currDistance < distance)
        {
            return true;
        }
        return false; 
    }

    // methods for handling player input scene load
    // to be called by external input handler
    public void openTownScene()
    {
        if (displayMessage)
        {
            SceneManager.LoadScene(townSceneNumber);
        }
    }

    // GETTERS + SETTERS
    public bool getIsBoatInRange()
    {
        return displayMessage; 
    }
}
