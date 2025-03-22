using UnityEngine;
using UnityEngine.InputSystem;

// script to control player's movement in town scene
public class controlPlayer : MonoBehaviour
{
    [SerializeField]
    GameObject gameManager;
    public PlayerInput playerInput;
    public InputAction inputAction;
    private string playerActionMapName = "MovePlayer";
    private string playerActionName = "MovePlayer";

    public float speed = .25f;

    private bool isJumping = false; 
    public float jumpSpeed = 1f;
    public float jumpTime = 0.1f;
    private float currTime = 0; 

    void Start()
    {
        playerInput = gameManager.GetComponent<PlayerInput>();
        inputAction = playerInput.actions.FindActionMap(playerActionMapName).FindAction(playerActionName);
    }

    // Update is called once per frame
    private void Update()
    {
        movePlayer();
        if (isJumping)
        {
            currTime += Time.deltaTime; 
            if(currTime >= jumpTime)
            {
                isJumping = false;
                currTime = 0f; 
            }
        }
    }

    public void movePlayer()
    {
        if (inputAction.IsPressed())
        {
            Vector2 input = inputAction.ReadValue<Vector2>();
            Vector3 velocity = new Vector3(input.x * speed, 0, 0);
            // check if jumping is allowable 
            if (input.y > 0 && !isJumping)
            {
                isJumping = true;
                float jumpForce = jumpSpeed * (jumpTime - currTime);
                velocity = new Vector3(input.x * speed, jumpForce, 0);
            }
            transform.position += velocity;
        }
    }
}
