using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialsManager : MonoBehaviour
{
    public enum TrialStatus { Ready, CountingDown, TrialInProgress }
    public enum PlayerStatus { InSandbox, InTrialArea }

    [Header("Test case options")]
    public Vector3 firstTestPosition;
    public float verticalOffset;
    public TestCase[] testCases;

    [Header("Positions")]
    public Vector3 catcherStartPosition;
    public Vector3 baseballHomePosition;

    [Header("Miscellaneous")]
    public int secondsCountdownBeforeStart;

    [Header("References")]
    public GameObject baseball;
    public UnityEngine.UI.Text infoBox;
    public UnityEngine.UI.Text sandboxButtonText;
    public GameObject UI;
    public GameObject overlay;
    public GameObject testsGroup;
    public DataWriter dataWriter;

    internal TestCase loadedTestCase;
    internal ICatcher catcher;
    
    internal TrialStatus trialStatus = TrialStatus.Ready;

    void FixedUpdate()
    {
        if (trialStatus == TrialStatus.TrialInProgress)
        {
            catcher.Move();
        }

    }

    internal IEnumerator StartTrial(TestCase.Type type)
    {
        if (loadedTestCase == null)
        {
            // No test loaded
            infoBox.text = "No test loaded";
            yield break;
        }
        // Update status
        trialStatus = TrialStatus.CountingDown;
        
        // Bring catcher to field
        catcher.GetRigidbody().position = catcherStartPosition;
        
        // Update text boxes
        if(type == TestCase.Type.Trial)
            loadedTestCase.testStatus.text = "In Progress";
        else if (type == TestCase.Type.Practice)
            loadedTestCase.testStatus.text = "Practice in Progress";
        else if (type == TestCase.Type.Robot)
            loadedTestCase.testStatus.text = "Robot trial in Progress";
        loadedTestCase.testStatus.color = CustomColors.Red;
        

        // Hide the UI
        UI.SetActive(false);
        
        // Activate the overlay
        overlay.SetActive(true);
        
        // Update counter color
        infoBox.color = CustomColors.Black;

        // Countdown
        float currCountdownValue = secondsCountdownBeforeStart;
        while (currCountdownValue > 0)
        {
            infoBox.text = "Starting in " + currCountdownValue + "...";
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
        }

        if (type == TestCase.Type.Trial)
            infoBox.text = "Trial running";
        else if (type == TestCase.Type.Practice)
            infoBox.text = "Practice trial running";
        else if (type == TestCase.Type.Robot)
            loadedTestCase.testStatus.text = "Robot trial runnning";

        // Update status
        trialStatus = TrialStatus.TrialInProgress;

        if (type == TestCase.Type.Practice)
            // Hack this a bit so the trial counter doesn't change
            loadedTestCase.timesPerformed--;
        
        // Start the data writer
        dataWriter.StartNewTest(loadedTestCase.testNumber, type);

        // Apply the velocity specified
        baseball.transform.position = baseballHomePosition;
        baseball.GetComponent<Rigidbody>().velocity = loadedTestCase.initialVelocityVector;
    }

    internal void CompleteTrial(bool caught)
    {
        loadedTestCase.CompleteTest(caught);
        // Update infobox
        infoBox.text = "Trial completed";
        infoBox.color = CustomColors.Black;
        // Show UI
        UI.SetActive(true);
        // Hide overlay
        overlay.SetActive(false);
        // Cleanup
        UnloadTest();
        // Send catcher home
        catcher.SendHome();
        // Bring the ball to its place
        baseball.transform.position = baseballHomePosition;
        baseball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        // Update status
        trialStatus = TrialStatus.Ready;
        // Stop writer
        dataWriter.CompleteTest(caught);
    }

    internal void LoadTest(TestCase testCase)
    {
        if(loadedTestCase != null)
        {
            // Then some other test was previously loaded
            loadedTestCase.UnloadTest();
            UnloadTest();
        }
        this.loadedTestCase = testCase;
    }

    internal void UnloadTest()
    {
        loadedTestCase = null;
    }

    public void ResetTests()
    {
        // Go through all the test cases and call reset on them
        foreach (Transform child in testsGroup.transform)
        {
            if (child.gameObject.name.Contains("TestGroup"))
            {
                // It's a test case. Reset it
                child.GetComponent<TestCase>().UnloadTest();
                child.GetComponent<TestCase>().ResetCounter();
            }
        }
    }
}
