using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

// script to handle the day / night cycle in the overworld

public enum DayOfWeek
{
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday,
    Sunday
}

public class runDayCycle : MonoBehaviour
{
    [SerializeField]
    GameObject dayNightObj;

    [SerializeField]
    TMP_Text dayText;

    float dayLengthInSeconds = 30;
    float currTime = 0;
    float incrementTime = 1; 
    float degreesOfRotation;

    DayOfWeek[] daysOfTheWeek;
    int currDayIndex = 0; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityEditor.SceneManagement.EditorSceneManager.sceneClosing += onSceneClose; // add listener to SceneClose event
        daysOfTheWeek = (DayOfWeek[])Enum.GetValues(typeof(DayOfWeek));
        currDayIndex = PersistData.Instance.getDayOfWeekIndex();
        changeDayOfWeek();
        degreesOfRotation = 360 / dayLengthInSeconds; 
    }

    // Update is called once per frame
    void Update()
    {
        if(currTime >= dayLengthInSeconds)
        {
            currDayIndex++;
            changeDayOfWeek();
            // reset all our variables
            currTime = 0;
            incrementTime = 1;
            resetClock();
        }
        if(currTime >= incrementTime)
        {
            rotateClock(); // rotate in "ticks", not smoothly and consistently
            incrementTime += 1; 
        }
        currTime += Time.deltaTime; 
    }

    private void changeDayOfWeek()
    {
        dayText.text = daysOfTheWeek[currDayIndex].ToString();
    }

    private void rotateClock()
    {
        dayNightObj.transform.Rotate(Vector3.forward * degreesOfRotation);
    }

    private void resetClock()
    {
        dayNightObj.transform.rotation = Quaternion.identity; 
    }

    void onSceneClose(Scene scene, bool removingScene)
    {
        PersistData.Instance.persistDayOfWeek(currDayIndex); // save what day it currently before exiting scene
    }
}
