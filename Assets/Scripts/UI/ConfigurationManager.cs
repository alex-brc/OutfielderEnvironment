using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;

[DisallowMultipleComponent]
public class ConfigurationManager : MonoBehaviour
{
    // Ball presets
    public const int CUSTOMBALL = 0, BASEBALL = 1, TENNISBALL = 2, SHUTTLECOCK = 3, FOOTBALL = 4, BASKETBALL = 5;
    // Markers
    public const string TEST_MARKER = "@", VARIABLE_MARKER = "$", COMMENT_MARKER = "//", ASSIGN_MARKER = "=";
    // Test types
    public const string TARGET_TEST = "T", VELOCITY_TEST = "V", PARAMETERS_TEST = "P";
    
    public string floatFormat = "0.##";

    [Header("Default values")]
    public int numberOfTests = 8;
    public int practiceRuns = 10;
    public int trialRuns = 30;
    public float maxSpeed = 5;
    public float startingDistance = 30;
    public bool auto = true;
    public float maxBallHeight = 20;
    public float radius = 8;
    public int targetsShape = 0;
    public int ballPreset = 1;
    public float ballMass = 0.1f;
    public float ballSize = 0.14f;
    public float ballFriction = 0.01f;
    public float pauseBetweenTrials = 3;

    [Header("UI elements")]
    public Text statusText;
    public InputField configurationFileName;
    public Button loadButton;
    public Button buildButton;
    public Button controlViewButton;
    public AutoManualToggle autoToggle;
    public BallPresetDropdown ballPresetDropdown;
    public InputField ballMassField;
    public InputField ballSizeField;
    public InputField ballFrictionField;
    public CustomSlider startingDistanceSlider;
    public CustomSlider maxSpeedSlider;
    public InputField numberOfTestsBox;
    public InputField practiceRunsBox;
    public InputField trialRunsBox;
    public InputField maxBallHeightBox;
    public InputField radiusBox;
    public Dropdown targetsShapeDropdown;
    public InputField pauseBetweenBox;

    [Header("References")]
    public TrialsManager manager;
    public TestBuilder builder;
    public DataManager dataManager;
    public PlayerController player;
    public SubjectOperations subjectOp;
    public Rigidbody ball;
    
    internal bool loadedOk;
    internal bool testsBuilt;
    internal List<TestCase> tests;

    private void Start()
    {
        tests = new List<TestCase>();
        loadButton.interactable = false;
        buildButton.interactable = false;
        testsBuilt = false;
        FillUI();
    }

    public void LoadButton()
    {
        // Load config file, if any present
        tests = new List<TestCase>();
        if (!LoadConfig()) {
            statusText.color = CustomColors.Red;
            loadedOk = false;
        }
        else if (!CheckConfig())
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

        // Make build buttons available
        buildButton.interactable = true;

        SetBallValues(); // based on the ball presets

        UpdateValuesExtern();
        
        // Finally fill the boxes
        FillUI();
    }

    public void ResetTests()
    {
        tests = new List<TestCase>();
        manager.TestCases = null;
        builder.ClearUI();
        controlViewButton.interactable = false;
    }

    public void BuildTests()
    {
        UpdateValues();
        UpdateValuesExtern();

        if (auto)
        {
            builder.MakeTargets();
            tests = builder.GetTests();
        }

        manager.TestCases = tests.ToArray();
        builder.UpdateUI();
        dataManager.WriteTestsFile(manager.TestCases);
        if(subjectOp.hasSubject)
            controlViewButton.interactable = true;
    }

    public void UpdateValues()
    {
        numberOfTests = int.Parse(numberOfTestsBox.text);
        practiceRuns = int.Parse(practiceRunsBox.text);
        trialRuns = int.Parse(trialRunsBox.text);
        maxSpeed = maxSpeedSlider.GetValue() ;
        startingDistance = startingDistanceSlider.GetValue();
        auto = autoToggle.GetValue() ;
        maxBallHeight = float.Parse(maxBallHeightBox.text);
        radius = float.Parse(radiusBox.text);
        targetsShape = targetsShapeDropdown.value;
        ballPreset = ballPresetDropdown.GetValue();
        if (ballPreset == 0)
        {
            ballMass = float.Parse(ballMassField.text);
            ballSize = float.Parse(ballMassField.text);
            ballFriction = float.Parse(ballFrictionField.text);
        }
        SetBallValues();
        pauseBetweenTrials = float.Parse(pauseBetweenBox.text);
    }

