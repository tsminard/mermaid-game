using UnityEngine;
using UnityEngine.SceneManagement;

public class exitTown : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    SpriteRenderer exitMessageRenderer;
    bool displayMessage = true;
    float distance = 5f; // distance of how far the player can get before toggling the visibility of the message

    int overworldSceneIndex = 2; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        exitMessageRenderer = gameObject.GetComponent<SpriteRenderer>();
        exitMessageRenderer.enabled = true;
        displayMessage = true;
    }

    // Update is called once per frame
    void Update()
    {
        displayMessage = checkDistance();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        displayMessage = true;
        exitMessageRenderer.enabled = true;
    }

    public void closeTownScene()
    {
        if (displayMessage)
        {
            SceneManager.LoadScene(overworldSceneIndex);
        }
    }

    private bool checkDistance()
    {
        float playerX = player.transform.position.x;
        float objX = gameObject.transform.position.x; 
        if(Mathf.Abs(objX - playerX) >= distance)
        {
            exitMessageRenderer.enabled = false;
            return false;
        }
        return true;
    }

    // GETTER + SETTERS
    public bool getIsMessageDisplayed()
    {
        return displayMessage;
    }
}
