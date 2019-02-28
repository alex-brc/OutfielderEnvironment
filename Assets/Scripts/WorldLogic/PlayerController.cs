using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(DataCollector))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, ICatcher
{
    public float maximumSpeed = 7;
    public Vector3 homePosition;
    
    [Header("References")]
    public TrialsManager manager;
    public GameObject fove;

    internal Vector3 motionVector;
    internal Vector3 velocityVector;
    internal bool calibrating = false;
    internal bool calibrated = false;

    private Rigidbody rigidbody;
    private Vector3 minimumLean, maximumLean, zeroPosition;

    void Start() {
    	rigidbody = GetComponent<Rigidbody>();
        minimumLean.x = minimumLean.z = Mathf.Infinity;
        maximumLean.x = maximumLean.z = Mathf.NegativeInfinity;
    }
    
    public void StartButton()
    {
        if (!calibrating)
        {
            SetZeroPosition();
            manager.catcher = this;
            StartCoroutine(manager.StartTrial(TestCase.Type.Trial));
        }
    }

    public void StartPracticeButton()
    {
        if (!calibrating)
        {
            SetZeroPosition();
            manager.catcher = this;
            StartCoroutine(manager.StartTrial(TestCase.Type.Practice));
        }
    }

    void FixedUpdate()
    {
        if (calibrating)
        {
            Vector3 currentLean = GetControlInput() - zeroPosition;
            if (currentLean.x < minimumLean.x)
                minimumLean.x = currentLean.x;
            if (currentLean.z < minimumLean.z)
                minimumLean.z = currentLean.z;

            if (currentLean.x > maximumLean.x)
                maximumLean.x = currentLean.x;
            if (currentLean.z > maximumLean.z)
                maximumLean.z = currentLean.z;

            Debug.Log("maxlean: " + maximumLean + "  minLean: " + minimumLean);
        }
    }

    public void Move()
    { 
        // Raw motion vector
        motionVector = GetControlInput() - zeroPosition;
        
        // Cutoff exceeding values
        motionVector.x = Mathf.Clamp(motionVector.x, minimumLean.x, maximumLean.x);
        motionVector.y = 0;
        motionVector.z = Mathf.Clamp(motionVector.z, minimumLean.z, maximumLean.z);

        // Ignore y, start preparing actual velocity vector
        velocityVector = motionVector;
        
        if (motionVector.x < 0) // Leaning back
            velocityVector.x = motionVector.x / Mathf.Abs(minimumLean.x);
        else                    // Leaning forward
            velocityVector.x = motionVector.x / Mathf.Abs(maximumLean.x);

        if (motionVector.z < 0) // Leaning left
            velocityVector.z = motionVector.z / Mathf.Abs(minimumLean.z);
        else                    // Leaning right
            velocityVector.z = motionVector.z / Mathf.Abs(maximumLean.z);
        // Magnitude should be in [0..1] for each of x and z
        
        // Correct orientation
        velocityVector = Quaternion.Euler(0, -90, 0) * velocityVector;

        // Put this through a sinusoid for smoother values (currently not needed)
        // float t = velocityVector.magnitude * Mathf.PI;
        // t = (1 - Mathf.Cos(t)) / 2;
        
        rigidbody.velocity = velocityVector * maximumSpeed;
    }

    public void SendHome()
    {
        rigidbody.position = homePosition;
        rigidbody.velocity = Vector3.zero;
        rigidbody.rotation = Quaternion.Euler(0,0,0);
    }

    public Rigidbody GetRigidbody()
    {
        return rigidbody;
    }

    public void StartDataCollector()
    {
        gameObject.GetComponent<DataCollector>().enabled = true;
    }

    public void StopDataCollector()
    {

        gameObject.GetComponent<DataCollector>().enabled = false;
    }

    internal void ClearCalibration()
    {
        zeroPosition = Vector3.zero;
        minimumLean.x = minimumLean.z = Mathf.Infinity;
        maximumLean.x = maximumLean.z = Mathf.NegativeInfinity;
    }

    internal void SetZeroPosition()
    {
        zeroPosition = GetControlInput();
        Debug.Log("set zero at " + zeroPosition);
    }

    private Vector3 GetControlInput()
    {
        return fove.transform.position;
    }
}