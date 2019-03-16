using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationManager : MonoBehaviour
{
    public enum RangeType { Closed, Open, LeftOpen, RightOpen }
    public const string 
        TEST_MARKER = "@", VARIABLE_MARKER = "$", COMMENT_MARKER = "//", ASSIGN_MARKER = "=",
        TARGET_TEST = "T", VELOCITY_TEST = "V", PARAMETERS_TEST = "P";

    public string floatFormat = "0.##";
    public TrialsManager manager;
    public TestBuilder builder;
    public UIManager uiManager;
    public PlayerController player;
    public BallController ball;
    public Controller controller;
    public InputField configurationFileName;
    public Text statusText;
    public Button buildTestsButton;

    internal Configurable<bool> auto = new Configurable<bool>();

    // Define all the variables
    internal IVariable[] variables;

    internal bool testsBuilt;
    internal bool loadedOk;
    internal List<TestCase> tests;

    private bool startup;

    public void Start() {
        variables = new IVariable[] {
            new Variable<bool>("auto", ref auto, false, ref uiManager.autoToggle, false, true, RangeType.Closed),
            new Variable<int>("number_of_tests", ref builder.numberOfTests, 8, ref uiManager.numberOfTestsBox, 0, int.MaxValue, RangeType.Closed),
            new Variable<int>("practice_runs", ref manager.practiceRuns, 10, ref uiManager.practiceRunsBox, 0, int.MaxValue, RangeType.Closed),
            new Variable<int>("trial_runs", ref manager.trialRuns, 30, ref uiManager.trialRunsBox, 0, int.MaxValue, RangeType.LeftOpen),
            new Variable<float>("max_ball_height", ref builder.maxBallHeight, 20, ref uiManager.maxBallHeightBox, 0, float.MaxValue, RangeType.LeftOpen),
            new Variable<float>("radius", ref builder.radius, 8, ref uiManager.radiusBox, 0, float.MaxValue, RangeType.LeftOpen),
            new Variable<int>("targets_shape", ref builder.targetsShape, 0, ref uiManager.targetsShapeDropdown, 0, 1, RangeType.Closed),
            new Variable<float>("pause_between_trials", ref manager.pauseBetweenTrials, 2, ref uiManager.pauseBetweenBox, 0, float.MaxValue, RangeType.LeftOpen),
            new Variable<float>("max_speed", ref player.maximumSpeed, 5, ref uiManager.maxSpeedBox, 0, float.MaxValue, RangeType.LeftOpen),
            new Variable<float>("starting_distance", ref manager.startingDistance, 30, ref uiManager.startingDistanceBox, 0, float.MaxValue, RangeType.LeftOpen, manager.Refresh),
            new Variable<float>("ball_size", ref ball.size, 0.1f, ref uiManager.ballSizeBox, 0, float.MaxValue, RangeType.LeftOpen, ball.Refresh),
            new Variable<float>("ball_mass", ref ball.mass, 0.14f, ref uiManager.ballMassBox, 0, float.MaxValue, RangeType.LeftOpen, ball.Refresh),
            new Variable<float>("ball_drag", ref ball.drag, 0.01f, ref uiManager.ballDragBox, 0, 1, RangeType.Open, ball.Refresh),
            new Variable<int>("ball_preset", ref ball.preset, 0, ref uiManager.ballPresetDropdown, 0, 5, RangeType.Closed, ball.Refresh),
            new Variable<int>("controller_type", ref controller.controllerType, 0, ref uiManager.controllerTypeDropdown, 0, 1, RangeType.Closed, controller.Refresh),
            new Variable<bool>("input_smoothing", ref controller.inputSmoothing, false, ref uiManager.inputSmoothingToggle, false, true, RangeType.Closed, controller.Refresh), 
            new Variable<int>("smoothing_amount", ref controller.smoothingAmount, 30, ref uiManager.smoothingAmountBox, 0, int.MaxValue, RangeType.Closed, controller.Refresh), 
            new Variable<int>("input_curve", ref controller.inputCurve, 3, ref uiManager.inputCurveDropdown, 0, 3, RangeType.Closed, controller.Refresh),
            new Variable<float>("curve_parameter", ref controller.curveParameter, 7, ref uiManager.curveParameterBox, 0, float.MaxValue, RangeType.LeftOpen, controller.DrawCurveGraph) 
        };
        
        tests = new List<TestCase>();
        testsBuilt = false;

        startup = true;
        // Push defaults to UI
        Push();
        startup = false;
    }
    
    /// <summary>
    /// The refresh cycle is pretty simple conceptually:
    ///     - Pull() values from the UI boxes
    ///     - Check() they're in the correct range
    ///     - Push() them to their respective references (and the UI)
    /// </summary>
    public void Refresh()
    {
        // Some variables will ask for refreshes when starting up, don't
        if (startup)
            return;

        Pull();
        Check();
        Push();
    }

    public void LoadButton()
    {
        // Load config file, if any present
        tests = new List<TestCase>();
        if (!Load())
        {
            statusText.color = CustomColors.Red;
            loadedOk = false;
        }
        else if (!Check())
        {
            statusText.color = CustomColors.Red;
            loadedOk = false;
        }
        else
        {
            statusText.text = "Configuration file loaded succesfully.";
            statusText.color = CustomColors.Black;
            loadedOk = true;
        }

        if (!loadedOk)
        {
            return;
        }
        
        Push(true);

        // Unlock build button
        buildTestsButton.interactable = true;
    }

    /// <summary>
    /// Reads the configuration files and loads all found variables
    /// into their respective Variable object. Also builds all found 
    /// tests and adds them to the test list.
    /// </summary>
    /// <returns>true if succeeded, false otherwise</returns>
    private bool Load()
    {
        // Read the file
        string[] lines;
        if (!ReadConfig(out lines))
            return false;
        for(int lineNo = 0; lineNo < lines.Length; lineNo++)
        {
            string line = lines[lineNo];

            // No assignment marker is not ok
            if (!line.Contains(ASSIGN_MARKER))
            {
                statusText.text = "Line doesn't contain an assignment marker (" + ASSIGN_MARKER + ") (line:" + lineNo + ")";
                return false;
            }

            if (line.StartsWith(VARIABLE_MARKER))
            {
                // $variable_name should be in [0], value should be in [1]  
                string[] tokens = line.Split('=');
                string varName = tokens[0].Substring(1); // without the $$
                string valueString = tokens[1];

                // Search for the variable in the defined list
                IVariable var = Find(varName, variables);
                if(var == null)
                {
                    statusText.text = "Config file contains undefined variable name \"" + varName + "\". (line:" + (lineNo + 1) + ")";
                    return false; // Undefined variable name
                }

                // We found it, now try to set its value
                if (!var.TryParse(valueString))
                {
                    statusText.text = "Illegal value \"" + tokens[1] + "\" for ball_friction (line:" + (lineNo + 1) + ")";
                    return false;
                }
            }
            else if (line.StartsWith(TEST_MARKER))
            {
                // @type in [0], values in [1]
                string[] tokens = line.Split('=');
                string typeSpecifier = tokens[0].Substring(1);
                string values = tokens[1];

                // Try to make a test
                if (!TryMakeTest(typeSpecifier, values))
                {
                    // Error message displayed already
                    return false;
                }
            }
        }
        
        // Everything alright!
        return true;
    }

    private void Save()
    {
        // Retrieve and check values from UI
        foreach(IVariable var in variables)
        {
            if (!var.Pull())
                Debug.Log("Couldn't pull variable " + var.Name());

            if (!var.Check())
                Debug.Log("Value out of range for variable " + var.Name());
        }

        // Don't try to save if manual
        if (!auto.Get())
            return;

        // Write everything
        try
        {
            using (StreamWriter writer = new StreamWriter("saved_configuration.cfg"))
            {
                // Write all variables
                foreach(IVariable var in variables)
                {
                    writer.WriteLine(var.ToString());
                }
            }
        }
        catch (IOException e)
        {
            Debug.LogError("Couldn't save config. " + e.ToString());
        }
    }

    /// <summary>
    /// Checks whether the set value is in the given
    /// range. If no value is set, returns true.
    /// </summary>
    private bool Check()
    {
        foreach(IVariable var in variables)
        {
            if (!var.Check())
            {
                statusText.text = "Variable " + var.Name() + 
                    " has value outside accepted range. " +
                    "Check the references for a list of accepted ranges.";
                return false;
            }
        }

        // Everything okay
        return true;
    }

    /// <summary>
    /// Pushes the value inside this variable (or the default if none set)
    /// to the given reference and the UI
    /// </summary>
    private void Push(bool loading = false)
    {
        foreach(IVariable var in variables)
        {
            // Push this value to the given destination
            var.Push(loading);
        }
    }

    /// <summary>
    /// Attempt to pull values from the UI
    /// </summary>
    private void Pull()
    {
        foreach (IVariable var in variables)
        {
            // Try to pull values from their respective ui containers
            var.Pull();
        }
    }
    
    private bool ReadConfig(out string[] lines)
    {
        lines = null;
        List<String> newLines = new List<String>();
        try
        {
            using (StreamReader reader = new StreamReader(configurationFileName.text))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    // Skip comment lines
                    if (line.StartsWith(COMMENT_MARKER))
                    {
                        line = reader.ReadLine();
                        continue;
                    }

                    // Remove whitespace
                    line = line.Replace(" ", "");

                    // Remove inline comments
                    int commentPosition = line.IndexOf("//");
                    if (commentPosition != -1)
                        line = line.Remove(commentPosition);

                    // If we're left with anything, add it
                    if (!line.Equals(""))
                        newLines.Add(line);

                    // Advance
                    line = reader.ReadLine();
                }
            }

            // Ok, return lines
            lines = newLines.ToArray();
        }
        catch (FileNotFoundException)
        {
            statusText.text = "Config file not found.";
            return false;
        }
        catch (IOException e)
        {
            statusText.text = "IOException! Config file not loaded: " + e.ToString();
            return false;
        }

        return true;
    }

    private IVariable Find(string name, IEnumerable<IVariable> list)
    {
        foreach (IVariable var in list)
            if (var.Name().Equals(name))
                return var;

        return null;
    }

    private bool TryMakeTest(string typeSpecifier, string value)
    {
        // Check vector values
        Vector3 vector = new Vector3();
        string[] tokens = value.Split(',');
        // Check format
        if (tokens.Length != 3)
            return false;

        float[] vals = new float[3];
        for (int i = 0; i < tokens.Length; i++)
        {
            if (!float.TryParse(tokens[i], out vals[i]))
                return false;
        }

        // Assign contents
        vector.x = vals[0];
        vector.y = vals[1];
        vector.z = vals[2];

        // Vector values okay, make test
        switch (typeSpecifier)
        {
            case TARGET_TEST:
                tests.Add(new TestCase(new Vector3(vector.x, 0, vector.y), vector.z, tests.Count + 1));
                return true;
            case VELOCITY_TEST:
                tests.Add(new TestCase(vector, tests.Count + 1));
                return true;
            case PARAMETERS_TEST:
                tests.Add(new TestCase(vector.x, vector.y, vector.z, tests.Count + 1));
                return true;
            default:
                statusText.text = "Unknown test identifier \"" + tokens[0] + "\"";
                return false;
        }
    }
}

