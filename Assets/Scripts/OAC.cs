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

    [Header("Limitations")]
    [Min(0f)]
    public float topVelocity = 7;
    [Min(0f)]
    public float topAcceleration = 10;

    [Header("Positions")]
    [Tooltip("Should be inside the staging room")]
    public Vector3 homePosition; // Somewhere inside the staging room

    [Header("References")]
    public GameObject baseball;
    public TrialsManager manager;
    
    private CustomPhysics baseballPh;
    private Rigidbody baseballRb;
    private Rigidbody catcherRb;

    private float sqrTopVelocity;
    private float startingTime;
    private float K; // A constant that depends on the ball's initial conditions.
    private float H; // Angular velocity of the d angle

    // Start is called before the first frame update
    void Start()
    {
        catcherRb = gameObject.GetComponent<Rigidbody>();
        baseballRb = baseball.GetComponent<Rigidbody>();
        baseballPh = baseball.GetComponent<CustomPhysics>();
        sqrTopVelocity = topVelocity * topVelocity;
        timeToSkip = (skippedUpdates + 1) * Time.fixedDeltaTime;
    }

    public void Move()
    {
        // debug
        baseball.GetComponent<RadiusDisplay>().radius = GetAlphaRadius();

        // Don't move if we need to skip updates
        if (Time.time - startingTime < timeToSkip)
        {
            return;
        }
        if (limited)
        {
            // For now the restricted motion is not implemented
            throw new NotImplementedException();
        }
        else 
            catcherRb.position = GetAlphaPosition();
            
    }

    public void SendHome()
    {
        catcherRb.position = homePosition;
        catcherRb.velocity = Vector3.zero;
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

        // Compute H
        temp = (manager.loadedTestCase.initialVelocityVector.x - manager.catcherStartPosition.x);
        H = Mathf.Acos(
            manager.catcherStartPosition.x * (-1*temp)
            / 
            (manager.catcherStartPosition.x + Mathf.Sqrt(
                         temp*temp 
                         + 
                         manager.loadedTestCase.initialVelocityVector.z * manager.loadedTestCase.initialVelocityVector.z
                         )
            ));

        startingTime = Time.time + manager.secondsCountdownBeforeStart;

        // Start
        manager.catcher = this;
        StartCoroutine(manager.StartTrial(TestCase.Type.Robot));
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    private float GetGOACPoint()
    {
        // First get tan(d) at current time, given by H * t, since ideally it is linear
        float tanD = Mathf.Tan( H * (Time.time - startingTime));
        float r = GetAlphaRadius();
        // This gives us the correct d line of equation:  z = tanD * (x - xc) + zc
        // Also, we have the correct alpha circle with:   (x - xb)^2 + (z - zb)^2 = r^2
        // This means our destination is at:



        // We must find the point on the d line at radius distance from the ball (projection)

        throw new System.NotImplementedException();
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
    
    private Vector3 tempBaseballPos;
    private Vector3 tempCatcherPos;
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
    
    float relativePosition;
    float relativeVelocity;
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
