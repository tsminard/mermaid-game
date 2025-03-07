using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Script to control Controls menu buttons
public class ControlsMenuScript : MonoBehaviour
{
    [SerializeField]
    Button returnButton; 
    public void OnReturnButton()
    {
        ControlMenuButtons.changeColourOfButtonText(returnButton, new Color(255, 255, 255));
        SceneManager.LoadScene(0); // return to main menu when button is clicked
    }
}
