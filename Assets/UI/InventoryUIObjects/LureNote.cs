using UnityEngine;
using UnityEngine.UIElements;

// visual element class to handle individual notes
public class LureNote : VisualElement
{
    public Image noteImage;
    public KeyCode inputKey; // associate which key should be pressed for this note - currently Z, X, C, or V
    public bool isActivated = false; // i want to listen for input only when this lure is selected 
    public bool isPressed = false;
    string lureNoteUssName = "lureNoteContainer";
    string correctNoteUssName = "correctLureNote";
    string incorrectNoteUssName = "incorrectLureNote";

    public LureNote(KeyCode inputKey)
    {
        noteImage = new Image();
        this.inputKey = inputKey;
        AddToClassList(lureNoteUssName);
        // set height based on width to ensure square note shape
        float currWidth = resolvedStyle.width;
        style.height = currWidth;
        Add(noteImage);
        setImage();
    }

    private void setImage() // based on required inputKey
    {
        string spritePath = "Sprites/LureSprites/LureNotes/";
        string keyString = "";
        switch (inputKey)
        {
            case KeyCode.Z:
                keyString = "z";
                break;
            case KeyCode.X:
                keyString = "x";
                break;
            case KeyCode.C:
                keyString = "c";
                break;
            case KeyCode.V:
                keyString = "v";
                break;
        }
        if(keyString != "")
        {
            spritePath += keyString += "-lure";
            Debug.Log("Setting image to " + spritePath);
            Sprite noteSprite = Resources.Load<Sprite>(spritePath);
            noteImage.sprite = noteSprite;
        }
    }

    // HELPER METHODS
    // toggle whether or not a green outline appears on this note
    public void toggleCorrectNote(bool setCorrect)
    {
        if (setCorrect)
        {
            AddToClassList(correctNoteUssName);
        }
        else
        {
            RemoveFromClassList(correctNoteUssName);
        }
    }

    // toggle whether or not a red outline appears on this note
    public void toggleIncorrectNote(bool setIncorrect)
    {
        if (setIncorrect)
        {
            AddToClassList(incorrectNoteUssName);
        }
        else
        {
            RemoveFromClassList(incorrectNoteUssName);
        }
    }
}
