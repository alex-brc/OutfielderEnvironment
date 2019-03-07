using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class TestCase : System.Object
{
    public enum TrialType { Practice, Trial, Robot };
    public enum BuildType { Target, InitialVelocity, InitialParameters };
    
    public float launchSpeed; // Meters per second
    public float launchAngle; // Degrees from plane
    public float launchDeviation; // Degrees deviation from player
    public string floatFormat = "0.##";

    internal BuildType buildType;
    internal int testNumber;
    internal GameObject testCaseObject;
    internal Button loadButton;
    internal Button unloadButton;
    internal Text testCounterBox;
    internal Text testStatus;
    internal Vector3 target;
    internal Vector3 initialVelocityVector;

    private TrialsManager manager;
    /// <summary>
    /// Create a test case whose ball will land at the specified target,
    /// achieving the specified maximum ball height.
    /// </summary>
    /// <param name="target">The target point for the ball to land at</param>
    public TestCase(Vector3 target, float height)
    {
        Vector3 velocityVector = new Vector3();
        velocityVector.y = Mathf.Sqrt(-2 * (height-1) * Physics.gravity.y);
        float t =  (velocityVector.y + Mathf.Sqrt(velocityVector.y * velocityVector.y + 2 * Physics.gravity.y))/ (-1 * Physics.gravity.y);
        velocityVector.x = target.x / t;
        velocityVector.z = target.z / t;

        initialVelocityVector = velocityVector;

        launchSpeed = velocityVector.magnitude;
        launchDeviation = Mathf.Sign(velocityVector.z) * Vector3.Angle(Vector3.right, velocityVector.XZ());
        launchAngle = Vector3.Angle(Vector3.right, velocityVector.XY());
        
        this.target = target;

        buildType = BuildType.Target;
    }

    /// <summary>
    /// Create a test case with the initial velocity.
    /// </summary>
    /// <param name="initialVelocityVector">The initial velocity vector of the ball</param>
    public TestCase(Vector3 initialVelocityVector)
    {
        this.initialVelocityVector = initialVelocityVector;

        launchSpeed = initialVelocityVector.magnitude;
        launchDeviation = Mathf.Sign(initialVelocityVector.z) * Vector3.Angle(Vector3.right, initialVelocityVector.XZ());
        launchAngle = Vector3.Angle(Vector3.right, initialVelocityVector.XY());

        // Compute target
        target = new Vector3();
        float t = (initialVelocityVector.y + Mathf.Sqrt(initialVelocityVector.y * initialVelocityVector.y + 2 * Physics.gravity.y)) / (-1 * Physics.gravity.y);
        target.x = t * initialVelocityVector.x;
        target.z = t * initialVelocityVector.z;

        buildType = BuildType.InitialVelocity;
    }

    /// <summary>
    /// Create a test case with the specified initial launch speed, deviation and angle.
    /// </summary>
    public TestCase(float launchSpeed, float launchDeviation, float launchAngle)
    {
        initialVelocityVector = Quaternion.Euler(0,launchDeviation,launchAngle) * Vector3.right * launchSpeed;
        
        this.launchSpeed = launchSpeed;
        this.launchAngle = launchAngle;
        this.launchDeviation = launchDeviation;

        // Compute target
        Vector3 target = new Vector3();
        float t = (initialVelocityVector.y + Mathf.Sqrt(initialVelocityVector.y * initialVelocityVector.y + 2 * Physics.gravity.y)) / (-1 * Physics.gravity.y);
        target.x = t * initialVelocityVector.x;
        target.z = t * initialVelocityVector.z;

        buildType = BuildType.InitialParameters;
    }

    public void Initialise(GameObject gameObject, int testNumber, TrialsManager manager)
    {
        testCaseObject = gameObject;
        this.manager = manager;
        this.testNumber = testNumber;

        // Link buttons
        loadButton = testCaseObject.transform.Find("LoadButton").GetComponent<Button>();
        loadButton.onClick.AddListener(Load);
        unloadButton = testCaseObject.transform.Find("UnloadButton").GetComponent<Button>();
        unloadButton.onClick.AddListener(Unload);
        
        GameObject textCols = testCaseObject.transform.Find("TextCols").gameObject;
        
        foreach (Transform child in textCols.transform)
        {
            switch (child.name)
            {
                case "SpeedCol":
                    child.GetComponent<Text>().text = launchSpeed.ToString(floatFormat) + "m/s";
                    break;
                case "AngleCol":
                    child.GetComponent<Text>().text = launchAngle.ToString(floatFormat) + "°";
                    break;
                case "DevCol":
                    child.GetComponent<Text>().text = launchDeviation.ToString(floatFormat) + "°";
                    break;
                case "NumCol":
                    child.GetComponent<Text>().text = "#" + testNumber;
                    break;
            }
        }
        // Get test status box
        testStatus = testCaseObject.transform.Find("TestStatus").GetComponent<Text>();
    }

    public void Load()
    {
        manager.LoadTest(this);

        testStatus.text = "Loaded";
        testStatus.color = CustomColors.Orange;
    }

    public void Unload()
    {
        manager.UnloadTest();
        
        // Change status
        testStatus.text = "Not Loaded";
        testStatus.color = CustomColors.Black;
    }

    public void CompleteTest(bool caught)
    {
        // Write status
        if (caught)
            testStatus.text = "Catch";
        else
            testStatus.text = "No Catch";
        testStatus.color = CustomColors.Black;
    }

    public void UnloadTest()
    {
        // Change status
        testStatus.text = "Not Loaded";
        testStatus.color = CustomColors.Black;
    }

    public string ToConfigFormat()
    {
        string marker, content;
        
        switch (buildType)
        {
            case TestCase.BuildType.InitialParameters:
                marker = ConfigurationManager.PARAMETERS_TEST;
                content = "" + launchSpeed + "," + launchAngle + "," + launchDeviation;
                break;
            case TestCase.BuildType.InitialVelocity:
                marker = ConfigurationManager.VELOCITY_TEST;
                content = "" + initialVelocityVector.x + "," + initialVelocityVector.y + "," + initialVelocityVector.z;
                break;
            case TestCase.BuildType.Target:
                marker = ConfigurationManager.TARGET_TEST;
                content = "" + target.x + "," + target.y + "," + target.z;
                break;
            default: // Should never be
                marker = content = "";
                Debug.LogError("Test build type undefined");
                break;
        }

        return ConfigurationManager.TEST_MARKER + marker + ConfigurationManager.ASSIGN_MARKER + content;
    }
}
