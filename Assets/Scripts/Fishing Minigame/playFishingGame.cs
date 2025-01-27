using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; 

public class playFishingGame : MonoBehaviour
{
    public PlayerInput playerInput;
    public InputAction inputAction;

    public Collider2D barCollider;

    private string leftBarTag = "LeftBar";
    private string rightBarTag = "RightBar";
    private float enabledTime = 0.5f;

    private bool isLeftSide;
    private int successfulFish = 0;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        // set bar colliders to false initially - we should only enable them when arrow keys are pressed
        if (gameObject.tag == leftBarTag)
        {
            barCollider = GameObject.FindGameObjectWithTag(leftBarTag).GetComponent<Collider2D>();
            isLeftSide = true; 
        }
        else if (gameObject.tag == rightBarTag)
        {
            barCollider = GameObject.FindGameObjectWithTag(rightBarTag).GetComponent<Collider2D>();
            isLeftSide = false; 
        }
        else
        {
            Debug.Log("INCORRECT USE OF SCRIPT - CHECK playFishingGame.cs SCRIPT AND ENSURE IT IS HOOKED UP PROPERLY");
        }

        barCollider.enabled = false;
        inputAction = isLeftSide ? playerInput.actions.FindAction("CastLeft") : playerInput.actions.FindAction("CastRight");
    }

    // we should reinstantiate successful fish count every time this script is enabled 
    private void OnEnable()
    {
        barCollider.enabled = false;
        successfulFish = 0; 
    }

    // TODO : fix exploit where you can just hold down the arrow keys to keep the colliders activated the whole time
    public void FixedUpdate()
    {
        if (inputAction.IsPressed())
        {
            StartCoroutine(enableCollider()); 
        }
    }

    // check collisions between a specific bar and fish
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject fishPrefab = collision.gameObject;
        moveRhythmFish prefabScript = fishPrefab.GetComponent<moveRhythmFish>();
        FISHSIDE fishSide = prefabScript.getFishSide();

        if ((isLeftSide && fishSide == FISHSIDE.LEFT)
            || (!isLeftSide && fishSide == FISHSIDE.RIGHT))
        {
            prefabScript.changeFishColour(Color.green); // turn fish green indicating successful catch
            successfulFish++; 
        }
    }

    // responsible for keeping collider active for only a given amount of time
    IEnumerator enableCollider()
    {
        barCollider.enabled = true;
        yield return new WaitForSeconds(enabledTime);
        barCollider.enabled = false; 
    }

    // GETTERS + SETTERS
    public int getSuccessRate()
    {
        return successfulFish; 
    }
}
