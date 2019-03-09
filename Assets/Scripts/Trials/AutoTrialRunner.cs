using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(TrialsManager))]
public class AutoTrialRunner : MonoBehaviour
{
    private enum Status { Stopped, Paused, Running, Finished }
    private enum Type { Trial, Practice }

    [Header("References")]
    public PlayerController player;
    public Text statusBox;
    public Button startButton;
    public Button practiceButton;
    public Button stopButton;
    public Button pauseButton;

    internal Image[] testBackgrounds;

    private Status status;
    private Coroutine runner;
    private TrialsManager manager;
    private int[] testIndexes;
    private System.Random rand;
    private int currentIndex;

    private void Start()
    {
        status = Status.Stopped;
        statusBox.text = "Ready to start";
        manager = GetComponent<TrialsManager>();
        rand = new System.Random();
        stopButton.interactable = false;
        pauseButton.interactable = false;
    }

    public void StartButton()
    {
        Start(Type.Trial);
        practiceButton.interactable = false;
    }

    public void PracticeButton()
    {
        Start(Type.Practice);
    }

    /// <summary>
    /// Coroutine that handles loading and unloading, as 
    /// well as starting and stopping them.
    /// </summary>
    private IEnumerator Runner(int[] testIndexes, Type type)
    {
        for (currentIndex = 0;
            (status == Status.Running || status == Status.Paused)
            && currentIndex < testIndexes.Length;
            currentIndex++)
        {
            if (status == Status.Paused)
            {
                yield return new WaitForSeconds(0.05f);
                continue;
            }
            // Load the test with number index[currentIndex]
            manager.LoadTest(manager.TestCases[testIndexes[currentIndex]]);

            // Set null position for controller
            player.SetZeroPosition();

            // Start the trial
            StartCoroutine(manager.StartTrial(TestCase.TrialType.Trial));

            Debug.Log("currentIndex:" + currentIndex);
            
            // Manage highlights
            testBackgrounds[testIndexes[currentIndex]].color = CustomColors.Background.Orange; // Current
            if(currentIndex >= 1)
                testBackgrounds[testIndexes[currentIndex-1]].color = CustomColors.Background.Green; // Previous
            if(currentIndex >= 2)
                testBackgrounds[testIndexes[currentIndex-2]].color = CustomColors.Background.White; // Previous green one

            // Write status
            statusBox.text = "Currently at " + (currentIndex + 1) + "/" + testIndexes.Length;

            // Wait for the trial to finish
            yield return new WaitForSeconds(0.1f);
            while (manager.trialStatus != TrialsManager.TrialStatus.Ready) 
                yield return new WaitForSeconds(0.05f);
        }

        // Stopped or finished?
        if (currentIndex == testIndexes.Length) // Then it finished
        {
            status = Status.Finished;
            statusBox.text = "Finished";
            statusBox.color = CustomColors.Green;
        }
        else
        {
            status = Status.Stopped;
            statusBox.text = "Stopped";
            statusBox.color = CustomColors.Red;
        }
        yield return 0;
    }

    /// <summary>
    /// Runs through the trials in a random order, until 
    /// each of them has been performed the required number
    /// of times required.
    /// </summary>
    private void Start(Type type)
    {
        testIndexes = new int[manager.TestCases.Length * manager.trialRuns];
        // Fill the testIndexes with timesToRunEachTrial copies of each test case index
        for (int i = 0; i < manager.TestCases.Length; i++)
            for (int j = 0; j < manager.trialRuns; j++)
                testIndexes[i * manager.trialRuns + j] = i;

        // Shuffle them
        int t,r;
        for (int i = testIndexes.Length - 1; i >= 0; i--) {
            r = rand.Next(0,i);
            t = testIndexes[i];
            testIndexes[i] = testIndexes[r];
            testIndexes[r] = t;
        }
        
        // Set the catcher
        manager.catcher = player;

        // Finally, update the status and buttons
        status = Status.Running;

        // Start the runner
        runner = StartCoroutine(Runner(testIndexes, type));

        pauseButton.interactable = true;
    }

    /// <summary>
    /// Pauses running through trials. Lets the running trial 
    /// finish.
    /// </summary>
    public void Pause()
    {
        if (status == Status.Running)
        {
            status = Status.Paused;
            statusBox.text = "Paused";
            statusBox.color = CustomColors.Orange;
            testBackgrounds[testIndexes[currentIndex]].color = CustomColors.Background.Red;
            pauseButton.GetComponent<Text>().text = "Resume";
            stopButton.interactable = true;
        }
        else if(status == Status.Paused)
        {
            status = Status.Running;
            statusBox.text = "Running";
            statusBox.color = CustomColors.Black;
            testBackgrounds[testIndexes[currentIndex]].color = CustomColors.Background.Orange;
            pauseButton.GetComponent<Text>().text = "Pause";
            stopButton.interactable = false;
        }
    }

    /// <summary>
    /// Stops the experiment. Can only stop while paused.
    /// </summary>
    public void Stop()
    {
        if (status != Status.Paused) // just in case
            return;

        status = Status.Stopped;
    }
}
