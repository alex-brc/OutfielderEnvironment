using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class TrialRunner : MonoBehaviour
{
    private enum Status { Stopped, Paused, Running, Finished }

    [Header("References")]
    public TrialManager manager;
    public PlayerController player;
    public BallController ball;
    public Text statusBox;
    public ProgressBar progressBar;
    public LastResult lastResult;
    public Button startButton;
    public Button stopButton;
    public Button pauseButton;
    public Button backButton;

    internal Image[] testBackgrounds;

    private volatile Status status;
    private Coroutine runner;
    private int[] trialIndexes;
    private System.Random rand;
    private bool pausing;
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

    private void OnEnable()
    {
        stopButton.interactable = false;
        pauseButton.interactable = false;

        status = Status.Stopped;
        rand = new System.Random();
    }

    public void StartButton()
    {
        ball.Initialise();
        StartExperiment();
        startButton.interactable = false;
        backButton.interactable = false;
    }

    public void PauseButton()
    {
        if (status == Status.Running)
        {
            pausing = true;
            PauseExperiment();
            statusBox.text = "Pausing...";
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
        statusBox.text = "Stopping...";
        statusBox.color = CustomColors.Red;

        // Restore text of pause button and block it
        pauseButton.transform.Find("Text").GetComponent<Text>().text = "Pause";
        pauseButton.interactable = false;

        // Make this button inactive
        stopButton.interactable = false;
    }

    private void UpdateBeforeTest()
    {
        // Manage highlights
        if (CurrentIndex >= 1)
            testBackgrounds[trialIndexes[CurrentIndex - 1]].color = CustomColors.Background.White; // Previous
        testBackgrounds[trialIndexes[CurrentIndex]].color = CustomColors.Background.BrightWhite; // Current

        statusBox.text = "Running test #" + (trialIndexes[CurrentIndex] + 1);
        statusBox.color = CustomColors.Black;

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
                if (pausing)
                {
                    pausing = false;
                    statusBox.text = "Paused";
                }
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
            while (manager.trialStatus != TrialManager.TrialStatus.Ready) 
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
            statusBox.text = "Stopped at " + (currentIndex+1);
            statusBox.color = CustomColors.Red;
        }
        // Show total number of catcher out of tries
        lastResult.ShowTotal(numTrials);
        // And clean hightlight
        testBackgrounds[trialIndexes[CurrentIndex - 1]].color = CustomColors.Background.White;
        // Reactivate buttons
        startButton.interactable = true;
        backButton.interactable = true;

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
        numTrials = manager.trialRuns.Get() * numTests;
        numPractices = manager.practiceRuns.Get() * numTests;

        trialIndexes = new int[NumTrials + numPractices];

        Debug.Log("numTests: " + numTests);
        Debug.Log("numTrials: " + numTrials);
        Debug.Log("numPractices: " + numPractices);

        // Fill the practices
        for (int i = 0; i < numTests; i++)
            for (int j = 0; j < manager.practiceRuns.Get(); j++)
                trialIndexes[manager.practiceRuns.Get() * i + j] = i;

        // Fill the trials
        for(int i = 0; i < numTests; i++)
            for(int j = 0; j < manager.trialRuns.Get(); j++)
                trialIndexes[numPractices + manager.trialRuns.Get() * i + j] = i;
        
        Debug.Log(PrintList(trialIndexes));

        // Shuffle the practices
        Shuffle(0, numPractices, ref trialIndexes);

        // Shuffle trials
        Shuffle(numPractices, trialIndexes.Length, ref trialIndexes);

        Debug.Log(PrintList(trialIndexes));
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

    private void Shuffle(int from, int to, ref int[] list)
    {
        int n = to;
        while (n > from + 1)
        {
            n--;
            int k = rand.Next(n + 1);
            int t = list[k];
            list[k] = list[n];
            list[n] = t;
            Debug.Log("n: " + n);
        }
    }

    private string PrintList(IEnumerable<int> list)
    {
        string s = "[";
        foreach (int e in list)
            s += " " + e +";";
       return s + " ]";
    }
}
