using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Fove.Managed;
using System;

public class DataWriter : MonoBehaviour
{
    private const int MIN_NUM = 10000000, MAX_NUM = 20000000;

    public bool active = true;
    public string rootDataPath;
    public float writeInterval = 1;
    public GameObject baseball;
    
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
    
    public bool isActive()
    {
        return active;
    }

    public bool Running()
    {
        return writerOn;
    }

    public void Init(string subjectName)
    {
        // Set the dataPath. Folder structure:
        //
        // Data
        //     | <today's date>
        //                     | <subject's name>
        //                                        | Test_#<test number>
        //                                                          | <trial starting hour>.txt
        //                                                          | <other trial starting hour>.txt
        //                                                          | ...
        //                                        | Test_#<other test number>
        //                                        | ...
        //                     | <other subject's name>
        //                     | ...
        //     | <other dates>
        //     | ...

        //
        dataPath = rootDataPath + "\\" + DateTime.Now.ToString("dd-MM-yy") 
            + "\\" + subjectName.Replace(' ','_');
        // Create subject folder
        Directory.CreateDirectory(dataPath);
    }

    public void StartNewTest(int testNumber)
    {
        // Make the file name and directory for this specific trial if it's not there
        fileName = dataPath + "\\Test_#"
            + testNumber;
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
                gameObject.transform.position.x + DataTags.SecondarySeparator +
                gameObject.transform.position.y + DataTags.SecondarySeparator +
                gameObject.transform.position.z +
            DataTags.MainSeparator +
                gameObject.transform.rotation.eulerAngles.x + DataTags.SecondarySeparator +
                gameObject.transform.rotation.eulerAngles.y + DataTags.SecondarySeparator +
                gameObject.transform.rotation.eulerAngles.z +
            DataTags.MainSeparator +
                gameObject.GetComponent<Rigidbody>().velocity.x + DataTags.SecondarySeparator +
                gameObject.GetComponent<Rigidbody>().velocity.y + DataTags.SecondarySeparator +
                gameObject.GetComponent<Rigidbody>().velocity.z;

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
                lastPose.position.x + DataTags.SecondarySeparator +
                lastPose.position.y + DataTags.SecondarySeparator +
                lastPose.position.z +
            DataTags.MainSeparator +
                rotation.x + DataTags.SecondarySeparator +
                rotation.y + DataTags.SecondarySeparator +
                rotation.z +
            DataTags.MainSeparator;

        // Get ball screen coordinates and write them
        Vector3 ballCoord = fove.GetNormalizedViewportPointForEye(baseball.transform.position, EFVR_Eye.Right);
        record += "" +
                ballCoord.x + DataTags.SecondarySeparator +
                ballCoord.y + DataTags.SecondarySeparator +
                ballCoord.z +
            DataTags.MainSeparator;

        // Get ray
        FoveInterfaceBase.GazeConvergenceData gazeData = FoveInterface.GetGazeConvergence();
        // Get the world point where the covnergence is
        Vector3 worldConv = gazeData.ray.GetPoint(gazeData.distance);
        // Get the screen point for the world point
        Vector3 screenConv = fove.GetNormalizedViewportPointForEye(worldConv, EFVR_Eye.Right);
        // Write this into the record
        record += "" +
                screenConv.x + DataTags.SecondarySeparator +
                screenConv.y + DataTags.SecondarySeparator +
                screenConv.z;

        // Not tested
        throw new NotImplementedException();
        
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
}
