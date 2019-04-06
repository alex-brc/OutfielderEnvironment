using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTargetDisplay : MonoBehaviour
{
    public TrialManager manager;

    private MeshRenderer mesh;
    private bool trialRunning;

    public void Start()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    public void Update()
    {
        if (!trialRunning && manager.trialStatus == TrialManager.TrialStatus.TrialInProgress)
        {
            // New trial started
            transform.position = new Vector3(manager.loadedTestCase.target.x, 0.1f, manager.loadedTestCase.target.z);
        }
        else if(trialRunning && manager.trialStatus != TrialManager.TrialStatus.TrialInProgress)
        {
            // Trial ended
            trialRunning = false;
        }
    }

    public void ToggleOnValueChanged(bool isOn)
    {
        if (isOn)
            mesh.enabled = true;
        else
            mesh.enabled = false;
    }
}
