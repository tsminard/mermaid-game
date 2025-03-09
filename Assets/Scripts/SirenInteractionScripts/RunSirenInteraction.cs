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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        dialogueRunner.gameObject.SetActive(false);
        dialogueRunner.gameObject.SetActive(true);
        dialogueRunner.onDialogueComplete.AddListener(onDialogueComplete);

        // retrieve persist data object which contains necessary information
        persistData = GameObject.FindGameObjectWithTag("DontDestroyOnLoad").GetComponent<PersistData>();
        // handle which script is running
    }



    void onDialogueComplete()
    {
        Debug.Log("Dialogue complete");
        SceneManager.LoadScene(1); // go back to overworld
    }
}
