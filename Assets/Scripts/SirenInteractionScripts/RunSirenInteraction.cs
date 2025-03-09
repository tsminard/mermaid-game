using UnityEngine;
using Yarn.Unity;
using UnityEngine.SceneManagement;

// handles running siren dialogue
// TODO FILL THIS OUT !!
public class RunSirenInteraction : MonoBehaviour
{
    [SerializeField]
    DialogueRunner dialogueRunner;
    PersistData persistData;
    string nodeToPlay = "start";
    void Awake()
    {
        dialogueRunner.gameObject.SetActive(false);
        dialogueRunner.gameObject.SetActive(true);
        dialogueRunner.onDialogueComplete.AddListener(onDialogueComplete);

        // retrieve persist data object which contains necessary information
        persistData = GameObject.FindGameObjectWithTag("DontDestroyOnLoad").GetComponent<PersistData>();
        // handle which script is running
        generateNodeToPlay();
        dialogueRunner.StartDialogue(nodeToPlay);
    }


    private void generateNodeToPlay() // the name of our node to play will be based off of the siren and interaction number
    {
        string sirenName = persistData.getSiren().ToString();
        string interactionNumber = persistData.getSirenInteractionNumber().ToString();
        nodeToPlay = sirenName + "-interaction-" + interactionNumber;
        Debug.Log("Playing dialogue node " + nodeToPlay);
    }

    void onDialogueComplete() // listener to close the scene when our dialogue is over
    {
        Debug.Log("Dialogue complete");
        SceneManager.LoadScene(2); // go back to overworld
    }
}
