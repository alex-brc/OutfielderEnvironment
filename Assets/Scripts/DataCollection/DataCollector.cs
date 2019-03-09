using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Collects data from the object it's attached to during a trial/practice.
/// </summary>
public class DataCollector : MonoBehaviour, Collector
{
    public string fileName;

    [Header("References")]
    public DataManager dataManager;

    private StringBuilder stringBuilder;
    private Coroutine writerCoroutine;
    private float startingFrame, startingTime;

    private void Start()
    {
        // Give this to the manager
        dataManager.collectors.Add(this);
    }

    public void StartCollecting()
    {
        // Make the file
        string fullFileName = dataManager.testPath + "\\" + fileName;
        // Write the columns
        string output =
            "Time,Frame," +
            "Position_X,Position_Y,Position_Z," +
            "Rotation_X,Rotation_Y,Rotation_Z," +
            "Velocity_X,Velocity_Y,Velocity_Z\r\n";
        File.WriteAllText(fullFileName, output);

        startingFrame = Time.frameCount;
        startingTime = Time.time;
        
        stringBuilder = new StringBuilder();
        writerCoroutine = StartCoroutine(DataManager.WriterRoutine(dataManager, fullFileName, stringBuilder));
    }

    public void StopCollecting()
    {
        // Dump the stringbuilder
        File.AppendAllText(fileName, stringBuilder.ToString());
        stringBuilder = new StringBuilder();
    }

    private string Record()
    {
        object[] vals = {
            (Time.time - startingTime),
            (Time.frameCount - startingFrame),
            gameObject.transform.position.ToCSVFormat(),
            gameObject.transform.rotation.eulerAngles.ToCSVFormat(),
            gameObject.GetComponent<Rigidbody>().velocity.ToCSVFormat()};

        // Make those values into a comma separated line
        return DataManager.ToCSVLine(vals);
    }

    // Write every physics update
    void FixedUpdate()
    {
        // If the writer is off don't write anything. 
        // This is only a thing inside the editor for testing purposes.
        if (!dataManager.isActive())
            return;
        
        if (dataManager.Running())
            stringBuilder.Append(Record());
    }
}
