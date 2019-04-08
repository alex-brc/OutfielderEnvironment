using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Fove.Managed;
using System;

public class DataManager : MonoBehaviour
{
    [Header("Controls")]
    public bool active = true;
    [Tooltip("Relative path to the folder where this writer saves its data (folder structure will be created if necessary)")]
    public string rootDataPath;
    [Tooltip("Time in seconds between file writes")]
    public float writeInterval = 1;

    [Header("References")]
    public GameObject baseball;
    public FoveInterface fove;
    
    internal string testPath;
    internal List<Collector> collectors;
    
    private string dataPath;
    private long startingFrame;
    private float startingTime;
    private volatile bool writerOn;

    public void Awake()
    {
        // Init workers list
        collectors = new List<Collector>();
    }

    public static string ToCSVLine(object[] vals)
    {
        string output = vals[0].ToString();
        for(int i = 1; i < vals.Length; i++)
        {
            output += "," + vals[i];
        }

        return output + "\r\n";
    }

    public void Init(string subjectName, string age, string gender, string handedness, string other)
    {
        dataPath = rootDataPath + "\\" + DateTime.Now.ToString("dd-MM-yy") 
            + "\\" + subjectName.Replace(' ','_');
        // Create subject folder
        Directory.CreateDirectory(dataPath);
        
        // Write info file
        string subjectFileName = dataPath + "\\subject_info.csv";
        string output = "" +
            "Name,Age,Gender,Handedness,OtherInformation\r\n" + // Columns
            subjectName + "," + age + "," + gender + "," + handedness + "," + other + "\r\n"; // Values
        File.WriteAllText(subjectFileName, output);
    }

    public void WriteTestsFile(TestCase[] tests)
    {
        // Write test list file
        string testsFileName = dataPath + "\\test_list.csv";
        string output = "" +
            "No,Speed,Angle,Deviation,TargetX,TargetZ,MaxHeight,VelocityX,VelocityY,VelocityZ\r\n"; // Columns
        foreach (TestCase test in tests)
        {
            output += test.ToCSVLine(); // Values
        }
        File.WriteAllText(testsFileName, output);
    }

    public static String ExtractObjectPositionsForMatlab(String fileName, String gameObjectName)
    {
        StringBuilder X = new StringBuilder().Append("xPos = [ ");
        StringBuilder Y = new StringBuilder().Append("yPos = [ ");
        StringBuilder Z = new StringBuilder().Append("zPos = [ ");
        StringBuilder F = new StringBuilder().Append("frame = [ ");
        StringBuilder T = new StringBuilder().Append("time = [ ");

        using (StreamReader sr = new StreamReader(fileName))
        {
            while(sr.Peek() >= 0)
            {
                string[] tokens = sr.ReadLine().Split(':');
                if (!tokens[3].Equals(gameObjectName))
                    continue;

                // Append time and frame
                F.Append(" " + tokens[0]);
                T.Append(" " + tokens[1]);

                // Append positions
                string[] positions = tokens[4].Split(';');
                X.Append(" " + positions[0]);
                Y.Append(" " + positions[1]);
                Z.Append(" " + positions[2]);
            }
        }
        
        F.Append("];");
        T.Append("];");
        X.Append("];");
        Y.Append("];");
        Z.Append("];");

        return
            X.ToString() + '\n' +
            Y.ToString() + '\n' +
            Z.ToString() + '\n' +
            F.ToString() + '\n' +
            T.ToString() + '\n' ;
    }

    public void StartNewTest(int testNumber, TestCase.TrialType type)
    {
        // Make the directory for this specific trial if it's not there
        testPath = dataPath;
        // Set trial or practice folder
        if (type == TestCase.TrialType.Trial)
            testPath += "\\TRIAL";
        else if(type == TestCase.TrialType.Practice)
            testPath += "\\PRACTICE";
        // Set test number folder
        testPath += "\\Test_#" + testNumber;
        // Create test attempt folder
        testPath += "\\" + DateTime.Now.ToString("HH-mm-ss");
        Directory.CreateDirectory(testPath);
        
        // Reset frames and time
        startingFrame = Time.frameCount;
        startingTime = Time.time;
        
        // Start writer
        writerOn = true;

        // Start all data collectors
        foreach (Collector c in collectors)
            c.StartCollecting();
    }

    public void CompleteTest(bool caught)
    {
        writerOn = false;

        // Stop all data collectors
        foreach (Collector c in collectors)
            c.StopCollecting();

        // Write a result file, first columns
        string cols = "Catch,Total_time,Total_frames\r\n";
        string fileName = testPath + "\\results.csv";
        File.WriteAllText(fileName, cols);

        int result = caught ? 1 : 0; 

        // Then results
        object[] vals = {
             result, (Time.time - startingTime), (Time.frameCount - startingFrame)};
        File.AppendAllText(fileName, ToCSVLine(vals));
    }

    public void ResetWriter()
    {
        dataPath = "";
        testPath = "";
        startingFrame = 0;
        startingTime = 0;
        writerOn = false;
    }

    public bool isActive()
    {
        return active;
    }

    public bool Running()
    {
        return writerOn;
    }
}
