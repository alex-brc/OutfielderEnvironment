using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapher : MonoBehaviour
{
    public TrialsManager manager;
    public float maximumTime = 3;
    public float accelerationDisplayMultiplier = 6;
    public int accelerationSamples = 3;

    private LineRenderer deltaGraph;
    private LineRenderer alphaGraph;
    private LineRenderer ddeltaGraph;
    private LineRenderer dalphaGraph;

    private LowPassFilter alphaFilter;
    private LowPassFilter deltaFilter;

    private Rigidbody baseballRb;
    private Rigidbody catcherRb;
    private bool trialRunning;

    public void Start()
    {
        alphaFilter = new LowPassFilter(accelerationSamples);
        deltaFilter = new LowPassFilter(accelerationSamples);

        foreach (Transform child in transform)
        {
            switch (child.name)
            {
                case "DeltaGraph":
                    deltaGraph = child.GetComponent<LineRenderer>();
                    break;
                case "dDeltaGraph":
                    ddeltaGraph = child.GetComponent<LineRenderer>();
                    break;
                case "AlphaGraph":
                    alphaGraph = child.GetComponent<LineRenderer>();
                    break;
                case "dAlphaGraph":
                    dalphaGraph = child.GetComponent<LineRenderer>();
                    break;
                default:
                    break;
            }
        }
    }

    public void FixedUpdate()
    {
        if (!trialRunning && manager.trialStatus == TrialsManager.TrialStatus.TrialInProgress)
        {
            // Initialise for a new trial
            trialRunning = true;
            catcherRb = manager.catcher.GetRigidbody();
            baseballRb = manager.baseball.GetComponent<Rigidbody>();

            // Reset linerenderers
            deltaGraph.positionCount = 1;
            ddeltaGraph.positionCount = 0;
            alphaGraph.positionCount = 1;
            dalphaGraph.positionCount = 0;
        }
        else if(trialRunning && manager.trialStatus != TrialsManager.TrialStatus.TrialInProgress)
        {
            // End trial
            trialRunning = false;
        }

        if (trialRunning)
        {
            // Add new delta angle
            float currentT = (Time.time - manager.startingTime) * 150 / maximumTime - 75;
            float currentDelta = Vector3.Angle(Vector3.right,
                                          catcherRb.position.XZ());
            deltaGraph.positionCount++;
            deltaGraph.SetPosition(
                deltaGraph.positionCount - 1,
                new Vector3(currentT,
                            currentDelta));
            // Add new delta acceleration
            Vector3 previousDelta = deltaGraph.GetPosition(deltaGraph.positionCount - 2);
            float deltaT = currentT - previousDelta.x;
            float deltaDelta = currentDelta - previousDelta.y;
            ddeltaGraph.positionCount++;
            ddeltaGraph.SetPosition(
                ddeltaGraph.positionCount - 1,
                new Vector3(currentT,
                            deltaFilter.Filter(deltaDelta / deltaT) * accelerationDisplayMultiplier));

            // Add new alpha angle
            float currentAlpha = Vector3.Angle(baseballRb.position.XZ() - catcherRb.position,
                                               baseballRb.position - catcherRb.position);
            alphaGraph.positionCount++;
            alphaGraph.SetPosition(
                alphaGraph.positionCount - 1,
                new Vector3(currentT,
                            currentAlpha));

            // Add new delta alpha
            Vector3 previousAlpha = alphaGraph.GetPosition(alphaGraph.positionCount - 2);
            float deltaAlpha = currentAlpha - previousAlpha.y;
            dalphaGraph.positionCount++;
            dalphaGraph.SetPosition(
                dalphaGraph.positionCount - 1,
                new Vector3(currentT,
                            alphaFilter.Filter(deltaAlpha / deltaT) * accelerationDisplayMultiplier));
        }
    }
}

public class LowPassFilter{

    float[] samples;

    public LowPassFilter(int numSamples)
    {
        samples = new float[numSamples];
    }

    public float Filter(float newSample)
    {
        float sum = newSample;
        for (int i = 0; i < samples.Length - 1; i++)
        {
            sum = sum + samples[i];
            samples[i] = samples[i + 1];
        }
        samples[samples.Length - 1] = newSample;

        return sum / samples.Length;
    }
}
