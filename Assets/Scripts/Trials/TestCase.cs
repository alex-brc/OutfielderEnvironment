using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class TestCase
{
    public enum TrialType { Practice, Trial, Robot };
    public enum BuildType { Target, InitialVelocity, InitialParameters };

    internal BuildType buildType;
    internal float duration;
    internal int testNumber;
    internal Vector3 target;
    internal float height;
    internal Vector3 initialVelocityVector;
    internal float launchSpeed; // Meters per second
    internal float launchAngle; // Degrees from plane
    internal float launchDeviation; // Degrees deviation from player

    private string floatFormat = "0.##";
    internal GameObject testCaseObject;

    /// <summary>
    /// Create a test case whose ball will land at the specified target,
    /// achieving the specified maximum ball height.
    /// </summary>
    /// <param name="target">The target point for the ball to land at</param>
    public TestCase(Vector3 target, float height, int testNumber)
    {
        this.testNumber = testNumber;

        Vector3 velocityVector = new Vector3();
        velocityVector.y = Mathf.Sqrt(-2 * (height-1) * Physics.gravity.y);
        float t =  (velocityVector.y + Mathf.Sqrt(velocityVector.y * velocityVector.y + 2 * Physics.gravity.y))/ (-1 * Physics.gravity.y);
        velocityVector.x = target.x / t;
        velocityVector.z = target.z / t;

        duration = t;

        initialVelocityVector = velocityVector;

        launchSpeed = velocityVector.magnitude;
        launchDeviation = Mathf.Sign(velocityVector.z) * Vector3.Angle(Vector3.right, velocityVector.XZ());
        launchAngle = Vector3.Angle(Vector3.right, velocityVector.XY());
        
        this.target = target;
        this.height = height;

        buildType = BuildType.Target;
    }

    /// <summary>
    /// Create a test case with the initial velocity.
    /// </summary>
    /// <param name="initialVelocityVector">The initial velocity vector of the ball</param>
    public TestCase(Vector3 initialVelocityVector, int testNumber)
    {
        this.testNumber = testNumber;

        this.initialVelocityVector = initialVelocityVector;

        launchSpeed = initialVelocityVector.magnitude;
        launchDeviation = Mathf.Sign(initialVelocityVector.z) * Vector3.Angle(Vector3.right, initialVelocityVector.XZ());
        launchAngle = Vector3.Angle(Vector3.right, initialVelocityVector.XY());

        // Compute target
        target = new Vector3();
        float t = (initialVelocityVector.y + Mathf.Sqrt(initialVelocityVector.y * initialVelocityVector.y + 2 * Physics.gravity.y)) / (-1 * Physics.gravity.y);

        duration = t;

        target.x = t * initialVelocityVector.x;
        target.z = t * initialVelocityVector.z;
        height = initialVelocityVector.y * initialVelocityVector.y / 2 * Physics.gravity.y + 1; // +1 since the ball starts at (0,1,0)

        buildType = BuildType.InitialVelocity;
    }

    /// <summary>
    /// Create a test case with the specified initial launch speed, deviation and angle.
    /// </summary>
    public TestCase(float launchSpeed, float launchDeviation, float launchAngle, int testNumber)
    {
        this.testNumber = testNumber;

        initialVelocityVector = Quaternion.Euler(0,launchDeviation,launchAngle) * Vector3.right * launchSpeed;
        
        this.launchSpeed = launchSpeed;
        this.launchAngle = launchAngle;
        this.launchDeviation = launchDeviation;

        // Compute target
        Vector3 target = new Vector3();
        float t = (initialVelocityVector.y + Mathf.Sqrt(initialVelocityVector.y * initialVelocityVector.y + 2 * Physics.gravity.y)) / (-1 * Physics.gravity.y);

        duration = t;

        target.x = t * initialVelocityVector.x;
        target.z = t * initialVelocityVector.z;
        height = initialVelocityVector.y * initialVelocityVector.y / 2 * Physics.gravity.y + 1; // +1 since the ball starts at (0,1,0)

        buildType = BuildType.InitialParameters;
    }

    public void Initialise(GameObject gameObject)
    {
        testCaseObject = gameObject;

        Transform textColumns = gameObject.transform.Find("TextCols");

        foreach (Transform child in textColumns)
        {
            switch (child.name)
            {
                case "Num":
                    child.GetComponent<Text>().text = "#" + testNumber;
                    break;
                case "Speed":
                    child.GetComponent<Text>().text = launchSpeed.ToString(floatFormat);
                    break;
                case "Angle":
                    child.GetComponent<Text>().text = launchAngle.ToString(floatFormat);
                    break;
                case "Deviation":
                    child.GetComponent<Text>().text = launchDeviation.ToString(floatFormat);
                    break;
                case "TargetX":
                    child.GetComponent<Text>().text = target.x.ToString(floatFormat);
                    break;
                case "TargetZ":
                    child.GetComponent<Text>().text = target.z.ToString(floatFormat);
                    break;
                case "MaxHeight":
                    child.GetComponent<Text>().text = height.ToString(floatFormat);
                    break;
                case "InitVelX":
                    child.GetComponent<Text>().text = initialVelocityVector.x.ToString(floatFormat);
                    break;
                case "InitVelY":
                    child.GetComponent<Text>().text = initialVelocityVector.y.ToString(floatFormat);
                    break;
                case "InitVelZ":
                    child.GetComponent<Text>().text = initialVelocityVector.z.ToString(floatFormat);
                    break;
            }
        }
    }

    public string ToCSVLine ()
    {
        object[] vals = { testNumber,
            launchSpeed, launchAngle, launchDeviation,
            target.x, target.z, height,
            initialVelocityVector.x, initialVelocityVector.y, initialVelocityVector.z};
        return DataManager.ToCSVLine(vals);
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
