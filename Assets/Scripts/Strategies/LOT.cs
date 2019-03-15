using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This class implements the Linear Optical Trajectory strategy.
/// </summary>
[DisallowMultipleComponent]
public class LOT : MonoBehaviour, IStrategy
{
    public float initialTime = 0.5f;
    
    private float a,d,T,deltaPf;
    private bool ready = false;

    private void Initialise(Vector3 initialBallVelocity, Vector3 initialBallPosition, Vector3 initialCatcherPosition)
    {
        // a is the tangent of alpha at the time of fixation
        Vector3 PosCB = initialCatcherPosition - initialBallPosition.XZ();
        deltaPf = PosCB.magnitude;
        a = initialBallPosition.y / PosCB.magnitude;

        // d is the tangent of delta at the time of fixation
        // This might be negative of positive, depending on whether
        // the ball went to the right or left of the catcher.
        d = PosCB.z / PosCB.x;

        // T is a constant depending on the velocity and position
        // of the ball and catcher at the moment of fixation.
        // Assume the catcher is stationary at the time of fixation
        T = (initialBallVelocity.y * deltaPf + initialBallVelocity.x * initialBallPosition.y)
            / (deltaPf * deltaPf);

        // Note that this T is the equivalent of the K and H of GOAC,
        // however since in LOT alpha and delta are controlled 
        // simultaneously, there is just one constant for each of them.
    }

    private void UpdatePrediction(float t, Vector3 ballPosition)
    {
        // For a detailed explanation, reference the dissertation

        // The resulting numbers are relative to the ball position. 
        // We will add this to the ball position to get the catcher position.
        Vector3 ballToPoint = new Vector3();
        
        // The correct alpha angle can be achieved from any point 
        // that lies on a circle of a specific radius around the ball.
        // This radius is:
        float r = ballPosition.y / (a + T * t);

        // The correct delta angle, in contrast, can be achieved from
        // any point that lies on a certain line (on which the ball also
        // lies). The vector along that line, from the ball to the correct
        // point is ballToPoint. Adjust T with a sign for when the ball's 
        // initial Z velocity < 0
        float dTt = (d + d * T * t);
        ballToPoint.x = r / Mathf.Sqrt(dTt * dTt + 1);
        ballToPoint.z = ballToPoint.x * dTt;

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
