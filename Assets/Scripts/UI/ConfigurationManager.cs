using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;

public class ConfigurationManager : MonoBehaviour
{
    // Ball presets
    public const int CUSTOMBALL = 0, BASEBALL = 1, TENNISBALL = 2, SHUTTLECOCK = 3, FOOTBALL = 4, BASKETBALL = 5;
    // Markers
    public const string TEST_MARKER = "@", VARIABLE_MARKER = "$", COMMENT_MARKER = "//", ASSIGN_MARKER = "=";
    // Test types
    public const string TARGET_TEST = "T", VELOCITY_TEST = "V", PARAMETERS_TEST = "P";
    
    public string configurationFileName;

    [Header("Default values")]
    public int numberOfTests = 8;
    public int practiceRuns = 10;
    public int trialRuns = 30;
    public float maxSpeed = 5;
    public float startingDistance = 30;
    public bool auto = true;
    public float maxBallHeight = 20;
    public float radius = 8;
    public int targetsShape = 1;
    public int ballPreset = 1;
    public float ballMass = 0.07f;
    public float ballSize = 0.2f;
    public float ballFriction = 0.01f;
    public int pauseBetweenTrials = 3;

    [Header("References")]
    public TrialsManager manager;
    public PlayerController player;
    public Rigidbody ball;
    public Text statusText;
    
    private List<TestCase> tests;

    void Start()
    {
        tests = new List<TestCase>();
        // Load config file, if any present
        if (!LoadConfig()) {
            statusText.color = CustomColors.Red;
        }
        else if (!CheckConfig())
        {
            statusText.color = CustomColors.Red;
        } 
        else
        {
            statusText.text = "Configuration file OK";
            statusText.color = CustomColors.Green;
        }

        // Create the tests if needed
        if (auto)
            MakeAutoTests();

        // Fill the boxes
        FillUI();

        // Set the values in respective classes
        manager.testCases = tests.ToArray();
        manager.practiceRuns = practiceRuns;
        manager.trialRuns = trialRuns;
        manager.pauseBetweenTrials = pauseBetweenTrials;
        manager.catcherStartPosition = new Vector3(startingDistance, 0, 0);
        player.maximumSpeed = maxSpeed;
        SetBallValues(); // based on the ball presets
    }

    private void FillUI()
    {

    }

    private void SetBallValues()
    {
        // For now just baseball
        if (ballPreset == CUSTOMBALL) // Custom
        {
            ball.mass = ballMass;
            ball.drag = ballFriction;
            ball.transform.localScale = new Vector3(ballSize, ballSize, ballSize);
        }
        else if(ballPreset == BASEBALL)
        {
            ballMass = 0.14f;
            ballFriction = 0.01f;
            ballSize = 0.1f;

            ball.mass = ballMass;
            ball.drag = ballFriction;
            ball.transform.localScale = new Vector3(ballSize, ballSize, ballSize);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private void OnApplicationQuit()
    {
        // Save config file
        SaveConfig();
    }

    private void MakeAutoTests()
    {
        throw new NotImplementedException();
    }
    
    private bool CheckConfig()
    {
        if (numberOfTests <= 0
        || practiceRuns <= 0
        || trialRuns <= 0
        || pauseBetweenTrials <= 0
        || maxSpeed <= 0
        || startingDistance <= 0
        || ballSize <= 0
        || ballMass <= 0
        || ballFriction <= 0
        || ballFriction >= 1)
            return false;
        if (auto &&
            (maxBallHeight <= 0 || maxBallHeight > 100
          || radius <= 0 || radius >= 100
          || (targetsShape != 1 && targetsShape != 2)))
            return false;
        if (ballPreset > 6 || ballPreset < 1)
            return false;
        if (!auto && numberOfTests != tests.Count)
            return false;
        // Everything ok
        return true;
    }

    private bool LoadConfig()
    {
        // Read lines
        List<string> lines = new List<string>();
        try
        {
            using (StreamReader reader = new StreamReader(configurationFileName))
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
        catch(IOException e)
        {
            Debug.LogError("Failed to load config file: " + e.ToString());
            return false;
        }

        Debug.Log(lines.Count);

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
                Debug.Log(tokens[0]);
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
                        if (!int.TryParse(tokens[1], out pauseBetweenTrials))
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
                        if (TryMakeTest(tokens[1], out temp))
                            tests.Add(new TestCase(temp, maxBallHeight));
                        else
                        {
                            statusText.text = "Illegal values \"" + tokens[1] + "\" for a target test. (line:" + lineNo + ")";
                            return false;
                        }
                        break;
                    case (TEST_MARKER + VELOCITY_TEST):
                        if (TryMakeTest(tokens[1], out temp))
                            tests.Add(new TestCase(temp));
                        else
                        {
                            statusText.text = "Illegal values \"" + tokens[1] + "\" for an initial velocity test. (line:" + lineNo + ")";
                            return false;
                        }
                        break;
                    case (TEST_MARKER + PARAMETERS_TEST):
                        if (TryMakeTest(tokens[1], out temp))
                            tests.Add(new TestCase(temp.x,temp.y,temp.z));
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

        // Make sure numbers add up
        if (tests.Count != numberOfTests)
        {
            statusText.text = "Number of tests (" + numberOfTests + ") doesn't match the length of the test list (" + tests.Count + ").";
            return false;
        }

        return true;
    }
    
    private bool SaveConfig()
    {
        return true;
    }

    private bool TryMakeTest(string values, out Vector3 vector)
    {
        vector = new Vector3();
        string[] tokens = values.Split(',');
        // Check format
        if (tokens.Length != 3)
            return false;

        float[] vals = new float[3];
        for (int i = 0; i < 3; i++)
        {
            if (!float.TryParse(tokens[i], out vals[i]))
                return false;
        }

        // Assign contents
        vector.x = values[0];
        vector.y = values[1];
        vector.z = values[2];

        return true;
    }
}
