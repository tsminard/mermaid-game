using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; 

public class playFishingGame : MonoBehaviour
{
    [SerializeField]
    public GameObject gameManager; // this holds PlayerInput object since having multiple references to it destroys it
    public PlayerInput playerInput;
    public InputAction inputAction;

    public Collider2D barCollider;

    protected string leftBarTag = "LeftBar";
    protected string rightBarTag = "RightBar";
    protected float enabledTime = 0.2f;

    protected bool isLeftSide;
    protected bool isUpOrDown = false; 
    protected int successfulFish = 0;
    public virtual void Awake()
    {
        playerInput = gameManager.GetComponent<PlayerInput>();
        barCollider = gameObject.GetComponent<Collider2D>();
        // set bar colliders to false initially - we should only enable them when arrow keys are pressed
        setLeftRightColliders(); 
        barCollider.enabled = false;
        inputAction = isLeftSide ? playerInput.actions.FindAction("CastLeft") : playerInput.actions.FindAction("CastRight");
    }

    // we should reinstantiate successful fish count every time this script is enabled 
    protected void OnEnable()
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


    // HELPER METHODS
    protected void setLeftRightColliders()
    {
        if (gameObject.tag == leftBarTag)
        {
            isLeftSide = true;
        }
        else if (gameObject.tag == rightBarTag)
        {
            isLeftSide = false;
        }
        else
        {
            isUpOrDown = true; // this is important for playSirenGame, which allows for up / down arrows
        }
    }

    // GETTERS + SETTERS
    public int getSuccessRate()
    {
        return successfulFish; 
    }
}
