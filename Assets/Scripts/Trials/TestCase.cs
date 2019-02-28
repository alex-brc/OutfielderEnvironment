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
    public int timesToPerform; // No of times to perform this trial
    public string floatFormat = "0.##";

    internal int testNumber;
    internal GameObject testCaseObject;
    internal Button loadButton;
    internal Button unloadButton;
    internal Text testCounterBox;
    internal Text testStatus;
    internal Vector3 initialVelocityVector;
    internal float timesPerformed = 0;

    private TrialsManager manager;
    /// <summary>
    /// Create a test case whose ball will land at the specified target,
    /// achieving the specified maximum ball height.
    /// </summary>
    /// <param name="target">The target point for the ball to land at</param>
    public TestCase(Vector3 target, float height, int timesToPerform)
    {
        Vector3 velocityVector = new Vector3();
        velocityVector.y = Mathf.Sqrt(-2 * (height-1) * Physics.gravity.y);
        float t =  (velocityVector.y + Mathf.Sqrt(velocityVector.y * velocityVector.y + 2 * Physics.gravity.y))/ (-1 * Physics.gravity.y);
        velocityVector.x = target.x / t;
        velocityVector.z = target.z / t;

        initialVelocityVector = velocityVector;

        launchSpeed = velocityVector.magnitude;
        launchDeviation = Mathf.Sign(velocityVector.z) * Vector3.Angle(Vector3.right, velocityVector.XZ());
        launchAngle = Vector3.Angle(Vector3.right, velocityVector.XY());

        this.timesToPerform = timesToPerform;
    }

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
                    child.GetComponent<Text>().text = launchSpeed.ToString(floatFormat) + "m/s";
                    break;
                case "AngleCol":
                    child.GetComponent<Text>().text = launchAngle.ToString(floatFormat) + "°";
                    break;
                case "DevCol":
                    child.GetComponent<Text>().text = launchDeviation.ToString(floatFormat) + "°";
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
