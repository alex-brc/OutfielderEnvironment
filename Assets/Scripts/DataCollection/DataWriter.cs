using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Fove.Managed;
using System;
using RockVR.Video;

public class DataWriter : MonoBehaviour
{
    private const int MIN_NUM = 10000000, MAX_NUM = 20000000;

    [Header("Controls")]
    public bool active = true;
    [Tooltip("Relative path to the folder where this writer saves its data (folder structure will be created if necessary)")]
    public string rootDataPath;
    [Tooltip("Time in seconds between file writes")]
    public float writeInterval = 1;

    [Header("References")]
    public GameObject baseball;
    public FoveInterface fove;
    
    private StringBuilder stringBuilder;
    private string dataPath;
    private Coroutine writerCoroutine;
    private long startingFrame;
    private long latestFrame;
    private float startingTime;
    private float latestTime;
    private volatile bool writerOn;
    private string fileName;
    private System.Random rand;

    void Start()
    {
        rand = new System.Random();
    }

    public void Init(string subjectName)
    {
        dataPath = rootDataPath + "\\" + DateTime.Now.ToString("dd-MM-yy") 
            + "\\" + subjectName.Replace(' ','_');
        // Create subject folder
        Directory.CreateDirectory(dataPath);
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

    public void StartNewTest(int testNumber, TestCase.Type type)
    {
        // Make the file name and directory for this specific trial if it's not there
        fileName = dataPath + "\\Test_#"
            + testNumber;
        if (type == TestCase.Type.Trial)
            fileName += "\\TRIAL";
        else if(type == TestCase.Type.Practice)
            fileName += "\\PRACTICE";
        Directory.CreateDirectory(fileName);
        fileName += "\\" + System.DateTime.Now.ToString("HH-mm-ss") + ".txt";

        // Reset frames and time
        startingFrame = Time.frameCount;
        startingTime = Time.time;

        // Start writer
        writerOn = true;
        stringBuilder = new StringBuilder();
        writerCoroutine = StartCoroutine(WriterRoutine());
    }

    public void CompleteTest(bool caught)
    {
        writerOn = false;

        // Make sure to clean out the stringbuilder when stopping
        File.AppendAllText(fileName, stringBuilder.ToString());
        stringBuilder = new StringBuilder();

        // Also write results
        string result = "" +
            latestFrame + DataTags.MainSeparator +
            latestTime + DataTags.MainSeparator +
            DataTags.EntryTypes.Result + DataTags.MainSeparator;
        result += caught ? DataTags.Result.Success : DataTags.Result.Failure;
        File.AppendAllText(fileName, result);
    }

    public void Reset()
    {
        // Don't really need to do anything special right now
    }

    public void Record(GameObject gameObject, float recordTime)
    {
        latestFrame = Time.frameCount - startingFrame;
        latestTime = recordTime - startingTime;

        // An entry for a gameobject looks like this:
        // GAMEOBJECT:<objectName>:<posX>;<posZ>;<posZ>:<rotX>;<rotY>;<rotZ>:<velX>;<velY>;<velZ>
        string record = "" +
            latestFrame + DataTags.MainSeparator +
            latestTime + DataTags.MainSeparator +
            DataTags.EntryTypes.GameObject + DataTags.MainSeparator +
            gameObject.name + DataTags.MainSeparator +
                gameObject.transform.position.ToRecordFormat() +
            DataTags.MainSeparator +
                gameObject.transform.rotation.eulerAngles.ToRecordFormat() +
            DataTags.MainSeparator +
                gameObject.GetComponent<Rigidbody>().velocity.ToRecordFormat();

        if (Running())
            // Append <frameNumber>:<timeTag>: to the object string and add to stringbuilder
            stringBuilder.AppendLine(record);
    }

    public void RecordFOVE(FoveInterface fove, float recordTime)
    {
        latestFrame = Time.frameCount - startingFrame;
        latestTime = recordTime - startingTime;

        SFVR_Pose lastPose = FoveInterface.GetLastPose();

        Vector3 rotation = FoveInterface.GetHMDRotation().eulerAngles;
        // Gives rotation, relative position (to player object), gaze information
        // FOVE:Fove Interface::<posX>;<posZ>;<posZ>:<rotX>;<rotY>;<rotZ>:
        string record = "" +
            latestFrame + DataTags.MainSeparator +
            latestTime + DataTags.MainSeparator +
            DataTags.EntryTypes.FOVE + DataTags.MainSeparator +
            "Fove Interface" + DataTags.MainSeparator +
                lastPose.position.ToRecordFormat() +
            DataTags.MainSeparator +
                rotation.ToRecordFormat() +
            DataTags.MainSeparator;

        /* This doesn't work for some reason, CHECK LATER !!!!!!!!!!!!!!!!!!!!!!!!!
        // Get ball screen coordinates and write them
        Debug.Log(fove.ToString() + "is fove null? a:" + (fove == null));
        Vector3 ballCoord = 
            FoveInterface.
            .GetNormalizedViewportPointForEye(
                baseball
                .transform.position, 
                EFVR_Eye.Right);

        record += "" +
                ballCoord.x + DataTags.SecondarySeparator +
                ballCoord.y + DataTags.SecondarySeparator +
                ballCoord.z +
            DataTags.MainSeparator;
        */
        // Get ray
        FoveInterfaceBase.GazeConvergenceData gazeData = FoveInterface.GetGazeConvergence();
        // Get the world point where the covnergence is
        Vector3 worldConv = gazeData.ray.GetPoint(gazeData.distance);
        // Get the screen point for the world point
        Vector3 screenConv = fove.GetEyeCamera(EFVR_Eye.Right).WorldToScreenPoint(worldConv);
        // Write this into the record
        record += screenConv.ToRecordFormat();
        
        if (Running())
            // Append the object string to stringbuilder
            stringBuilder.AppendLine(record);

    }

    // Writing operations are performed once every 
    // writeInterval seconds so as to avoid writing 
    // to file on every frame.
    private IEnumerator WriterRoutine()
    {
        Loop:
        // Write contents of stringbuilder to file
        string output = stringBuilder.ToString();
        stringBuilder = new StringBuilder();
        File.AppendAllText(fileName, output);
        // Wait for writeInterval seconds before writing again.
        yield return new WaitForSeconds(writeInterval);
        if(Running())
            goto Loop;
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
