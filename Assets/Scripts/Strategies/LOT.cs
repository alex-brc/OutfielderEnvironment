using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This class implements the Linear Optical Trajectory strategy.
/// </summary>
[DisallowMultipleComponent]
public class LOT : MonoBehaviour, IStrategy
{
    public Configurable<float> initialTime = new Configurable<float>(1f);
    public Configurable<float> lotParameter = new Configurable<float>(0.2f);
    
    private float p, q;
    private bool ready = false;

    private void Initialise(Vector3 initialBallVelocity, Vector3 initialBallPosition, Vector3 initialCatcherPosition)
    {
        p = (initialBallPosition.x - initialCatcherPosition.x)
            / (initialCatcherPosition.z - initialBallPosition.z);

        float xpz = initialBallPosition.x + p * initialBallPosition.z;
        q = (initialBallPosition.y / xpz) * (initialCatcherPosition.x * (p * p + 1) - xpz) / (initialCatcherPosition.x - initialBallPosition.x);

        Debug.Log("p:" + p + " q:" + q);
    }

    private Vector3 UpdatePrediction(float t, Vector3 ballPosition)
    {
        float xpz = ballPosition.x + p * ballPosition.z;
        float part = xpz / (ballPosition.y * (p * p + 1) - q * xpz);

        Vector3 result = new Vector3()
        {
            x = (ballPosition.y - q * ballPosition.x) * part,
            z = (p * ballPosition.y - q * ballPosition.z) * part
        };

        Debug.Log("ball: " + ballPosition);
        Debug.Log("prediction: " + result);

        return result;
    }

    // Since the maths here gets fuzzy around 0, we have to shift everything momentarily
    public void Initialise(Rigidbody ballRb, Rigidbody catcherRb)
    {
        Initialise(Shift(ballRb.velocity), Shift(ballRb.position), Shift(catcherRb.position));
        // Initialise(ballRb.velocity, ballRb.position, catcherRb.position);
        ready = true;
    }

    public void UpdatePrediction(float t, Rigidbody ballRb)
    {
        if (!ready)
            return;
        transform.position = ShiftBack(UpdatePrediction(t - initialTime.Get(), Shift(ballRb.position)));
        
        // transform.position = UpdatePrediction(t - initialTime.Get(), ballRb.position);
    }
    
    public Vector3 GetPrediction()
    {
        return transform.position;
    }

    public bool IsReady()
    {
        return ready;
    }

    public void Terminate()
    {
        ready = false;
    }

    public float TimeToInit()
    {
        return initialTime.Get();
    }

    private Vector3 Shift(Vector3 input)
    {
        return Quaternion.Euler(0, 30, 0) * input;
    }

    private Vector3 ShiftBack(Vector3 input)
    {
        return Quaternion.Euler(0, -30, 0) * input;
    }
}
