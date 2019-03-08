using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(TrialsManager))]
public class AutoTrialRunner : MonoBehaviour
{
    private enum Status { Stopped, Paused, Running }

    public PlayerController player;

    private Status status;
    private Coroutine runner;
    private TrialsManager manager;
    private int[] testIndexes;
    private System.Random rand;

    private void Start()
    {
        status = Status.Stopped;
        manager = GetComponent<TrialsManager>();
        rand = new System.Random();
    }

    /// <summary>
    /// Coroutine that handles loading and unloading, as 
    /// well as starting and stopping them.
    /// </summary>
    private IEnumerator Runner(int[] testIndexes)
    {
        int currentIndex = 0;
        while(status == Status.Running || status == Status.Paused)
        {
            if (status == Status.Paused)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }
            // Load the test with number index[currentIndex]
            manager.LoadTest(manager.testCases[testIndexes[currentIndex]]);

            // Set null position for controller
            player.SetZeroPosition();

            // Start the trial
            manager.StartTrial(TestCase.TrialType.Trial);

            // Wait for the trial to finish
            while (manager.trialStatus != TrialsManager.TrialStatus.Ready)
                yield return new WaitForSeconds(0.1f);
        }

        yield return 0;
    }

    /// <summary>
    /// Runs through the trials in a random order, until 
    /// each of them has been performed the required number
    /// of times required.
    /// </summary>
    public void StartExperiment()
    {
        testIndexes = new int[manager.testCases.Length * manager.trialRuns];
        // Fill the testIndexes with timesToRunEachTrial copies of each test case index
        for (int i = 0; i < manager.testCases.Length; i++)
            for (int j = 0; j < manager.trialRuns; j++)
                testIndexes[i * manager.trialRuns + j] = i;

        // Shuffle them
        int t,r = 0;
        for (int i = testIndexes.Length - 1; i >= 0; i--) {
            r = rand.Next(0,i);
            t = testIndexes[i];
            testIndexes[i] = testIndexes[r];
            testIndexes[r] = t;
        }

        // Set the catcher
        manager.catcher = player;

        // Start the runner
        runner = StartCoroutine(Runner(testIndexes));

        // Finally, update the status
        status = Status.Running;
    }

    /// <summary>
    /// Runs through each trial for practice the required 
    /// number of times.
    /// </summary>
    public void StartPractice()
    {

        status = Status.Running;
    }

    /// <summary>
    /// Pauses running through trials. Lets the running trial 
    /// finish.
    /// </summary>
    public void Pause()
    {
        status = Status.Paused;
    }

    /// <summary>
    /// Stops the experiment. Can only stop while paused.
    /// </summary>
    public void Stop()
    {
        if (status != Status.Paused)
            return;

        status = Status.Stopped;
    }
}
