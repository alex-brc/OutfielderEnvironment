using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialsManager : MonoBehaviour
{
    public enum TrialStatus { Ready, CountingDown, TrialInProgress }

    [Header("Test case options")]
    public Vector3 firstTestPosition;
    public float verticalOffset;
    public float maximumBallHeight = 20;
    public float distanceToTargets = 4;
    public int trialRuns = 5;
    public int practiceRuns = 2;

    [Header("Positions")]
    public Vector3 catcherStartPosition;
    public Vector3 baseballHomePosition;

    [Header("Miscellaneous")]
    public float pauseBetweenTrials;

    [Header("References")]
    public GameObject baseball;
    public UnityEngine.UI.Text infoBox;
    public UnityEngine.UI.Text sandboxButtonText;
    public GameObject UI;
    public GameObject overlay;
    public GameObject testsGroup;
    public DataWriter dataWriter;
    public FoveInterface fove;
    public TestBuilder builder;
    public ConfigurationManager confManager;

    internal TestCase loadedTestCase;
    internal TestCase[] testCases;
    internal ICatcher catcher;
    internal TrialStatus trialStatus = TrialStatus.Ready;
    internal float startingTime = 0;
    internal float startingFrame = 0;
    
    void Start()
    {
        baseball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    void FixedUpdate()
    {
        if (trialStatus == TrialStatus.TrialInProgress)
        {
            catcher.Move();
        }
    }

    public void BuildTests()
    {
        confManager.UpdateValues();
        builder.MakeTargets();
        testCases = builder.GetTests().ToArray();
        builder.UpdateUI();
    }

    internal IEnumerator StartTrial(TestCase.TrialType type)
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
        catcher.GetRigidbody().transform.eulerAngles = new Vector3(0, -90, 0);
        
        // Update text boxes
        if(type == TestCase.TrialType.Trial)
            loadedTestCase.testStatus.text = "In Progress";
        else if (type == TestCase.TrialType.Practice)
            loadedTestCase.testStatus.text = "Practice in Progress";
        else if (type == TestCase.TrialType.Robot)
            loadedTestCase.testStatus.text = "Robot trial in Progress";
        loadedTestCase.testStatus.color = CustomColors.Red;
        
        // Update counter color
        infoBox.color = CustomColors.Black;

        // Countdown
        float currCountdownValue = pauseBetweenTrials;
        while (currCountdownValue > 0)
        {
            infoBox.text = "Starting in " + currCountdownValue + "...";
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
        }

        if (type == TestCase.TrialType.Trial)
            infoBox.text = "Trial running";
        else if (type == TestCase.TrialType.Practice)
            infoBox.text = "Practice trial running";
        else if (type == TestCase.TrialType.Robot)
            loadedTestCase.testStatus.text = "Robot trial runnning";

        // Update status
        trialStatus = TrialStatus.TrialInProgress;
        
        // Activate the data collector for the catcher
        catcher.StartDataCollector();
        // Start the data writer
        dataWriter.StartNewTest(loadedTestCase.testNumber, type);

        startingTime = Time.time;
        startingFrame = Time.frameCount;
        
        // Apply the velocity specified and unlock the ball
        baseball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        baseball.GetComponent<Rigidbody>().velocity = loadedTestCase.initialVelocityVector;
    }

    internal void CompleteTrial(bool caught)
    {
        loadedTestCase.CompleteTest(caught);
        // Update infobox
        infoBox.text = "Trial completed";
        infoBox.color = CustomColors.Black;
        // Cleanup
        UnloadTest();
        // Send catcher home
        catcher.SendHome();
        // Bring the ball to its place
        baseball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        baseball.transform.position = baseballHomePosition;
        // Update status
        trialStatus = TrialStatus.Ready;
        baseball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        // Stop writer
        catcher.StopDataCollector();
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
            }
        }
    }
}
