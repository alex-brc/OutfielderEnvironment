using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TrialsManager : MonoBehaviour
{
    public enum TrialStatus { Ready, CountingDown, TrialInProgress }
    
    public Configurable<int> trialRuns = new Configurable<int>();
    public Configurable<int> practiceRuns = new Configurable<int>();
    public Configurable<float> startingDistance = new Configurable<float>();
    public Configurable<float> pauseBetweenTrials = new Configurable<float>();

    [Header("Positions")]
    public Vector3 baseballHomePosition;

    [Header("Strategies")]
    public GOAC GOACObject;
    public LOT LOTObject;

    [Header("References")]
    public UnityEngine.UI.Text infoBox;
    public Rigidbody ballRb;
    public Rigidbody playerRb;
    public PlayerController player;
    public BallController ball;
    public GameObject UI;
    public GameObject overlay;
    public DataManager dataWriter;
    public FoveInterface fove;
    public TestBuilder builder;
    public ConfigurationManager confManager;
    public ViewManager viewManager;

    internal Vector3 playerStartPosition;
    internal TestCase loadedTestCase;
    private TestCase[] testCases;
    internal LinkedList<IStrategy> strategies;
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
        ballRb.constraints = RigidbodyConstraints.FreezeAll;
        strategies = new LinkedList<IStrategy>();
        strategies.AddLast(GOACObject);
        strategies.AddLast(LOTObject);
    }

    internal void Refresh()
    {
        // Set catcher position
        playerStartPosition = new Vector3(startingDistance.Get(), 0, 0);
        // Refresh UI camera distance
        viewManager.Refresh(startingDistance.Get());
    }

    void FixedUpdate()
    {
        if (trialStatus == TrialStatus.TrialInProgress)
        {
            // Move player
            player.Move();
            // Predict the correct position via different strategies
            foreach (IStrategy strat in strategies)
            {
                if (strat.IsReady())
                    strat.UpdatePrediction(Time.time - startingTime, ballRb.GetComponent<Rigidbody>());
            }
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
        playerRb.position = playerStartPosition;
        playerRb.transform.eulerAngles = new Vector3(0, -90, 0);

        // Update counter color
        infoBox.color = CustomColors.Black;

        // Countdown
        float currCountdownValue = pauseBetweenTrials.Get();
        while (currCountdownValue > 0)
        {
            infoBox.text = "Starting in " + currCountdownValue.ToString("0.0") + "...";
            yield return new WaitForSeconds(0.01f);
            currCountdownValue -= 0.01f;
        }

        // Set null position for controller
        player.controller.SetZeroPosition();

        if (type == TestCase.TrialType.Trial)
            infoBox.text = "Trial running";
        else if (type == TestCase.TrialType.Practice)
            infoBox.text = "Practice trial running";

        // Update status
        trialStatus = TrialStatus.TrialInProgress;

        // Initialise the strategies
        foreach (IStrategy strat in strategies)
            StartCoroutine(StrategyInitialiser(strat));

        // Start the data writer
        dataWriter.StartNewTest(loadedTestCase.testNumber, type);

        startingTime = Time.time;
        startingFrame = Time.frameCount;

        // Apply the velocity specified and unlock the ball
        ballRb.constraints = RigidbodyConstraints.None;
        ballRb.velocity = loadedTestCase.initialVelocityVector;
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
        player.SendHome();
        // Bring the ball to its place
        ballRb.velocity = Vector3.zero;
        ballRb.transform.position = baseballHomePosition;
        // Update status
        trialStatus = TrialStatus.Ready;
        ballRb.constraints = RigidbodyConstraints.FreezeAll;
        // Stop writer
        dataWriter.CompleteTest(caught);
        // Stop strategies
        foreach (IStrategy strat in strategies)
        {
            strat.Terminate();
        }
    }

    private IEnumerator StrategyInitialiser(IStrategy strategy)
    {
        // Wait for how many seconds it likes 
        yield return new WaitForSeconds(strategy.TimeToInit());

        // Initialise the strat
        strategy.Initialise(ballRb, playerRb);

        yield return 0;
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
