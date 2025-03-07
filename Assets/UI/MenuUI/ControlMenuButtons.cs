using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; 

// Script to hook up Menu buttons with other scenes
public class ControlMenuButtons : MonoBehaviour
{
    [SerializeField]
    Button startButton;
    [SerializeField]
    Button infoButton;
    [SerializeField]
    Button quitButton;
    
    public void OnStartButton()
    {
        changeColourOfButtonText(startButton, new Color(255, 255, 255));
        SceneManager.LoadScene(2);
    }

    public void OnControlsButton()
    {
        changeColourOfButtonText(startButton, new Color(255, 255, 255));
        SceneManager.LoadScene(1);
    }

    public void OnQuitButton()
    {
        changeColourOfButtonText(startButton, new Color(255, 255, 255));
        Application.Quit();
    }

    // helper methods
    public static void changeColourOfButtonText(Button button, Color c)
    {
        TMP_Text text = button.GetComponentInChildren<TMP_Text>();
        if(text != null)
        {
            text.color = c; 
        }
        else
        {
            Debug.Log("Could not find Text in button");
        }
    }
}
