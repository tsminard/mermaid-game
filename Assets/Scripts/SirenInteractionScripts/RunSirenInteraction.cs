using UnityEngine;
using Yarn.Unity;
using UnityEngine.SceneManagement;

// handles running siren dialogue
// TODO FILL THIS OUT !!
public class RunSirenInteraction : MonoBehaviour
{
    [SerializeField]
    DialogueRunner dialogueRunner;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueRunner.gameObject.SetActive(true);
        dialogueRunner.onDialogueComplete.AddListener(onDialogueComplete);
    }



    void onDialogueComplete()
    {
        Debug.Log("Dialogue complete");
        SceneManager.LoadScene(1); // go back to overworld
    }
}
