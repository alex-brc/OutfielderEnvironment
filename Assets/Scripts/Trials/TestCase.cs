using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class TestCase : System.Object
{
    public enum Type { Practice, Trial, Robot };
    
    public float launchSpeed; // Meters per second
    public float launchAngle; // Degrees from plane
    public float launchDeviation; // Degrees deviation from player
    public float timesToPerform; // No of times to perform this trial

    internal int testNumber;
    internal GameObject testCaseObject;
    internal Button loadButton;
    internal Button unloadButton;
    internal Text testCounterBox;
    internal Text testStatus;
    internal Vector3 initialVelocityVector;
    internal float timesPerformed = 0;

    private TrialsManager manager;

    public void Initialise(GameObject gameObject, int testNumber, TrialsManager manager)
    {
        testCaseObject = gameObject;
        this.manager = manager;
        this.testNumber = testNumber;

        // Link buttons
        loadButton = testCaseObject.transform.Find("LoadButton").GetComponent<Button>();
        loadButton.onClick.AddListener(LoadButton);
        unloadButton = testCaseObject.transform.Find("UnloadButton").GetComponent<Button>();
        unloadButton.onClick.AddListener(UnloadButton);
        
        GameObject textCols = testCaseObject.transform.Find("TextCols").gameObject;
        
        foreach (Transform child in textCols.transform)
        {
            switch (child.name)
            {
                case "SpeedCol":
                    child.GetComponent<Text>().text = launchSpeed.ToString() + "m/s";
                    break;
                case "AngleCol":
                    child.GetComponent<Text>().text = launchAngle.ToString() + "°";
                    break;
                case "DevCol":
                    child.GetComponent<Text>().text = launchDeviation.ToString() + "°";
                    break;
                case "TimesCol":
                    testCounterBox = child.GetComponent<Text>();
                    testCounterBox.text = timesPerformed + "/" + timesToPerform;
                    break;
                case "NumCol":
                    child.GetComponent<Text>().text = "#" + testNumber;
                    break;
            }
        }
        // Get test status box
        testStatus = testCaseObject.transform.Find("TestStatus").GetComponent<Text>();

        // Compute initial velocity vector
        initialVelocityVector = Quaternion.Euler(0, -launchDeviation, launchAngle) * Vector3.right;
        // Apply magnitude
        initialVelocityVector = initialVelocityVector.normalized * launchSpeed;
    }

    public void LoadButton()
    {
        manager.LoadTest(this);

        testStatus.text = "Loaded";
        testStatus.color = CustomColors.Orange;
    }

    public void UnloadButton()
    {
        manager.UnloadTest();
        
        // Change status
        testStatus.text = "Not Loaded";
        testStatus.color = CustomColors.Black;
    }

    public void CompleteTest(bool caught)
    {
        // Add to counter
        timesPerformed++;
        // Write status
        if (caught)
            testStatus.text = "Catch";
        else
            testStatus.text = "No Catch";
        testStatus.color = CustomColors.Black;
        // Write counter
        testCounterBox.text = timesPerformed + "/" + timesToPerform;
        if (timesPerformed > timesToPerform)
            testCounterBox.color = CustomColors.Green;
    }

    public void ResetCounter()
    {
        testCounterBox.text = "0/" + timesToPerform;
        timesPerformed = 0;
    }

    public void UnloadTest()
    {
        // Change status
        testStatus.text = "Not Loaded";
        testStatus.color = CustomColors.Black;
    }
}
