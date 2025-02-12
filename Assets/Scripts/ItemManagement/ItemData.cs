using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// helper class to represent an inventory / interactable object
[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public float value;
    [TextArea]
    public string description;

    public void init(string itemName, Sprite icon, float value, string description)
    {
        this.itemName = itemName;
        this.icon = icon;
        this.value = value;
        this.description = description;
    }
    // basic contructor which requires no value
    public void init(string itemName, Sprite icon, string description)
    {
        this.itemName = itemName;
        this.icon = icon;
        this.value = 0;
        this.description = description;
    }

    // basic constructor which requires no value or description
    public void init(string itemName, Sprite icon)
    {
        this.itemName = itemName;
        this.icon = icon;
        this.value = 0;
        description = "Undescribable horror...";
    }

}
