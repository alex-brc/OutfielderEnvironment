using System;
using UnityEngine;

/// <summary>
/// When attached to a game object, during robot trials this rigidbody will move according to strict OAC strategy.
/// Also needs to be linked inside a start button to be activated.
/// Due to limitations in Unity, this single script offers multiple functionalities.
/// </summary>
[Obsolete]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(RigidbodyCollector))]
public class GOACRobot : MonoBehaviour
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
        Vector3 p = GetDeltaPoint();
        radiusDisplay.radius = GetAlphaRadius();
        lineDisplay.pointA = baseballRb.position.XZ();
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

    public void RobotButton()
    {
        if (manager.loadedTestCase == null)
            return;

        // Compute K and H, assuming ball is always launching from (0,yb0,0) and catcher is stationary (and grounded) at first
        float xzVelocity = manager.loadedTestCase.initialVelocityVector.XZ().magnitude;
        deltaP0 = (manager.baseballHomePosition - manager.playerStartPosition).magnitude;

        K = manager.loadedTestCase.initialVelocityVector.y / manager.baseballHomePosition.y // divided by yb0 = 1
            -
            xzVelocity / deltaP0;

        H = manager.loadedTestCase.initialVelocityVector.z / manager.playerStartPosition.x;
        
        startingTime = Time.time + manager.pauseBetweenTrials.Get();
        StartCoroutine(manager.StartTrial(TestCase.TrialType.Robot));
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
    }

    private double H;
    /// <summary>
    /// Returns the correct catcher position as predicted by the GOAC model.
    /// </summary>
    private Vector3 GetDeltaPoint()
    {
        // The resulting numbers are relative to the ball position. 
        // We will add this to the ball position to get the catcher position.
        Vector3 ballToPoint = new Vector3();
        // First, find delta x
        double t = Time.time - startingTime;
        ballToPoint.x = GetAlphaRadius()
                       / (float)(H * H * t * t + 1);
        // Now find delta z
        ballToPoint.z = ballToPoint.x * (float) (H * t);

        // Finally find and return the catcher position
        return baseballRb.position.XZ()
               + ballToPoint;
    }

    public void SendHome()
    {
        catcherRb.velocity = Vector3.zero;
        catcherRb.angularVelocity = Vector3.zero;
        catcherRb.position = homePosition;
    }

    public Rigidbody GetRigidbody()
    {
        return catcherRb;
    }

    public void StartDataCollector()
    {
        gameObject.GetComponent<RigidbodyCollector>().enabled = true;
    }

    public void StopDataCollector()
    {

        gameObject.GetComponent<RigidbodyCollector>().enabled = false;
    }

    private double deltaP0, K;
    /// <summary>
    /// Returns the radius of the circle which contains the points from which 
    /// we can achieve the correct alpha angles according to the OAC strategy. 
    /// This assumes the ball always launches from (0,yb0,0)
    /// </summary>
    /// <returns>A float representing the radius</returns>
    private float GetAlphaRadius()
    {
        return (float)deltaP0 * baseballRb.position.y
               / ( manager.baseballHomePosition.y * (1 + (float)K * (Time.time - startingTime)));
    }

    private Vector3 tempBaseballPos, tempCatcherPos;
    /// <summary>
    /// Computes the position inside the ball's flight plane predicted
    /// by the OAC strategy that provides the correct alpha angle.
    /// </summary>
    /// <returns>A Vector3 representing the position</returns>
    [Obsolete("Functional, but should never be used. Instead, use GetAlphaRadius() to obtain the distance from the ball and apply a direction.")]
    private Vector3 GetAlphaPosition()
    {
        Vector3 result = new Vector3();
        // To simplify the maths, first circular project both the ball and catcher into the XY plane
        tempBaseballPos = RotateIntoXY(baseballRb.position);
        // Catcher is always grounded (i.e. it is always one the XZ plane)
        tempCatcherPos = CircularProjection(catcherRb.position, Vector3.right);

        // Now compute the correct X
        result.x = tempBaseballPos.x - tempBaseballPos.y *
            (manager.baseballHomePosition.x - manager.playerStartPosition.x) /
            (manager.baseballHomePosition.y * (1 + (float)K * (Time.time - startingTime)));

        // And rotate that around the Y axis into the flight plane of the ball
        result = CircularProjection(
            result, 
            baseballRb.position.XZ());

        return result;
    }

    private float d, q;
    private Vector3 ballToPoint;
    /// <summary>
    /// Uses information about the correct alpha radius and d angle to give the correct point 
    /// to reach for using the GOAC strategy.
    /// </summary>
    /// <returns>A vector representing said point</returns>
    [Obsolete("Functional, but hacky. Use GetDeltaPoint() instead for the mathematical approach. ")]
    private Vector3 GetGOACPoint()
    {
        // Get H, the angular velocity from the linear velocity (in degrees), using 
        // the velocity vector of the ball projected onto the z axis. This is adjusted, 
        // since it doesn't really matter what the velocity is, just that it's constant
        float H = angularVelocityModifier *
            360f * manager.loadedTestCase.initialVelocityVector.z
            /
            catcherRb.position.x;

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
        Vector3 ballProjected = baseballRb.position.XZ();
        ballToPoint = Quaternion.Euler(0, -q, 0) * (-1 * ballProjected).normalized * GetAlphaRadius();

        // Add this vector to the ball position to obtain the correct GOAC point
        return ballProjected + ballToPoint;
    }

    /// <summary>
    /// Rotates the given vector around the Y axis to obtain the "circular projection" onto the XY plane
    /// </summary>
    /// <param name="vector">The vector to be rotated</param>
    /// <returns>The circular projection onto the XY plane</returns>
    [Obsolete("This method is only used in deprecated methods.")]
    private Vector3 RotateIntoXY(Vector3 vector)
    {
        // Isolate the vector inside the XZ plane and project it onto the X axis
        Vector3 result = CircularProjection(
            vector.XZ(),
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
    [Obsolete("This method is only used in deprecated methods.")]
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
