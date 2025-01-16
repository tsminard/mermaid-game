using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// helper class to represent an inventory / interactable object
[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [TextArea]
    public string description;

    public void init(string itemName, Sprite icon, string description)
    {
        this.itemName = itemName;
        this.icon = icon;
        this.description = description;
    }

    
    // basic constructor which requires no description
    public void init(string itemName, Sprite icon)
    {
        this.itemName = itemName;
        this.icon = icon;
        description = "No Description";
    }

}
