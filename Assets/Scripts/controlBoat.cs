using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

// script to control boat's movement 
public class controlBoat : MonoBehaviour
{
    [SerializeField]
    GameObject gameManager;
    public PlayerInput playerInput; // using InputActions to easily handle re-mapping controls
    public InputAction inputAction; //inputAction for movement

    // handle acceleration, deacceleration
    public float speed;
    public float maxSpeed;

    public float currSpeed;
    public Vector3 currVelocity; 

    public float accelerationValue;
    private float timeAccelerating; 

    public bool isAnchored;

    // handle collisions
    public string islandTagName;
    public bool isRebounding;
    public float reboundForce;
    private float timeRebounding;
    public float maxTimeRebounding;

    // handle UI interactions
    public UIDocument uiDocument;
    private TabbedUIController tabController; // use this to toggle UI Menu visibility 

    // handle fishing action + opening of minigame
    public bool isFishing = false;
    private bool isFishCaught = false; 
    private float fishTimer; // random amount of time to wait until a fish bites
    private float fishCountdown = 0f;
    public GameObject fishingMinigame; // gameobject parent with background of fishing game

    // Start is called before the first frame update
    void Start()
    {
        playerInput = gameManager.GetComponent<PlayerInput>();
        inputAction = playerInput.actions.FindAction("Move");
        inputAction.Enable();

        tabController = uiDocument.GetComponent<TabbedUIController>();

        speed = 2f;
        currSpeed = 0; 
        maxSpeed = 10f;
        accelerationValue = 1f;
        timeAccelerating = 0;

        isAnchored = false; 

        islandTagName = "Island";
        isRebounding = false;
        reboundForce = 0.2f;
        timeRebounding = 0;
        maxTimeRebounding = 1f;

        GetComponent<SpriteRenderer>().color = Color.yellow; // TODO : temporary fix for a visual indicator of when we're fishing
    }

    // Update is called once per frame
    void Update()
    {
        if (isFishing)
        {
            fishCountdown += Time.deltaTime;
        }
        if(isFishing && fishCountdown >= fishTimer && !isFishCaught) // condition necessary to catch a fish
        {
            catchFish();
            isFishCaught = true; 
            fishCountdown = 0; 
        }
    }

    // using this for forces to guarantee forces are applied in-sync with the engine
    private void FixedUpdate()
    {
        if (!isRebounding)
        {
            moveBoat();
        }
        else if (isRebounding) // writing it like this for clarity 
        {
            reboundBoat(); 
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(islandTagName) && !isRebounding)
        {
            isRebounding = true;
        }
    }

    // handles boat movement based on player input 
    private void moveBoat()
    {
        if(speed > 0 && !isAnchored)
        {
            if (inputAction.IsPressed())  // moves boat based on player input
            {
                Vector2 currentInput = inputAction.ReadValue<Vector2>();
                // calculate speed with acceleration
                currSpeed = speed + (accelerationValue * timeAccelerating);
                float speedValue = currSpeed > maxSpeed ? maxSpeed : currSpeed; 
                currVelocity = new Vector3(currentInput.x, currentInput.y, 0) * Time.deltaTime * speedValue;
                transform.position += currVelocity; 
                // increase time that has been passed since we began accelerating
                timeAccelerating += Time.deltaTime;
            }
            else
            {
                timeAccelerating = 0; // reset acceleration time
                // calculate drag and continue movement in this direction
                Vector3 currDrag = new Vector3(-1 * currVelocity.x, -1 * currVelocity.y, 0) * Time.deltaTime;
                currVelocity += currDrag;
                transform.position += currVelocity; 
            }   
        }
    }

    // handle drop anchor action
    public void OnDropAnchor()
    {
        isAnchored = !isAnchored; // invert anchor status
        if (!isAnchored)
        {
            GetComponent<SpriteRenderer>().color = Color.yellow; 
            isFishing = false;
            fishCountdown = 0;
            timeAccelerating = 0; 
        }
    }

    // handle opening + closing menu UI
    public void OnOpenInventory()
    {
        tabController.toggleDisplay();
    }

    // handle triggering initial fishing 
    // anchoring action is NOT disabled - lifting anchor should allow you to cancel this event at least until the minigame starts
    public void OnFish()
    {
        if (!isAnchored)
        {
            Debug.Log("Cannot fish without being anchored");
            return;
        }
        isFishing = true;
        isFishCaught = false; 
        GetComponent<SpriteRenderer>().color = Color.blue; // TODO : this is just a quick visual indicator to show you are fishing
        fishTimer = Random.Range(0.5f, 5.1f); // generate the amount of time to wait before a fish bites
        Debug.Log("Fish will be caught in " + fishTimer + " seconds");
    }

    // method called when fish is ACTUALLY CAUGHT
    private void catchFish()
    {
        Debug.Log("You caught a fish !");
        fishingMinigame.SetActive(true); 
    }

    // if we're rebounding, freeze player controls briefly 
    private void reboundBoat()
    {
        if (timeRebounding >= maxTimeRebounding)
        {
            isRebounding = false;
            timeRebounding = 0; 
        }
        Vector3 currRebound = currVelocity * reboundForce * -1;
        transform.position += currRebound;
        timeRebounding += Time.deltaTime;
    }

    // GETTERS + SETTERS
    // helper method so other scripts can set isFishing
    public void toggleIsFishing(bool isFishing)
    {
        this.isFishing = isFishing; 
    }
}
