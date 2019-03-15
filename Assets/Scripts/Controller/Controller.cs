using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    private enum ControllerType { FOVE = 0, JOYSTICK = 1 }
    private enum InputCurveType { LINEAR = 0, SIGMOID = 1, EXPONENTIAL = 2, SINUSOID = 3 }

    // Configurable data
    public Configurable<int> controllerType;
    public Configurable<bool> smoothing;
    public Configurable<int> joystickSamplingFrequency;
    public Configurable<int> numberOfSmoothingSamples;
    public Configurable<int> inputCurve;
    public Configurable<float> curveParameter;
    
    // Button refs
    public Button calibrateButton;
    public Button calibrateEyetracking;

    public FoveInterface fove;

    internal bool calibrating, calibrated;
    private Vector3 zeroPosition;
    private Vector3 minimumLean, maximumLean;

    /// <summary>
    /// Returns a vector of magnitude [0..1] in the XZ plane representing the input.
    /// 
    /// <para>This input has already been processed (i.e. put through smoothing, curve, 
    /// etc. and is directly usable)</para>
    /// </summary>
    public Vector3 GetInputVector()
    {
        Vector3 input = new Vector3();

        // Get raw input vector
        input = LatestInput();

        // Get calibrated input vector
        input = Calibrated(input);

        // Put it through the curve
        input = Curved(input);
        
        return input;
    }
    
    /// <summary>
    /// Applies the specified input curve to the magnitude.
    /// </summary>
    /// <param name="magnitude">The magnitude of the input vector (clamped to [0..1])</param>
    /// <returns>The curved magitude, guaranteed to be in [0..1]</returns>
    private Vector3 Curved(Vector3 input)
    {
        float x = Mathf.Clamp01(input.magnitude);
        float y = 0;

        switch (inputCurve.Get())
        {
            case (int)InputCurveType.LINEAR:
                y = x;
                break;
            case (int)InputCurveType.SINUSOID:
                y = (-1 / 2) * Mathf.Cos(Mathf.PI * x) + 1 / 2;  
                break;
            case (int)InputCurveType.EXPONENTIAL:
                y = Mathf.Pow(x, curveParameter.Get()); // This can get heavy for big powers
                break;
            case (int)InputCurveType.SIGMOID: // Very computationally intensive
                y = ModulusSigmoid(x, curveParameter.Get());
                break;
            default:
                throw new System.Exception("Undefined input curve type: " + inputCurve.Get());
        }

        return input.normalized * y;
    }


    private Vector3 LatestInput()
    {
        Vector3 input = new Vector3();
        switch (controllerType.Get())
        {
            case (int)ControllerType.FOVE:
                input = GetFOVEInput();
                break;
            case (int)ControllerType.JOYSTICK:
                input = GetJoystickInput();
                break;
            default:
                throw new System.Exception("Undefined controller type: " + controllerType.Get());
        }

        if (smoothing.Get())
        {

        }

        return input;
    }

    private Vector3 Calibrated(Vector3 input)
    {
        // Raw motion vector
        input = input - zeroPosition;

        // Cutoff exceeding values
        input.x = Mathf.Clamp(input.x, minimumLean.x, maximumLean.x);
        input.z = Mathf.Clamp(input.z, minimumLean.z, maximumLean.z);
        
        if (input.x < 0) // Leaning back
            input.x /= Mathf.Abs(minimumLean.x);
        else             // Leaning forward
            input.x /= Mathf.Abs(maximumLean.x);

        if (input.z < 0) // Leaning left
            input.z /= Mathf.Abs(minimumLean.z);
        else             // Leaning right
            input.z /= Mathf.Abs(maximumLean.z);
        // Magnitude should be in [0..1] for each of x and z

        // Correct orientation
        input = Quaternion.Euler(0, -90, 0) * input;

        return input;
    }

    private Vector3 GetFOVEInput()
    {
        return fove.transform.localPosition.XZ();
    }

    private Vector3 GetJoystickInput()
    {
        Vector3 input = new Vector3()
        {
            x = -Input.GetAxis("Vertical"),
            z = Input.GetAxis("Horizontal")
        };
        Debug.Log("joystick x: " + input.x + " y: " + input.z);
        return input;
    }

    private bool init = false;
    private float a, b, c;
    /// <summary>
    /// A parametrised version of the modulus sigmoid function x / 1 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public float ModulusSigmoid(float x, float param)
    {
        if (!init)
        {
            a = Mathf.Clamp(param, 0.334f, 1);
            b = 2 * a;
            c = (1 - a) / (3 * a - 1) + 1;
            init = true;
        }
        float cxm1 = c * x - 1;
        return b * cxm1 / (1 + Mathf.Abs(cxm1)) + a;
    }
    
    public void CalibrateButton()
    {

    }
    
    void FixedUpdate()
    {
        if (calibrating)
        {
            Vector3 currentLean = LatestInput() - zeroPosition;
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

    public void Start()
    {
        calibrating = calibrated = false;
        minimumLean.x = minimumLean.z = Mathf.Infinity;
        maximumLean.x = maximumLean.z = Mathf.NegativeInfinity;
    }
}
