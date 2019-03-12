using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This class implements the General Optical Acceleration Cancellation strategy.
/// </summary>
[DisallowMultipleComponent]
public class GOAC : MonoBehaviour, IStrategy
{
    public float initialTime = 0.5f;

    private Vector3 initialBallPosition;
    private float K, H, deltaP0;
    private bool ready = false;

    private void Initialise(Vector3 initialBallVelocity, Vector3 initialBallPosition, Vector3 initialCatcherPos)
    {
        this.initialBallPosition = initialBallPosition;
        // Delta P0 is the initial distance between the catcher and
        // the ball
        deltaP0 = (initialBallPosition - initialCatcherPos).magnitude;

        // K is a constant derived from the initial conditions
        // of the ball. This is used in finding the alpha-radius.
        K = initialBallVelocity.y / initialBallPosition.y
            -
            initialBallVelocity.XZ().magnitude / deltaP0;

        // H is K's equivalent for finding the delta-line
        // In fact, due to the simpler nature of finding delta, 
        // H can actually be interpreted as the the angular 
        // velocity of the ball, relative to the catcher.
        H = initialBallVelocity.z / initialCatcherPos.x;

        // Note that K and H are the equivalent of the T from 
        // LOT, however, since in GOAC alpha and delta are controlled
        // separately, they have different values.
    }

    private void UpdatePrediction(float t, Vector3 ballPosition)
    {
        // For a detailed explanation, reference the dissertation

        // The resulting numbers are relative to the ball position. 
        // We will add this to the ball position to get the catcher position.
        Vector3 ballToPoint = new Vector3();

        // The correct elevation angle can be achieved from any point 
        // that lies on a circle of a specific radius around the ball.
        // This radius is:
        float r = deltaP0 * ballPosition.y
               / (initialBallPosition.y * (1 + K * t));

        // The correct delta angle, in contrast, can be achieved from
        // any point that lies on a certain line (on which the ball also
        // lies). The vector along that line, from the ball to the correct
        // point is ballToPoint.
        float Ht = H * t;
        ballToPoint.x = r / Mathf.Sqrt(Ht * Ht + 1);
        ballToPoint.z = ballToPoint.x * Ht;

        // Add that to the ball position to obtain the correct point
        transform.position = ballPosition.XZ() + ballToPoint;
    }

    public void Initialise(Rigidbody ballRb, Rigidbody catcherRb)
    {
        Initialise(ballRb.velocity, ballRb.position, catcherRb.position);
        ready = true;
    }

    public void UpdatePrediction(float t, Rigidbody ballRb)
    {
        if (!ready)
            return;
        UpdatePrediction(t - initialTime, ballRb.position);
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
        return initialTime;
    }
}
