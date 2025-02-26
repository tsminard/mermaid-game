using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class TabbedUIController : MonoBehaviour
{
    public VisualElement root;
    public bool isVisible;
    public TabView tabView;
    private List<Tab> tabs = new List<Tab>();

    // NOTE : this functionality will break if gameobject is disabled in the editor
    // Awake() will only run if the gameobject is enabled
    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.None;
        isVisible = false;
        tabView = root.Query<TabView>();
        tabView.reorderable = false; // remove reordering so we will always know which index corresponds to which tab
        tabs = tabView.Query<Tab>().ToList();
    }

    public void toggleDisplay()
    {
        if (isVisible)
        {
            root.style.display = DisplayStyle.None;
        }
        else
        {
            root.style.display = DisplayStyle.Flex;
        }
        isVisible = !isVisible; 
    }

    public void setActiveTab(int activeTabIndex)
    {
        tabView.activeTab = tabs[activeTabIndex];
    }
}
