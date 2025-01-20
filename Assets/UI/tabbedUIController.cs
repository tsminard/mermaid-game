using UnityEngine;
using UnityEngine.UIElements;

public class TabbedUIController : MonoBehaviour
{
    private VisualElement root;
    public bool isVisible; 

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        root.style.visibility = Visibility.Hidden;
        isVisible = false; 
    }

    public void toggleDisplay()
    {
        if (isVisible)
        {
            root.style.visibility = Visibility.Hidden;
        }
        else
        {
            root.style.visibility = Visibility.Visible; 
        }
        isVisible = !isVisible; 
    }
}