    public void UpdateValuesExtern()
    {
        manager.practiceRuns = practiceRuns;
        manager.trialRuns = trialRuns;
        manager.pauseBetweenTrials = pauseBetweenTrials;
        manager.catcherStartPosition = new Vector3(startingDistance, 0, 0);
        player.maximumSpeed = maxSpeed;
        
        // Set ball
        ball.mass = ballMass;
        ball.drag = ballFriction;
        ball.transform.localScale = new Vector3(ballSize, ballSize, ballSize);
    }

    internal void FillUI()
    {
        // Fill regular boxes
        startingDistanceSlider.SetValue(startingDistance);
        maxSpeedSlider.SetValue(maxSpeed);
        numberOfTestsBox.text = numberOfTests.ToString();
        practiceRunsBox.text = practiceRuns.ToString();
        trialRunsBox.text = trialRuns.ToString();
        maxBallHeightBox.text = maxBallHeight.ToString(floatFormat);
        radiusBox.text = radius.ToString(floatFormat);
        pauseBetweenBox.text = pauseBetweenTrials.ToString(floatFormat);

        // Do ball boxes
        ballPresetDropdown.SetValue(ballPreset);
        if (ballPreset == 0)
        {
            ballPresetDropdown.FillBoxes(ballSize, ballMass, ballFriction, floatFormat);
        }
    }

    private void SetBallValues()
    {
        // Set values for ballMass, ballFriction, ballSize
         switch(ballPreset)
         {
            case CUSTOMBALL:
                // Values already set
                break;
            case BASEBALL:
                ballMass = BallPreset.Baseball.mass;
                ballSize = BallPreset.Baseball.size;
                ballFriction = BallPreset.Baseball.friction;
                break;
            case TENNISBALL:
                ballMass = BallPreset.Tennisball.mass;
                ballSize = BallPreset.Tennisball.size;
                ballFriction = BallPreset.Tennisball.friction;
                break;
            case SHUTTLECOCK:
                ballMass = BallPreset.Shuttlecock.mass;
                ballSize = BallPreset.Shuttlecock.size;
                ballFriction = BallPreset.Shuttlecock.friction;
                break;
            case FOOTBALL:
                ballMass = BallPreset.Football.mass;
                ballSize = BallPreset.Football.size;
                ballFriction = BallPreset.Football.friction;
                break;
            case BASKETBALL:
                ballMass = BallPreset.Basketball.mass;
                ballSize = BallPreset.Basketball.size;
                ballFriction = BallPreset.Basketball.friction;
                break;
            default:
                statusText.text = "Something went wrong, unidentified ball preset.";
                break;
        }
    }

    private void OnApplicationQuit()
    {
        // Save config file
        SaveConfig();
    }
    
    private bool CheckConfig()
    {
        if (numberOfTests <= 0
        || practiceRuns < 0
        || trialRuns <= 0
        || pauseBetweenTrials <= 0
        || maxSpeed <= 0
        || startingDistance <= 0
        || ballSize <= 0
        || ballMass <= 0
        || ballFriction <= 0
        || ballFriction >= 1)
        {
            statusText.text = "Values outside accepted ranges in config file.";
            return false;
        }
        if (auto &&
            (maxBallHeight <= 0 || maxBallHeight > 100
          || radius <= 0 || radius >= 100
          || (targetsShape != 0 && targetsShape != 1)))
        {
            statusText.text = "Auto parameters outside accepted ranges in config file.";
            return false;
        }
        if (ballPreset > 6 || ballPreset < 1)
        {
            statusText.text = "Ball preset outside accepted range in config file.";
            return false;
        }
        if (!auto && numberOfTests != tests.Count)
        {
            statusText.text = "Number of tests (" + numberOfTests + ") doesn't match the length of the test list (" + tests.Count + ") in config file.";
            return false;
        }
        if (auto && tests.Count > 0)
        {
            statusText.text = "Auto mode but found tests in config file.";
            return false;
        }
        // Everything ok
        return true;
    }

