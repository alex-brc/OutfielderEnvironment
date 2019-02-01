using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TestCase : MonoBehaviour
{
    public GameObject baseball;
    public GameObject player;

    internal TrialsManager manager;
    internal GameObject testGroup;
    internal Text testCounterBox;
    internal Text testStatus;
    internal float timesPerformed = 0;

    public int testNumber;
    [Header("Test case parameters")]
    public float launchSpeed; // Meters per second
    public float launchAngle; // Degrees from plane
    public float launchDeviation; // Degrees deviation from player
    public float timesToPerform; // No of times to perform this trial
    
    void Start()
    {
        manager = TrialsManager.GetTrialsManager();

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
                    WriteCounter();
                    break;
            }
        }
        // Get test status box
        testStatus = testGroup.transform.Find("TestStatus").GetComponent<Text>();
    }

    public void LoadTest()
    {
        // Clean up
        ResetScene();
        // Load the manager with the test data
        manager.Load(this);
        // Change status
        testStatus.text = "Loaded";
        testStatus.color = CustomColors.Orange;
    }

    public void CompleteTest(bool caught)
    {
        // Cleanup 
        ResetScene();
        // Add to counter
        timesPerformed++;
        // Write status
        if (caught)
            testStatus.text = "Catch";
        else
            testStatus.text = "No Catch";
        testStatus.color = CustomColors.Black;
        // Write counter
        WriteCounter();
    }

    public void ResetTest()
    {
        testStatus.text = "Not Loaded";
        testStatus.color = CustomColors.Black;
        testCounterBox.text = "0/" + timesToPerform;
        timesPerformed = 0;
    }

    public void UnloadTest()
    {
        // Cleanup
        ResetScene();
        // Change status
        testStatus.text = "Not Loaded";
        testStatus.color = CustomColors.Black;
    }

    internal void WriteCounter()
    {
        testCounterBox.text = timesPerformed + "/" + timesToPerform;
    }

    private void ResetScene()
    {
        // Reset player
        player.transform.position = new Vector3(0, 1, -40);
        player.transform.rotation = Quaternion.Euler(0, 0, 0);

        // Reset baseball
        baseball.transform.position = new Vector3(0, 2.6f, 60);
    }
}
