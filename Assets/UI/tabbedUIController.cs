using UnityEngine;
using UnityEngine.UIElements;

public class TabbedUIController : MonoBehaviour
{
    public VisualElement root;
    public bool isVisible;

    // NOTE : this functionality will break if gameobject is disabled in the editor
    // Awake() will only run if the gameobject is enabled
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