    private bool LoadConfig()
    {
        // Read lines
        List<string> lines = new List<string>();
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
                    
                    // Remove comments
                    int commentPosition = line.IndexOf("//");
                    if (commentPosition != -1)
                        line = line.Remove(commentPosition);

                    lines.Add(line);
                    line = reader.ReadLine();
                }
            }
        }
        catch(FileNotFoundException)
        {
            statusText.text = "Config file not found.";
            return false;
        }
        catch(IOException e)
        {
            statusText.text = "Config file not loaded: " + e.ToString();
            return false;
        }
        
        // Keep track of line numbers
        int lineNo = 1;
        // Process the lines
        foreach(string line in lines)
        {
            if (line.StartsWith(VARIABLE_MARKER))
            {
                if (!line.Contains(ASSIGN_MARKER))
                {
                    statusText.text = "Line containing variable marker (" + VARIABLE_MARKER 
                        + ") without an assignment marker (" + ASSIGN_MARKER + ") (line:" + lineNo + ")";
                    return false;
                }

                // varname should be in [0] (+$) val should be in [1]  
                string[] tokens = line.Split('=');
                switch (tokens[0])
                {
                    case (VARIABLE_MARKER + "number_of_tests"):
                        if (!int.TryParse(tokens[1], out numberOfTests))
                        {
                            Debug.Log("Got num tests, now value is " + numberOfTests);
                            statusText.text = "Illegal value \"" + tokens[1] + "\" for number_of_tests (line:" + lineNo + ")";
                            return false;
                        }
                        break;
                    case (VARIABLE_MARKER + "practice_runs"):
                        if (!int.TryParse(tokens[1], out practiceRuns))
                        {
                            statusText.text = "Illegal value \"" + tokens[1] + "\" for practice_runs (line:" + lineNo + ")";
                            return false;
                        }
                        break;
                    case (VARIABLE_MARKER + "trial_runs"):
                        if (!int.TryParse(tokens[1], out trialRuns))
                        {
                            statusText.text = "Illegal value \"" + tokens[1] + "\" for trial_runs (line:" + lineNo + ")";
                            return false;
                        }
                        break;
                    case (VARIABLE_MARKER + "auto"):
                        if (!bool.TryParse(tokens[1], out auto))
                        {
                            statusText.text = "Illegal value \"" + tokens[1] + "\" for auto (line:" + lineNo + ")";
                            return false;
                        }
                        break;
                    case (VARIABLE_MARKER + "max_ball_height"):
                        if (!float.TryParse(tokens[1], out maxBallHeight))
                        {
                            statusText.text = "Illegal value \"" + tokens[1] + "\" for max_ball_height (line:" + lineNo + ")";
                            return false;
                        }
                        break;
                    case (VARIABLE_MARKER + "radius"):
                        if (!float.TryParse(tokens[1], out radius))
                        {
                            statusText.text = "Illegal value \"" + tokens[1] + "\" for radius (line:" + lineNo + ")";
                            return false;
                        }
                        break;
                    case (VARIABLE_MARKER + "targets_shape"):
                        if (!int.TryParse(tokens[1], out targetsShape))
                        {
                            statusText.text = "Illegal value \"" + tokens[1] + "\" for targets_shape (line:" + lineNo + ")";
                            return false;
                        }
                        break;
                    case (VARIABLE_MARKER + "pause_between_trials"):
                        if (!float.TryParse(tokens[1], out pauseBetweenTrials))
                        {
                            statusText.text = "Illegal value \"" + tokens[1] + "\" for pause_between_trials (line:" + lineNo + ")";
                            return false;
                        }
                        break;
                    case (VARIABLE_MARKER + "max_speed"):
                        if (!float.TryParse(tokens[1], out maxSpeed))
                        {
                            statusText.text = "Illegal value \"" + tokens[1] + "\" for max_speed (line:" + lineNo + ")";
                            return false;
                        }
                        break;
                    case (VARIABLE_MARKER + "starting_distance"):
                        if (!float.TryParse(tokens[1], out startingDistance))
                        {
                            statusText.text = "Illegal value \"" + tokens[1] + "\" for starting_distance (line:" + lineNo + ")";
                            return false;
                        }
                        break;
                    case (VARIABLE_MARKER + "ball_preset"):
                        if (!int.TryParse(tokens[1], out ballPreset))
                        {
                            statusText.text = "Illegal value \"" + tokens[1] + "\" for ball_preset (line:" + lineNo + ")";
                            return false;
                        }
                        break;
                    case (VARIABLE_MARKER + "ball_size"):
                        if (!float.TryParse(tokens[1], out ballSize))
                        {
                            statusText.text = "Illegal value \"" + tokens[1] + "\" for ball_size (line:" + lineNo + ")";
                            return false;
                        }
                        break;
                    case (VARIABLE_MARKER + "ball_mass"):
                        if (!float.TryParse(tokens[1], out ballMass))
                        {
                            statusText.text = "Illegal value \"" + tokens[1] + "\" for ball_mass (line:" + lineNo + ")";
                            return false;
                        }
                        break;
                    case (VARIABLE_MARKER + "ball_friction"):
                        if (!float.TryParse(tokens[1], out ballFriction))
                        {
                            statusText.text = "Illegal value \"" + tokens[1] + "\" for ball_friction (line:" + lineNo + ")";
                            return false;
                        }
                        break;
                    default:
                        statusText.text = "Config file contains undefined variable name \"" + tokens[1] + "\". (line:" + lineNo + ")";
                        return false; // Undefined variable name
                }
            }
            else if (line.StartsWith(TEST_MARKER))
            {
                // Specification type in [0] (+@), values in [1]
                string[] tokens = line.Split('=');
                Vector3 temp = new Vector3();

                switch (tokens[0])
                {
                    case (TEST_MARKER + TARGET_TEST):
                        if (ParseVector(tokens[1], out temp))
                            tests.Add(new TestCase(new Vector3(temp.x, 0, temp.y), temp.z, tests.Count + 1));
                        else
                        {
                            statusText.text = "Illegal values \"" + tokens[1] + "\" for a target test. (line:" + lineNo + ")";
                            return false;
                        }
                        break;
                    case (TEST_MARKER + VELOCITY_TEST):
                        if (ParseVector(tokens[1], out temp))
                            tests.Add(new TestCase(temp, tests.Count + 1));
                        else
                        {
                            statusText.text = "Illegal values \"" + tokens[1] + "\" for an initial velocity test. (line:" + lineNo + ")";
                            return false;
                        }
                        break;
                    case (TEST_MARKER + PARAMETERS_TEST):
                        if (ParseVector(tokens[1], out temp))
                            tests.Add(new TestCase(temp.x,temp.y,temp.z, tests.Count + 1));
                        else
                        {
                            statusText.text = "Illegal values \"" + tokens[1] + "\" for a parameters test. (line:" + lineNo + ")";
                            return false;
                        }
                        break;
                    default:
                        statusText.text = "Unknown test identifier \"" + tokens[0] + "\". (line:" + lineNo + ")";
                        return false;
                }
            }

            // Increase line number
            lineNo++;
        }
        
        return true;
    }
    
    public bool SaveConfig()
    {
        UpdateValues();
        if (!loadedOk || !auto)
            return false;
        try
        {
            using (StreamWriter writer = new StreamWriter("saved_configuration.cfg"))
            {
                // Write variables
                writer.WriteLine(VARIABLE_MARKER + "starting_distance" + ASSIGN_MARKER + startingDistance.ToString());
                writer.WriteLine(VARIABLE_MARKER + "max_speed" + ASSIGN_MARKER + maxSpeed.ToString());
                writer.WriteLine(VARIABLE_MARKER + "number_of_tests" + ASSIGN_MARKER + numberOfTests.ToString());
                writer.WriteLine(VARIABLE_MARKER + "practice_runs" + ASSIGN_MARKER + practiceRuns.ToString());
                writer.WriteLine(VARIABLE_MARKER + "trial_runs" + ASSIGN_MARKER + trialRuns.ToString());
                writer.WriteLine(VARIABLE_MARKER + "pause_between_trials" + ASSIGN_MARKER + pauseBetweenTrials.ToString());

                if (ballPreset != 0)
                {
                    writer.WriteLine(VARIABLE_MARKER + "ball_preset" + ASSIGN_MARKER + ballPreset.ToString());
                    writer.WriteLine(VARIABLE_MARKER + "ball_size" + ASSIGN_MARKER + ballSize.ToString());
                    writer.WriteLine(VARIABLE_MARKER + "ball_mass" + ASSIGN_MARKER + ballMass.ToString());
                    writer.WriteLine(VARIABLE_MARKER + "ball_friction" + ASSIGN_MARKER + ballFriction.ToString());
                }

                if (auto)
                {
                    writer.WriteLine(VARIABLE_MARKER + "auto" + ASSIGN_MARKER + auto.ToString());
                    writer.WriteLine(VARIABLE_MARKER + "max_ball_height" + ASSIGN_MARKER + maxBallHeight.ToString());
                    writer.WriteLine(VARIABLE_MARKER + "radius" + ASSIGN_MARKER + radius.ToString());
                    writer.WriteLine(VARIABLE_MARKER + "targets_shape" + ASSIGN_MARKER + targetsShape.ToString());
                }
                else
                {
                    // Write list of tests
                    foreach(TestCase test in tests)
                    {
                        writer.WriteLine(test.ToConfigFormat());
                    }
                }
            }
        }
        catch(IOException e)
        {
            Debug.LogError("Couldn't save config. " + e.ToString());
            return false;
        }

        return true;
    }

    private bool ParseVector(string input, out Vector3 vector)
    {
        vector = new Vector3();
        string[] tokens = input.Split(',');
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

        return true;
    }
}
