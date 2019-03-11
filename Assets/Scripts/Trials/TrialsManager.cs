using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
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
    public GameObject UI;
    public GameObject overlay;
    public DataManager dataWriter;
    public FoveInterface fove;
    public TestBuilder builder;
    public ConfigurationManager confManager;

    internal TestCase loadedTestCase;
    private TestCase[] testCases;
    internal ICatcher catcher;
    internal TrialStatus trialStatus = TrialStatus.Ready;
    internal float startingTime = 0;
    internal float startingFrame = 0;
    internal bool testsChanged = false;
    internal bool lastResult = false;

    public TestCase[] TestCases
    {
        get
        {
            return testCases;
        }

        set
        {
            testsChanged = true;
            testCases = value;
        }
    }

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
        Debug.Log("Starting trial");
        // Update counter color
        infoBox.color = CustomColors.Black;

        // Countdown
        float currCountdownValue = pauseBetweenTrials;
        while (currCountdownValue > 0)
        {
            infoBox.text = "Starting in " + currCountdownValue.ToString("#") + "...";
            yield return new WaitForSeconds(0.01f);
            currCountdownValue -= 0.01f;
        }

        if (type == TestCase.TrialType.Trial)
            infoBox.text = "Trial running";
        else if (type == TestCase.TrialType.Practice)
            infoBox.text = "Practice trial running";

        // Update status
        trialStatus = TrialStatus.TrialInProgress;
        
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
        lastResult = caught;
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
        dataWriter.CompleteTest(caught);
    }
    
    internal void LoadTest(TestCase testCase)
    {
        this.loadedTestCase = testCase;
    }

    internal void UnloadTest()
    {
        loadedTestCase = null;
    }

    public void ResetTests()
    {
        TestCases = null;
    }
}
