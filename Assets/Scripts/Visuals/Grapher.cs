using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapher : MonoBehaviour
{
    private const float GRAPH_HEIGHT = 100, GRAPH_WIDTH = 150;

    public TrialManager manager;

    private float maximumTime;
    private float maxAlpha;
    private float maxDelta;

    private LineRenderer deltaGraph;
    private LineRenderer alphaGraph;

    private Rigidbody ballRb;
    private Rigidbody catcherRb;
    private bool trialRunning;

    public void Start()
    {
        foreach (Transform child in transform)
        {
            switch (child.name)
            {
                case "DeltaGraph":
                    deltaGraph = child.Find("Line").GetComponent<LineRenderer>();
                    break;
                case "AlphaGraph":
                    alphaGraph = child.Find("Line").GetComponent<LineRenderer>();
                    break;
                default:
                    break;
            }
        }
    }

    public void FixedUpdate()
    {
        if (!trialRunning && manager.trialStatus == TrialManager.TrialStatus.TrialInProgress)
        {
            // Initialise for a new trial
            trialRunning = true;
            catcherRb = manager.player.GetRigidbody();
            ballRb = manager.ballRb.GetComponent<Rigidbody>();
            maximumTime = manager.loadedTestCase.duration;

            // Reset linerenderers
            deltaGraph.positionCount = 0;
            alphaGraph.positionCount = 0;

            // Reset maxvalues
            maxAlpha = 50;
            maxDelta = 50;
        }
        else if(trialRunning && manager.trialStatus != TrialManager.TrialStatus.TrialInProgress)
        {
            // End trial
            trialRunning = false;
        }

        if (trialRunning)
        {
            // Add new delta angle
            float currentT = Time.time - manager.startingTime;
            Vector3 catcherToBall = ballRb.position.XZ() - catcherRb.position.XZ();
            float currentDelta = Vector3.Angle(-Vector3.right, catcherToBall);
            AddToLine(currentDelta, currentT, ref maxDelta, deltaGraph);

            // Add new alpha angle
            float currentAlpha = Vector3.Angle(ballRb.position.XZ() - catcherRb.position,
                                               ballRb.position - catcherRb.position);
            AddToLine(currentAlpha, currentT, ref maxAlpha, alphaGraph);
        }
    }
    
    private void AddToLine(float value, float time, ref float maxValue, LineRenderer line)
    {
        if(Mathf.Abs(value) > maxValue)
        {
            float oldMax = maxValue;
            do
            {
                // Increase max value by 20% so we don't do this every frame
                maxValue = maxValue + maxValue * 0.2f;
            }
            while (Mathf.Abs(value) > maxValue);
            
            // Rescale all the heights in the line
            for (int i = 0; i < line.positionCount; i++)
            {
                Vector3 currentPos = line.GetPosition(i);
                // Rescale
                currentPos.y = currentPos.y * oldMax / maxValue;
                line.SetPosition(i, currentPos);
            }
        }

        float ajustedTime = time * GRAPH_WIDTH / maximumTime;
        float adjustedValue = value * (GRAPH_HEIGHT/2) / maxValue;
       
        Vector3 positionToAdd = new Vector3(ajustedTime, adjustedValue);

        line.positionCount++;
        line.SetPosition(line.positionCount - 1, positionToAdd);
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
