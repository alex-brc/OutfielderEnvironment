using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When attached to a game object, during robot trials this rigidbody will move according to strict OAC strategy.
/// Also needs to be linked inside a start button to be activated.
/// Due to limitations in Unity, this single script offers multiple functionalities.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class OAC : MonoBehaviour, ICatcher
{
    [Tooltip("Whether to apply movement contraints to the catcher.")]
    public bool limited;
    [Tooltip("How many fixed updates to skip at the begginning of the trial.")]
    [Min(0f)]
    public int skippedUpdates = 5;
    private float timeToSkip;

    [Header("Movement parameters")]
    [Min(0f)]
    public float maxVelocity = 7;
    [Tooltip("The minimum distance between the catcher and the correct position from which the catcher uses maximum velocity.")]
    [Min(0f)]
    public float maxVelocityDistance = 10;
    [Tooltip("Velocity function growth rate for use in the MoveTowards() function.")]
    public float growthRateModifier = 3f;
    [Tooltip("Linear modifier for the angular velocity of the ball w.r.t. the fielder (i.e. the speed of the catcher's rotation).")]
    [Range(0, 1)]
    public float angularVelocityModifier = 0.3f;

    [Header("Positions")]
    [Tooltip("Position where the catcher sits inbetween trials. Should be inside the staging room.")]
    public Vector3 homePosition; // Somewhere inside the staging room

    [Header("References")]
    public GameObject baseball;
    public TrialsManager manager;

    [Header("Debug Tools")]
    public RadiusDisplay radiusDisplay;
    public LineDisplay lineDisplay;
    public PathDisplay pathDisplay;

    private CustomPhysics baseballPh;
    private Rigidbody baseballRb;
    private Rigidbody catcherRb;


    private float sqrTopVelocity;
    private float startingTime;
    private float K; // A constant that depends on the ball's initial conditions.
    private float H; // Angular velocity of the ball relative to the stationary catcher

    // Start is called before the first frame update
    void Start()
    {
        catcherRb = gameObject.GetComponent<Rigidbody>();
        baseballRb = baseball.GetComponent<Rigidbody>();
        baseballPh = baseball.GetComponent<CustomPhysics>();
        sqrTopVelocity = maxVelocity * maxVelocity;
        timeToSkip = (skippedUpdates + 1) * Time.fixedDeltaTime;
    }

    public void Move()
    {
        // debug
        Vector3 p = GetGOACPoint();
        radiusDisplay.radius = GetAlphaRadius();
        lineDisplay.pointA = new Vector3(baseballRb.position.x, 0, baseballRb.position.z);
        lineDisplay.pointB = p;

        // Don't move if we need to skip updates
        if (Time.time - startingTime < timeToSkip)
        {
            return;
        }
        if (limited)
        {
            MoveTowards(p); 
        }
        else
        {
            catcherRb.position = p;
        }
    }

    public void SendHome()
    {
        catcherRb.velocity = Vector3.zero;
        catcherRb.position = homePosition;
    }

    public Rigidbody GetRigidbody()
    {
        return catcherRb;
    }

    public void RobotButton()
    {
        if (manager.loadedTestCase == null)
            return;

        // Compute K
        // For adapting into the planar version of OAC, we replace xbc' with the launch speed *within the plane of flight*
        float temp = new Vector3(manager.loadedTestCase.initialVelocityVector.x,
                                 0,
                                 manager.loadedTestCase.initialVelocityVector.z)
                     .magnitude; 
        // Assuming ball is always launching from (0,1,0) and catcher is stationary at first
        K = manager.loadedTestCase.initialVelocityVector.y + temp / manager.catcherStartPosition.x;

        // Get H, the angular velocity from the linear velocity (in degrees), using 
        // the velocity vector of the ball projected onto the z axis. This is adjusted, 
        // since it doesn't really matter what the velocity is, just that it's constant
        H = angularVelocityModifier * 
            360f * new Vector3(
                0,
                0, 
                manager.loadedTestCase.initialVelocityVector.z)
            .magnitude              
            /
            catcherRb.position.x;

        // Start
        manager.catcher = this;
        startingTime = Time.time + manager.secondsCountdownBeforeStart;
        StartCoroutine(manager.StartTrial(TestCase.Type.Robot));
    }

    /// <summary>
    /// Moves the catcher towards the target by applying a velocity to it.
    /// </summary>
    /// <param name="target">Target to move towards</param>
    public void MoveTowards(Vector3 target)
    {
        float velocity = maxVelocity;
        float distance = Vector3.Distance(catcherRb.position, target);
        if (distance <= maxVelocityDistance)
            velocity = maxVelocity * Mathf.Pow(distance / maxVelocityDistance, growthRateModifier);

        // Move the catcher
        catcherRb.velocity = (target - catcherRb.position).normalized * velocity;

        // Also rotate it
        catcherRb.transform.LookAt(new Vector3(baseballRb.transform.position.x, 0, baseballRb.transform.position.z));
    }

    private float d, q;
    private Vector3 ballToPoint;
    /// <summary>
    /// Uses information about the correct alpha radius and d angle to give the correct point 
    /// to reach for using the GOAC strategy.
    /// </summary>
    /// <returns>A vector representing said point</returns>
    private Vector3 GetGOACPoint()
    {
        // First get the correct angle d in degrees at current time, given by H * t, since ideally it is linear
        d = H * (Time.time - startingTime) * -Mathf.Sign(manager.loadedTestCase.launchDeviation);
        Debug.Log(d);
        // This gives us the correct d line of equation:  z = tanD * (x - xb) + xb
        // This line makes an angle q with the ball's plane of flight, which is
        q = 180 - (d + manager.loadedTestCase.launchDeviation);
        
        // Also, we have the correct alpha circle with:   (x - xb)^2 + (z - zb)^2 = r^2
        //
        // Therefore our destination is at the intersection of the d line with alpha circle, which we can find by using
        // some vector manipulation to avoid the complicated linear algebra computation. 
        // 
        // We take the vector from the ball to the origin, rotate it q degrees, then set its magnitude equal to the radius
        Vector3 ballProjected = new Vector3(baseballRb.position.x, 0, baseballRb.position.z);
        ballToPoint = Quaternion.Euler(0, -q, 0) * (-1 * ballProjected).normalized * GetAlphaRadius();

        // Add this vector to the ball position to obtain the correct GOAC point
        return ballProjected + ballToPoint;
    }

    /// <summary>
    /// Returns the radius of the circle which contains the points from which 
    /// we can achieve correct alpha angles according to the OAC strategy. 
    /// </summary>
    /// <returns>A float representing the radius</returns>
    private float GetAlphaRadius()
    {
        return (GetAlphaPosition() -
                new Vector3(baseballRb.position.x, 0, baseballRb.position.z)
                ).magnitude;
    }
    
    private Vector3 tempBaseballPos, tempCatcherPos;
    /// <summary>
    /// Computes the position inside the ball's flight plane predicted
    /// by the OAC strategy that provides the correct alpha angle.
    /// </summary>
    private Vector3 GetAlphaPosition()
    {
        Vector3 result = new Vector3();
        // To simplify the maths, first circular project both the ball and catcher into the XY plane
        tempBaseballPos = RotateIntoXY(baseballRb.position);
        // Catcher is always grounded (i.e. it is always one the XZ plane)
        tempCatcherPos = CircularProjection(catcherRb.position, Vector3.right);

        // Now compute the correct X
        result.x = tempBaseballPos.x - tempBaseballPos.y *
            (manager.baseballHomePosition.x - manager.catcherStartPosition.x) /
            (manager.baseballHomePosition.y * (1 + K * (Time.time - startingTime)));

        // And rotate that around the Y axis into the flight plane of the ball
        result = CircularProjection(
            result, 
            new Vector3(baseballRb.position.x,
                        0,
                        baseballRb.position.z));

        return result;
    }
    
    /// <summary>
    /// Rotates the given vector around the Y axis to obtain the "circular projection" onto the XY plane
    /// </summary>
    /// <param name="vector">The vector to be rotated</param>
    /// <returns>The circular projection onto the XY plane</returns>
    private Vector3 RotateIntoXY(Vector3 vector)
    {
        // Isolate the vector inside the XZ plane and project it onto the X axis
        Vector3 result = CircularProjection(
            new Vector3(vector.x, 0, vector.z),
            Vector3.right);
        // Complete it with the original Y coordinate
        result.y = vector.y;

        return result;
    }

    /// <summary>
    /// "Projects" the first point onto the line (O,Point2) via a circular path. 
    /// Otherwise stated, gives the vector with the magnitude of Point1 in the direction of the vector Point2.
    /// </summary>
    /// <param name="Point1">The point to be projected</param>
    /// <param name="Point2">The point which gives to line to project upon</param>
    /// <returns>The circular projection of Point1 onto the line (O,Point2)</returns>
    private Vector3 CircularProjection(Vector3 Point1, Vector3 Point2)
    {
        return Point2.normalized * Point1.magnitude;
    }
    
    float relativePosition, relativeVelocity;
    /// <summary>
    /// Computes the X acceleration required to reach the interception point.
    /// Unlike GetStrictX(), this does not use knowledge of initial conditions,
    /// but in exchange is more costly.
    /// </summary>
    /// <returns>The X acceleration to be applied to the actor</returns>
    [Obsolete("The acceleration movement mode is not used anymore. A position dictated movement is used instead.")]
    private float GetAccelerationX()
    {
        relativePosition = baseballRb.position.x - catcherRb.position.x;
        relativeVelocity = baseballRb.velocity.x - catcherRb.velocity.x;

        return baseballPh.GetAcceleration().x
               - (relativePosition / baseballRb.position.y) * baseballPh.GetAcceleration().y
               - 2 * relativeVelocity * ((relativeVelocity / relativePosition)
                                        - (baseballRb.velocity.y / baseballRb.position.y));
    }
}
