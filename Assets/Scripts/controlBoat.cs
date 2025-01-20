using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

// script to control boat's movement 
public class controlBoat : MonoBehaviour
{
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
    
    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
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
    }

    // Update is called once per frame
    void Update()
    {
        
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
        Debug.Log("Collision detected");
        if (collision.gameObject.CompareTag(islandTagName) && !isRebounding)
        {
            isRebounding = true;
            Debug.Log("Beginning rebound");
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
    }

    // handle opening + closing menu UI
    public void OnOpenInventory()
    {
        tabController.toggleDisplay();
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
}
