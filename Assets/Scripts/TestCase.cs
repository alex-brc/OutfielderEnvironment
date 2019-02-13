using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TestCase : MonoBehaviour
{
    public enum Type { Practice, Trial, Robot };

    internal GameObject testGroup;
    internal Text testCounterBox;
    internal Text testStatus;
    internal Vector3 initialVelocityVector;
    internal float timesPerformed = 0;

    public int testNumber;
    [Header("Test case parameters")]
    public float launchSpeed; // Meters per second
    public float launchAngle; // Degrees from plane
    public float launchDeviation; // Degrees deviation from player
    public float timesToPerform; // No of times to perform this trial
    
    [Header("References")]
    public GameObject baseball;
    public GameObject player;
    public TrialsManager manager;

    void Start()
    {
        // Test group is the group object of this test case
        testGroup = gameObject;
        GameObject textCols = testGroup.transform.Find("TextCols").gameObject;
        
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
            }
        }
        // Get test status box
        testStatus = testGroup.transform.Find("TestStatus").GetComponent<Text>();

        // Compute initial velocity vector
        initialVelocityVector = Quaternion.Euler(0, launchDeviation, launchAngle) * Vector3.right;
        // Apply magnitude
        initialVelocityVector = initialVelocityVector.normalized * launchSpeed;
    }

    public void LoadButton()
    {
        manager.Load(this);

        testStatus.text = "Loaded";
        testStatus.color = CustomColors.Orange;
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
