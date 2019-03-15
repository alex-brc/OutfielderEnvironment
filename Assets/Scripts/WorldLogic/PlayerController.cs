using UnityEngine;

[RequireComponent(typeof(RigidbodyCollector))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public enum Controller { Joystick, Fove }

    public Controller controllerType;
    public Configurable<float> maximumSpeed = new Configurable<float>();
    public Vector3 homePosition;
    public PathDisplay path;

    [Header("References")]
    public TrialsManager manager;
    public GameObject fove;

    internal Vector3 motionVector;
    internal Vector3 velocityVector;

    private new Rigidbody rigidbody;
    internal bool calibrating = false;
    internal bool calibrated = false;
    private Vector3 minimumLean, maximumLean, zeroPosition;

    void Start() {
    	rigidbody = GetComponent<Rigidbody>();
        minimumLean.x = minimumLean.z = Mathf.Infinity;
        maximumLean.x = maximumLean.z = Mathf.NegativeInfinity;
    }
    
    public void IndividualTrialStart()
    {
        StartTrial(TestCase.TrialType.Trial);
    }

    public void IndividualPracticeStart()
    {
        StartTrial(TestCase.TrialType.Practice);
    }

    private void StartTrial(TestCase.TrialType type)
    {
        if (!calibrating)
        {
            SetZeroPosition();
            manager.player = this;
            StartCoroutine(manager.StartTrial(type));
        }
    }

    void FixedUpdate()
    {
        if (calibrating)
        {
            Vector3 currentLean = GetFOVEInput() - zeroPosition;
            if (currentLean.x < minimumLean.x)
                minimumLean.x = currentLean.x;
            if (currentLean.z < minimumLean.z)
                minimumLean.z = currentLean.z;

            if (currentLean.x > maximumLean.x)
                maximumLean.x = currentLean.x;
            if (currentLean.z > maximumLean.z)
                maximumLean.z = currentLean.z;
        }
    }

    public void Move()
    {
        float modifier = 1;
        if (controllerType == Controller.Fove)
        {
            // Raw motion vector
            motionVector = GetFOVEInput() - zeroPosition;

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

            // Linear map
            // rigidbody.velocity = modifier * velocityVector * maximumSpeed.Get();
        }
        else if(controllerType == Controller.Joystick)
        {
            // This is already filtered by the app
            velocityVector = GetWiiBoardInput();
            
            // Put this through a sinusoid for smoother values (currently not needed)
            modifier = velocityVector.magnitude * Mathf.PI;
            modifier = (1 - Mathf.Cos(modifier)) / 2;

            // Apply force
            // rigidbody.AddForce(modifier * velocityVector * maximumSpeed.Get() / 2, ForceMode.Acceleration);

            // Linear map
            // rigidbody.velocity = modifier * velocityVector * maximumSpeed.Get();
        }
        // Add current position to path
        path.UpdateLine(transform.position);
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
        gameObject.GetComponent<RigidbodyCollector>().enabled = true;
    }

    public void StopDataCollector()
    {

        gameObject.GetComponent<RigidbodyCollector>().enabled = false;
    }

    internal void ClearCalibration()
    {
        zeroPosition = Vector3.zero;
        minimumLean.x = minimumLean.z = Mathf.Infinity;
        maximumLean.x = maximumLean.z = Mathf.NegativeInfinity;
    }

    internal void SetZeroPosition()
    {
        zeroPosition = GetFOVEInput();
    }

    private Vector3 GetFOVEInput()
    {
        return fove.transform.localPosition;
    }

    private Vector3 GetWiiBoardInput()
    {
        Vector3 input = new Vector3()
        {
            x = -Input.GetAxis("Vertical"),
            z = Input.GetAxis("Horizontal")
        };
        Debug.Log("joystick x: " + input.x + " y: " + input.z);
        return input;
    }
}