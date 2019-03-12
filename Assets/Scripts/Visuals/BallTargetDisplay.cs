using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTargetDisplay : MonoBehaviour
{
    public TrialsManager manager;

    private bool trialRunning;

    public void Update()
    {
        if (!trialRunning && manager.trialStatus == TrialsManager.TrialStatus.TrialInProgress)
        {
            // New trial started
            transform.position = manager.loadedTestCase.target;
        }
        else if(trialRunning && manager.trialStatus != TrialsManager.TrialStatus.TrialInProgress)
        {
            // Trial ended
            trialRunning = false;
        }
    }
}
