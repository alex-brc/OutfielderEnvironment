using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(TrialsManager))]
public class AutoTrialRunner : MonoBehaviour
{
    private enum Status { Stopped, Paused, Running, Finished }

    [Header("References")]
    public PlayerController player;
    public Text statusBox;
    public ProgressBar progressBar;
    public LastResult lastResult;
    public Button startButton;
    public Button stopButton;
    public Button pauseButton;

    internal Image[] testBackgrounds;

    private volatile Status status;
    private Coroutine runner;
    private TrialsManager manager;
    private int[] trialIndexes;
    private System.Random rand;
    private int currentIndex;
    private int numTests;
    private int numTrials;
    private int numPractices;

    internal int NumTests
    {
        get
        {
            return numTests;
        }
    }
    internal int NumTrials
    {
        get
        {
            return numTrials;
        }
    }
    internal int NumPractices
    {
        get
        {
            return numPractices;
        }
    }
    internal int CurrentIndex
    {
        get
        {
            return currentIndex;
        }
    }

    private void Start()
    {
        stopButton.interactable = false;
        pauseButton.interactable = false;

        status = Status.Stopped;
        statusBox.text = "Ready to start";
        manager = GetComponent<TrialsManager>();
        rand = new System.Random();
    }

    public void StartButton()
    {
        StartExperiment();
        startButton.interactable = false;
    }

    public void PauseButton()
    {
        if (status == Status.Running)
        {
            PauseExperiment();
            statusBox.text = "Paused";
            statusBox.color = CustomColors.Orange;
            testBackgrounds[trialIndexes[currentIndex]].color = CustomColors.Background.Red;
            pauseButton.transform.Find("Text").GetComponent<Text>().text = "Resume";
            stopButton.interactable = true;
        }
        else if (status == Status.Paused)
        {
            ResumeExperiment();
            statusBox.text = "Running";
            statusBox.color = CustomColors.Black;
            testBackgrounds[trialIndexes[currentIndex]].color = CustomColors.Background.Orange;
            pauseButton.transform.Find("Text").GetComponent<Text>().text = "Pause";
            stopButton.interactable = false;
        }
    }

    public void StopButton()
    {
        StopExperiment();
    }

    private void UpdateBeforeTest()
    {
        // Manage highlights
        if (CurrentIndex >= 1)
            testBackgrounds[trialIndexes[CurrentIndex - 1]].color = CustomColors.Background.White; // Previous
        testBackgrounds[trialIndexes[CurrentIndex]].color = CustomColors.Background.BrightWhite; // Current

        statusBox.text = "Running test #" + (trialIndexes[CurrentIndex] + 1);
    }

    private void UpdateAfterTest(TestCase.TrialType trialType)
    {
        // Show last result
        lastResult.UpdateResult(manager.lastResult, trialType);

        // Update progress bars
        progressBar.UpdateBars(trialType);
    }

    /// <summary>
    /// Coroutine that handles loading and unloading, as 
    /// well as starting and stopping them.
    /// </summary>
    private IEnumerator Runner(int[] testIndexes)
    {
        currentIndex = 0;
        while((status == Status.Running || status == Status.Paused)
            && currentIndex < testIndexes.Length)
        {
            if (status == Status.Paused)
            { 
                yield return new WaitForSeconds(0.05f);
                continue;
            }
            // Load the test with number index[currentIndex]
            manager.LoadTest(manager.TestCases[testIndexes[currentIndex]]);
            
            // Start the trial or practice
            TestCase.TrialType currentType;
            if(currentIndex < numPractices)
                currentType = TestCase.TrialType.Practice;
            else
                currentType = TestCase.TrialType.Trial;
            StartCoroutine(manager.StartTrial(currentType));

            // Update view
            UpdateBeforeTest();

            // Wait for the trial to finish
            yield return new WaitForSeconds(0.1f);
            while (manager.trialStatus != TrialsManager.TrialStatus.Ready) 
                yield return new WaitForSeconds(0.05f);

            // Update bars and result
            UpdateAfterTest(currentType);

            // Advance
            currentIndex++;
        }

        // Stopped or finished?
        if (currentIndex == testIndexes.Length) // Then it finished
        {
            status = Status.Finished;
            statusBox.text = "Finished OK";
        }
        else
        {
            status = Status.Stopped;
            statusBox.text = "Stopped at " + (currentIndex+1);
            statusBox.color = CustomColors.Red;
        }
        // Show total number of catcher out of tries
        lastResult.ShowTotal(numTrials);
        // And clean hightlight
        testBackgrounds[trialIndexes[CurrentIndex - 1]].color = CustomColors.Background.White;
        // Reactivate button
        startButton.interactable = true;

        yield return 0;
    }

    /// <summary>
    /// Runs through the trials in a random order, until 
    /// each of them has been performed the required number
    /// of times required.
    /// </summary>
    private void StartExperiment()
    {
        numTests = manager.TestCases.Length;
        numTrials = manager.trialRuns * numTests;
        numPractices = manager.practiceRuns * numTests;

        trialIndexes = new int[NumTrials + numPractices];

        // Fill the testIndexes with trialRuns+practiceRuns copies of each test case index
        for (int i = 0; i < numTests; i++)
            for (int j = 0; j < (manager.trialRuns + manager.practiceRuns); j++)
                trialIndexes[i * manager.trialRuns + j] = i;

        // Shuffle the practices
        int t,r;
        for (int i = numPractices - 1; i >= 0; i--) {
            r = rand.Next(0,i);
            t = trialIndexes[i];
            trialIndexes[i] = trialIndexes[r];
            trialIndexes[r] = t;
        }
        // Shuffle the trials
        for (int i = trialIndexes.Length - 1; i >= numPractices; i--)
        {
            r = rand.Next(0, i);
            t = trialIndexes[i];
            trialIndexes[i] = trialIndexes[r];
            trialIndexes[r] = t;
        }

        // Set the catcher
        manager.player = player;

        // Finally, update the status and buttons
        status = Status.Running;

        // Reset and start the bars
        progressBar.Initialise();
        statusBox.text = "Running";
        
        // Start the runner
        runner = StartCoroutine(Runner(trialIndexes));

        pauseButton.interactable = true;
    }

    /// <summary>
    /// Pauses running through trials. Lets the running trial 
    /// finish.
    /// </summary>
    private void PauseExperiment()
    {
        status = Status.Paused;
    }

    private void ResumeExperiment()
    {
        status = Status.Running;
    }

    /// <summary>
    /// Stops the experiment. Can only stop while paused.
    /// </summary>
    private void StopExperiment()
    {
        status = Status.Stopped;
    }
}
