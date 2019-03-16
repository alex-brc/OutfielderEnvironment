using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    private enum ControllerType { FOVE = 0, JOYSTICK = 1 }
    private enum InputCurveType { LINEAR = 0, SINUSOIDAL = 1, EXPONENTIAL = 2, SIGMOID = 3 }

    [Header("Configurable data")]
    public Configurable<int> controllerType = new Configurable<int>();
    public Configurable<bool> inputSmoothing = new Configurable<bool>();
    public Configurable<int> smoothingAmount = new Configurable<int>();
    public Configurable<int> inputCurve = new Configurable<int>();
    public Configurable<float> curveParameter = new Configurable<float>();

    public FoveInterface fove;

    [Header("Graph references")]
    public int numSamples;
    public RectTransform graphArea;
    public LineRenderer graphLine;
    public LineRenderer diagLine;
    public Text graphInfo;

    private ControllerType controller;
    private InputCurveType curve;

    internal Vector3 latestInput;
    private Vector3 latestRawInput;
    private Smoother smoother;

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
        Debug.Log("Zero pos: " + zeroPosition);
        // Get raw input vector
        input = latestRawInput;
        Debug.Log("Raw input: " + input);
        Debug.Log("Relative input: " + (input-zeroPosition));
        // Get calibrated input vector
        input = Calibrated(input);
        Debug.Log("Calibrated input: " + input);

        // Put it through the curve
        input = Curved(input);
        Debug.Log("Curved input: " + input);

        return input;
    }
    
    /// <summary>
    /// Applies the specified input curve to the magnitude.
    /// </summary>
    /// <param name="magnitude">The magnitude of the input vector (clamped to [0..1])</param>
    /// <returns>The curved magitude, guaranteed to be in [0..1]</returns>
    private Vector3 Curved(Vector3 input)
    {
        return input.normalized * Curved(input.magnitude);
    }

    private float Curved(float val)
    {
        float x = Mathf.Clamp01(val);

        switch (curve)
        {
            case InputCurveType.LINEAR:
                if (x < curveParameter.Get())
                    return x / curveParameter.Get();
                return 1;
            case InputCurveType.SINUSOIDAL:
                if (x * x < curveParameter.Get())
                    return -0.5f * Mathf.Cos(Mathf.PI * x * x / curveParameter.Get()) + 0.5f;
                return 1;
            case InputCurveType.EXPONENTIAL:
                return Mathf.Pow(x, curveParameter.Get()); // This can get heavy for big powers
            case InputCurveType.SIGMOID: // May be computationally intensive
                return ModulusSigmoid(x);
        }
        // Will never reach
        return 0;
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

    private Vector3 GetFOVERawInput()
    {
        Vector3 input = new Vector3
        {
            z = Input.mousePosition.y,
            x = Input.mousePosition.x
        };
        return input;
        // return fove.transform.localPosition.XZ();
    }

    private Vector3 GetJoystickRawInput()
    {
        Vector3 input = new Vector3()
        {
            x = -Input.GetAxis("Vertical"),
            z = Input.GetAxis("Horizontal")
        };
        return input;
    }

    private bool siginit = false;
    private float k1 = 0.809f, k2 = 1.6181f, k;
    /// <summary>
    /// A parametrised version of the modulus sigmoid function x / (1 + |x|)
    /// </summary>
    /// <param name="x">The value </param>
    /// <returns></returns>
    public float ModulusSigmoid(float x)
    {
        if (!siginit)
        {
            // A wild assortment of magic maths. Plot this on a graph and 
            // see what happens with changes in the parameter.
            k = (k1 * (curveParameter.Get() - k2)
                / (1 + Mathf.Abs(curveParameter.Get() - k2)))
                + 0.5f;
            siginit = true;
        }
        float arg = curveParameter.Get() * x * x - k2;
        return (k1 * arg / (1 + Mathf.Abs(arg)) + 0.5f) / k;
    }

    public void SelectInputCurve()
    {
        switch (inputCurve.Get())
        {
            case (int)InputCurveType.LINEAR:
                curve = InputCurveType.LINEAR;
                break;
            case (int)InputCurveType.SINUSOIDAL:
                curve = InputCurveType.SINUSOIDAL;
                break;
            case (int)InputCurveType.EXPONENTIAL:
                curve = InputCurveType.EXPONENTIAL;
                break;
            case (int)InputCurveType.SIGMOID:
                curve = InputCurveType.SIGMOID;
                break;
            default:
                throw new System.Exception("Undefined input curve type: " + inputCurve.Get());
        }
    }
    
    public void DrawCurveGraph(string valueString)
    {
        if (valueString == "")
            return;
        
        // Check parameter is in range
        float value = float.Parse(valueString);
        switch (inputCurve.Get())
        {
            case (int)InputCurveType.LINEAR:
                if (value < 0 || value > 1)
                {
                    graphInfo.text = "Invalid parameter";
                    return;
                }
                break;
            case (int)InputCurveType.SINUSOIDAL:
                if (value < 0 || value > 1)
                {
                    graphInfo.text = "Invalid parameter";
                    return;
                }
                break;
            case (int)InputCurveType.EXPONENTIAL:
                if (value < 0)
                {
                    graphInfo.text = "Invalid parameter";
                    return;
                }
                break;
            case (int)InputCurveType.SIGMOID:
                if (value < 1)
                {
                    graphInfo.text = "Invalid parameter";
                    return;
                }
                siginit = false;
                break;
        }

        // Draw y=x
        diagLine.positionCount = 2;
        diagLine.SetPosition(0, Vector3.zero);
        diagLine.SetPosition(1, new Vector3(
            graphArea.rect.width,
            graphArea.rect.height));

        // Draw graph
        graphLine.positionCount = numSamples;
        for(int i = 0; i < numSamples; i++)
        {
            graphLine.SetPosition(i, 
                new Vector3(1.0f * i/numSamples * graphArea.rect.width,
                Curved(1.0f * i/numSamples) * graphArea.rect.height
                ));
        }
    }

    public void DrawCurveGraph()
    {
        DrawCurveGraph(curveParameter.ToString());
    }

    public void SelectController()
    {
        switch (controllerType.Get())
        {
            case (int)ControllerType.FOVE:
                controller = ControllerType.FOVE;
                break;
            case (int)ControllerType.JOYSTICK:
                controller = ControllerType.JOYSTICK;
                break;
            default:
                throw new System.Exception("Undefined controller type: " + controllerType.Get());
        }
    }
    
    void Update()
    { 
        // Grab input
        switch (controller)
        {
            case ControllerType.FOVE:
                latestRawInput = GetFOVERawInput();
                break;
            case ControllerType.JOYSTICK:
                latestRawInput = GetJoystickRawInput();
                break;
        }
        if (inputSmoothing.Get())
        {
            smoother.Add(latestRawInput);
            smoother.Check();
            latestRawInput = smoother.Get();
        }

        if(calibrated)
            latestInput = GetInputVector();

        if (calibrating)
        {
            Debug.Log("calibrating");
            Vector3 currentLean = latestRawInput - zeroPosition;
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

    internal void SetZeroPosition()
    {
        zeroPosition = latestRawInput;
    }

    internal void ClearCalibration()
    {
        zeroPosition = Vector3.zero;
        minimumLean.x = minimumLean.z = Mathf.Infinity;
        maximumLean.x = maximumLean.z = Mathf.NegativeInfinity;
    }

    public void Start()
    {
        calibrating = calibrated = false;
        minimumLean.x = minimumLean.z = Mathf.Infinity;
        maximumLean.x = maximumLean.z = Mathf.NegativeInfinity;
        smoother = new Smoother(smoothingAmount.Get());
    }

    public void Refresh()
    {
        SelectInputCurve();
        SelectController();
        smoother = new Smoother(smoothingAmount.Get());
    }
}

class Smoother {

    struct Input
    {
        public Input(Vector3 input)
        {
            this.vector = input;
            this.timestamp = Time.time;
        }
        public Vector3 vector;
        public float timestamp;
    }

    LinkedList<Input> inputs;
    float window;

    public Smoother(int smoothingWindow)
    {
        // winow is in seconds, smoothingwindow in miliseconds
        window = 1.0f * smoothingWindow / 1000;
        inputs = new LinkedList<Input>();
    }

    public void Add(Vector3 input)
    {
        Input newInput = new Input(input);
        inputs.AddLast(newInput);
        Debug.Log("added one");
    }

    public Vector3 Get()
    {
        // Average inputs
        Vector3 result = Vector3.zero;
        foreach(Input i in inputs)
            result += i.vector;
        result /= inputs.Count;
        Debug.Log("retrieved one");
        return result;
    }

    public void Check()
    {
        // Check wether the first input has expired
        Input i = inputs.First.Value;
        if(Time.time - i.timestamp > window)
        {
            // This input has expired, remove it
            inputs.RemoveFirst();
            Debug.Log("removed one");
        }
    }
}
